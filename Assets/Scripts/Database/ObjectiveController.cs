using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AYellowpaper.SerializedCollections;
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
    private int newLocation, newObject, newColor;
    public int instructionIndex;
    [SerializedDictionary("Location", "Coordinates")]
    public SerializedDictionary<string, string> _Location;

    [SerializedDictionary("Object", "Colors")]
    public SerializedDictionary<string, string> _Object;
    public GameObject[] silhueta;
    public string[] color;

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
                newColor = UnityEngine.Random.Range(0, color.Length); // Valor aleatorio

                instructionIndex++;
            }
            else if (locationServices.locationDetected == keyText)
            {
                matchLocationEvent.Invoke();
                UnityEngine.Debug.Log("teste123123123");

                newObject = UnityEngine.Random.Range(0, _Object.Count); // Valor aleatorio
                newColor = UnityEngine.Random.Range(0, color.Length); // Valor aleatorio

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
            string colorText = color[newColor];

            if (lastObjectTMP.enabled == true) lastObjectTMP.text = objectDetectionResult.objectDetected;
            UnityEngine.Debug.Log(objectDetectionResult.objectDetected);

            //UnityEngine.Debug.Log(locationText);

            if (matchObjective == true)
            {
                endMenu.SetActive(true);
                PlayAudio(complete);
            }
            else if (objectDetectionResult.objectDetected == keyText) //&& getColor.colorDetected == colorText)
            {
                endMenu.SetActive(true);
                PlayAudio(complete);
            }
        }
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
