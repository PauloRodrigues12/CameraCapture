using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject instructionPanel; // The panel to show instructions
    public GameObject[] otherPanels;   // Panels to deactivate when the instruction panel is shown
    public TextMeshProUGUI text; // TextMeshPro that is going to blink
    public float blinkSpeed = 1f; // Blink Speed

    private bool isFadingOut = true;

    void Update()
    {
         // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch phase is "Began" (initial tap)
            if (touch.phase == TouchPhase.Began)
            {
                ShowInstructions();
            }
        }

        // Blinking animation 
        
        Color color = text.color;
        float alphaChange = blinkSpeed * Time.deltaTime;

        if (isFadingOut)
            color.a -= alphaChange;
        else
            color.a += alphaChange;

        if (color.a <= 0)
        {
            color.a = 0;
            isFadingOut = false;
        }
        else if (color.a >= 1)
        {
            color.a = 1;
            isFadingOut = true;
        }

        text.color = color;
    }

    // Show the instruction panel and disable the logo and Text
    private void ShowInstructions()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);

            foreach (GameObject panel in otherPanels)
            {
                if (panel != null)
                {
                    panel.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("Instruction panel is not assigned!");
        }
    }
 
}
