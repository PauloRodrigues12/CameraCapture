using UnityEngine;
using System.Collections;
using TMPro;

public class TestLocationServices : MonoBehaviour
{

    public TextMeshProUGUI _locationText;

    // Resultados finais
    [HideInInspector] public string locationDetected;

    IEnumerator Start()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            _locationText.text = "Location not enabled on device or app does not have permission to access location";
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            yield break;
        }

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes.
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds, cancel the location service use.
        if (maxWait < 1)
        {
            _locationText.text = "Timed out";
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed, cancel location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            _locationText.text = "Unable to determine device location";
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, continuously log the location.
            while (true)
            {
                //_locationText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " +
                          //Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " +
                          //Input.location.lastData.timestamp;

                locationDetected = Input.location.lastData.latitude + " " + Input.location.lastData.longitude;

             // Retrieve and log the current location
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " +
                          Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " +
                          Input.location.lastData.timestamp);

                // Wait for a second before querying again (optional: adjust the time for how often you want the update)
                yield return new WaitForSeconds(1);
            }
        }
    }

    // Optionally stop location service when the object is destroyed
    void OnDestroy()
    {
        Input.location.Stop();
    }
}
