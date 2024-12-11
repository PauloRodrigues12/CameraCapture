using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niantic.Lightship.AR.ObjectDetection;
using TMPro;
using UnityEngine;

public class LogResults : MonoBehaviour
{
    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;
    [SerializeField] private float confidenceThreshold = .5f;
    private string resultString = "";
    public TextMeshProUGUI debugText;
    
    // Resultados finais
    [HideInInspector] public string objectDetected;
    void Start()
    {
        // Enable the object detection
        _objectDetectionManager.enabled = true;

        // Subscribe the event
        _objectDetectionManager.MetadataInitialized += ObjectDetectionManagerOnMetadataInitialized;
    }

    private void ObjectDetectionManagerOnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        // Subscribe the event
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void OnDestroy()
    {
        // Unsubscribe the event
        _objectDetectionManager.MetadataInitialized -= ObjectDetectionManagerOnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionManagerOnObjectDetectionsUpdated;

    }

    private void ObjectDetectionManagerOnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        // Update the object from the object detection manager
        var result = obj.Results;

        if (result == null)
        {
            return;
        }

        for (int i = 0; i < result.Count; i++)
        {
            var detection = result[i];
            var categories = detection.GetConfidentCategorizations(confidenceThreshold);

            if (categories.Count <= 0)
            {
                break;
            }
            
            categories.Sort((a,b) => b.Confidence.CompareTo(a.Confidence));

            // Check which category the object detected is
            for (int j = 0; j < categories.Count; j++)
            {
                var categoryToDisplay = categories[j];
                resultString = $"Detected: {categoryToDisplay.CategoryName}  with confidence {categoryToDisplay.Confidence} - ";
                
                objectDetected = $"{categoryToDisplay.CategoryName}";
            }
        }
    }

    void Update()
    {
        //debugText.text = resultString;
    }
}
