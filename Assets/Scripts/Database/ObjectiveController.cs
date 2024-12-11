using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public TestLocationServices locationServices;
    public LogResults objectDetectionResult;
    public GetColor getColor;

    public TextMeshProUGUI locationTMP;
    public TextMeshProUGUI objectTMP;
    public TextMeshProUGUI lastObjectTMP;

    public UnityEvent matchLocationEvent;
    public UnityEvent finishedObjectiveEvent;
    
    private Text locationDetectedCoords; // Obter as coordenadas do código do Tó
    public bool alwaysMatchCoords; // Playtesting
    private Text objectDetectedName; // Obter a string do objeto detetado
    private int newLocation, newObject, newColor;
    public int instructionIndex;
    [SerializedDictionary("Location", "Coordinates")]
    public SerializedDictionary<string, string> _Location;

    [SerializedDictionary("Object", "Colors")]
    public SerializedDictionary<string, string> _Object;
    public string[] color;

    void Start()
    {
        instructionIndex = 0;
    }
    void Update()
    {
        if (instructionIndex == 0)
        {
            newLocation = UnityEngine.Random.Range(0, _Location.Count); // Valor aleatorio
            instructionIndex++;
        }

        if (instructionIndex == 1)
        {
            string keyText = _Location.Keys.ToList().ElementAt(newLocation);
            if(locationTMP.enabled == true) locationTMP.text = keyText;
            //UnityEngine.Debug.Log(keyText);

            if  (alwaysMatchCoords == true)
            {
                matchLocationEvent.Invoke();
                UnityEngine.Debug.Log("teste123123123");

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
            string keyText = _Object.Keys.ToList().ElementAt(newObject);
            if(objectTMP.enabled == true) objectTMP.text = keyText;
        }
    }

    public void SetPassLocation(bool checkPass)
    {
        alwaysMatchCoords = checkPass;
    }

    public void Capture()
    {
        if (instructionIndex == 2)
        {
            string keyText = _Object.Keys.ToList().ElementAt(newObject);
            if(objectTMP.enabled == true) objectTMP.text = keyText;
            string colorText = color[newColor];

            if(lastObjectTMP.enabled == true) lastObjectTMP.text = objectDetectionResult.objectDetected;
            UnityEngine.Debug.Log(objectDetectionResult.objectDetected);
            
            //UnityEngine.Debug.Log(locationText);

            if (objectDetectionResult.objectDetected == keyText) //&& getColor.colorDetected == colorText)
            {
                finishedObjectiveEvent.Invoke();
                instructionIndex = 0;
            }
        }
    }
}
