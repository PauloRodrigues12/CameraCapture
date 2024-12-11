using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // For TextMeshPro components
using Newtonsoft.Json.Linq; // Install Newtonsoft.Json via NuGet or Unity's Package Manager
using UnityEngine.Android; // Required for Android permissions

public class OverpassQuery : MonoBehaviour
{
    private const string OverpassApiUrl = "https://overpass-api.de/api/interpreter";

    [SerializeField] private string[] locationNames; // Array visible in the Inspector
    [SerializeField] private TMP_Text locationText;  // Reference to the TextMeshPro text component
    [SerializeField] private TMP_Text rangeText; // Text for displaying the distance

    private float latitude;
    private float longitude;

    private float monumentLatitude;
    private float monumentLongitude;

    private Dictionary<string, Vector2> monumentData = new Dictionary<string, Vector2>(); // Store monument names and coordinates

    // Combined query template with placeholders
    private string overpassQueryTemplate = @"
        [out:json][timeout:25];
        (
          node[""historic""][""name""](around:1000, {LAT}, {LON});
          way[""historic""][""name""](around:1000, {LAT}, {LON});
          relation[""historic""][""name""](around:1000, {LAT}, {LON});

          node[""amenity""=""fountain""](around:1000, {LAT}, {LON});
          way[""amenity""=""fountain""](around:1000, {LAT}, {LON});
          relation[""amenity""=""fountain""](around:1000, {LAT}, {LON});

          node[""shop""=""books""][""name""=""Livraria Lello & Irmão""](around:1000, {LAT}, {LON});
          way[""shop""=""books""][""name""=""Livraria Lello & Irmão""](around:1000, {LAT}, {LON});
          relation[""shop""=""books""][""name""=""Livraria Lello & Irmão""](around:1000, {LAT}, {LON});

          node[""amenity""=""fast_food""][""brand""=""McDonald's""][""name""=""McDonald's Imperial""](around:1000, {LAT}, {LON});
          way[""amenity""=""fast_food""][""brand""=""McDonald's""][""name""=""McDonald's Imperial""](around:1000, {LAT}, {LON});
          relation[""amenity""=""fast_food""][""brand""=""McDonald's""][""name""=""McDonald's Imperial""](around:1000, {LAT}, {LON});

          node[""amenity""=""concert_hall""][""name""=""Casa da Música""](around:1000, {LAT}, {LON});
          way[""amenity""=""concert_hall""][""name""=""Casa da Música""](around:1000, {LAT}, {LON});
          relation[""amenity""=""concert_hall""][""name""=""Casa da Música""](around:1000, {LAT}, {LON});

          node[""name""=""Cálem""](around:1000, {LAT}, {LON});
          way[""name""=""Cálem""](around:1000, {LAT}, {LON});
          relation[""name""=""Cálem""](around:1000, {LAT}, {LON});

          node[""name""=""Miradouro do teleférico""](around:1000, {LAT}, {LON});
          way[""name""=""Miradouro do teleférico""](around:1000, {LAT}, {LON});
          relation[""name""=""Miradouro do teleférico""](around:1000, {LAT}, {LON});
        );
        out body;
        >;
        out skel qt;
    ";

    void Start()
    {
#if PLATFORM_ANDROID
        RequestLocationPermission();
#endif
        StartCoroutine(GetUserLocation());
    }

    private void RequestLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Requesting location permission...");
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    IEnumerator GetUserLocation()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            locationText.text = "Location permission is not granted. Please enable it in settings.";
            yield break;
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "Location services are not enabled. Please enable them in your device settings.";
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "Unable to determine device location.";
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        Debug.Log($"User Location: Latitude = {latitude}, Longitude = {longitude}");
        Input.location.Stop();
        StartCoroutine(GetCombinedResults());
    }

    IEnumerator GetCombinedResults()
    {
        string overpassQuery = overpassQueryTemplate
            .Replace("{LAT}", latitude.ToString())
            .Replace("{LON}", longitude.ToString());

        UnityWebRequest request = UnityWebRequest.PostWwwForm(OverpassApiUrl, overpassQuery);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(overpassQuery));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            ProcessOverpassResponse(jsonResponse);
        }
        else
        {
            Debug.LogError($"Error fetching Overpass data: {request.error}");
            locationText.text = "Error fetching location data.";
        }
    }

    void ProcessOverpassResponse(string jsonResponse)
    {
        try
        {
            JObject response = JObject.Parse(jsonResponse);
            JArray elements = (JArray)response["elements"];

            foreach (JObject element in elements)
            {
                if (element["tags"]?["name"] != null)
                {
                    string name = element["tags"]["name"].ToString();
                    float lat = element["lat"] != null ? (float)element["lat"] : 0f;
                    float lon = element["lon"] != null ? (float)element["lon"] : 0f;

                    if (lat != 0f && lon != 0f)
                    {
                        monumentData[name] = new Vector2(lat, lon);
                    }
                }
            }

            locationNames = new List<string>(monumentData.Keys).ToArray();
            Debug.Log("Monuments found:");
            foreach (var entry in monumentData)
            {
                Debug.Log($"{entry.Key} at ({entry.Value.x}, {entry.Value.y})");
            }

            if (locationNames.Length > 0)
            {
                string randomMonument = locationNames[Random.Range(0, locationNames.Length)];
                locationText.text = $"Random Location: {randomMonument}";
                SetMonumentCoordinates(randomMonument);
            }
            else
            {
                locationText.text = "No locations found nearby.";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing response: {ex.Message}");
            locationText.text = "Error processing location data.";
        }
    }

    private void SetMonumentCoordinates(string monumentName)
    {
        if (monumentData.ContainsKey(monumentName))
        {
            Vector2 coordinates = monumentData[monumentName];
            monumentLatitude = coordinates.x;
            monumentLongitude = coordinates.y;
            Debug.Log($"Selected Monument: {monumentName} at ({monumentLatitude}, {monumentLongitude})");
        }
        else
        {
            Debug.LogWarning($"Monument {monumentName} not found in the data!");
        }
    }

    void Update()
    {
        if (monumentLatitude != 0 && monumentLongitude != 0)
        {
            float distance = GetDistanceFromUser(latitude, longitude, monumentLatitude, monumentLongitude);
            rangeText.text = $"Distance from Monument: {distance:F2} meters";

            if (distance <= 50)
            {
                rangeText.text = "USER IN RANGE";
            }
        }
    }

    public float GetDistanceFromUser(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f;
        float φ1 = Mathf.Deg2Rad * lat1;
        float φ2 = Mathf.Deg2Rad * lat2;
        float Δφ = Mathf.Deg2Rad * (lat2 - lat1);
        float Δλ = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(Δφ / 2) * Mathf.Sin(Δφ / 2) +
                  Mathf.Cos(φ1) * Mathf.Cos(φ2) *
                  Mathf.Sin(Δλ / 2) * Mathf.Sin(Δλ / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return R * c;
    }
}
