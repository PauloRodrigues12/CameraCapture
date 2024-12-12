using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Niantic.Protobuf.WellKnownTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectiveController : MonoBehaviour
{
    // Outras scripts
    [Header("Tracking Scripts")]
    public TestLocationServices locationServices;
    public LogResults objectDetectionResult;
    public GetColor getColor;
    public OverpassQuery overpassQuery;
    public ClockTimer timer;

    [Header("Objective UI")]
    public TextMeshProUGUI locationTMP;
    public TextMeshProUGUI objectTMP;
    public TextMeshProUGUI lastObjectTMP;
    public GameObject endMenu;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip find;
    public AudioClip complete;

    public UnityEvent matchLocationEvent;
    public UnityEvent finishedObjectiveEvent;

    private Text locationDetectedCoords; // Obter as coordenadas do código do Tó
    public bool alwaysMatchCoords; // Playtesting
    public bool matchObjective; //Playtesting
    private float gameAreaRange;
    private float latitude, longitude;
    private float[] locationCoords;

    private Text objectDetectedName; // Obter a string do objeto detetado
    [SerializeField] private int newLocation, newObject, newColor;
    public int instructionIndex;
    [SerializedDictionary("Location", "Coordinates")]
    public SerializedDictionary<string, string> _Location;

    [SerializedDictionary("Object", "Colors")]
    public SerializedDictionary<string, string> _Object;
    public GameObject[] silhueta;
    public string[] color;
    public GameObject[] colorUI;
    public GameObject noColor;
    [SerializeField] private string[] usingColors;

    void Start()
    {
        instructionIndex = 0;
        locationCoords = new float[2];
    }
    void Update()
    {
        if (instructionIndex == 0)
        {
            PlayAudio(find);

            newLocation = UnityEngine.Random.Range(0, _Location.Count); // Valor aleatorio

            // Get coords
            string getCoords = _Location.Values.ToList().ElementAt(newLocation);
            UnityEngine.Debug.Log($"Raw getCoords: '{getCoords}'");

            getCoords = getCoords.Trim();
            string[] coordsTXT = getCoords.Split('/');

            locationCoords[0] = float.Parse(coordsTXT[0], CultureInfo.InvariantCulture);
            locationCoords[1] = float.Parse(coordsTXT[1], CultureInfo.InvariantCulture);

            instructionIndex++;
        }

        if (instructionIndex == 1)
        {
            string keyText = _Location.Keys.ToList().ElementAt(newLocation);
            if (locationTMP.enabled == true) locationTMP.text = keyText;
            //UnityEngine.Debug.Log(keyText);

            if (alwaysMatchCoords == true)
            {
                matchLocationEvent.Invoke();

                newObject = UnityEngine.Random.Range(0, _Object.Count); // Valor aleatorio

                string colors = _Object.Values.ToList().ElementAt(newObject);

                UnityEngine.Debug.Log("This are the colors" + colors);

                colors = colors.Trim();
                usingColors = colors.Split('/');

                if (usingColors[0] == "")
                {
                    noColor.SetActive(true);
                    newColor = 0;
                }
                else newColor = UnityEngine.Random.Range(0, usingColors.Length); // Valor aleatorio
                instructionIndex++;
            }
            else if (locationServices.locationDetected == keyText)
            {
                matchLocationEvent.Invoke();

                newObject = UnityEngine.Random.Range(0, _Object.Count); // Valor aleatorio

                string colors = _Object.Values.ToList().ElementAt(newObject);
                colors = colors.Trim();
                usingColors = colors.Split('/');

                if (usingColors[0] == "")
                {
                    noColor.SetActive(true);
                    newColor = 0;
                }
                else newColor = UnityEngine.Random.Range(0, usingColors.Length); // Valor aleatorio

                instructionIndex++;
            }
        }

        if (instructionIndex == 2)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            gameAreaRange = overpassQuery.GetDistanceFromUser(latitude, longitude, locationCoords[0], locationCoords[1]);

            if (gameAreaRange <= 50 && alwaysMatchCoords == false)
            {
                finishedObjectiveEvent.Invoke();
                instructionIndex = 0;
            }

            string keyText = _Object.Keys.ToList().ElementAt(newObject);

            // Activate Color UI
            if (noColor.activeSelf == false)
            {
                for (int i = 0; i < usingColors.Length; i++)
                {
                    if (i == newColor)
                    {
                        int pos = Int32.Parse(usingColors[i]);
                        colorUI[pos].SetActive(true);
                    }
                    else 
                    {
                        int pos = Int32.Parse(usingColors[i]);
                        colorUI[pos].SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < colorUI.Length; i++)
                {
                    colorUI[i].SetActive(false);
                }
            }

            if (objectTMP.enabled == true) objectTMP.text = keyText;

            for (int i = 0; i < silhueta.Length; i++)
            {
                if (i == newObject)
                {
                    silhueta[i].SetActive(true);
                }
                else silhueta[i].SetActive(false);
            }
        }

        if (timer.TimeLeft <= 0) endMenu.SetActive(true);
    }

    public void Capture()
    {
        if (instructionIndex == 2)
        {
            string keyText = _Object.Keys.ToList().ElementAt(newObject);
            if (objectTMP.enabled == true) objectTMP.text = keyText;

            string colorText = usingColors[newColor];
            if (noColor.activeSelf == true) colorText = null;

            if (lastObjectTMP.enabled == true) lastObjectTMP.text = objectDetectionResult.objectDetected;
            UnityEngine.Debug.Log(objectDetectionResult.objectDetected);

            //UnityEngine.Debug.Log(locationText);

            if (matchObjective == true)
            {
                objectiveMatched();
            }
            else if (objectDetectionResult.objectDetected == keyText && getColor.colorDetected == colorText)
            {
                objectiveMatched();
            }
            else if (objectDetectionResult.objectDetected == keyText && noColor.activeSelf == true)
            {
                objectiveMatched();
            }
        }
    }
    private void objectiveMatched()
    {
        endMenu.SetActive(true);
        PlayAudio(complete);
    }
    public void SetPassLocation(bool checkPass)
    {
        alwaysMatchCoords = checkPass;
    }
    public void SetObjective(bool checkPass)
    {
        matchObjective = checkPass;
    }
    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void StartNewObjective()
    {
        endMenu.SetActive(false);

        finishedObjectiveEvent.Invoke();

        instructionIndex = 0;
    }
    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }
}
