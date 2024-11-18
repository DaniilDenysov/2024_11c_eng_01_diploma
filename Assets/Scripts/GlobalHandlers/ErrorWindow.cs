using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MonoBehaviour
{
    [SerializeField] private Button yes, no;
    [SerializeField] private TMP_Text display; 
    [SerializeField] private ViewAnimationHandler animationHandler;


    public void Construct(string text = "Error", Action yesHandler = null, Action noHandler = null)
    {
        display.text = text;
        animationHandler.OpenWindow();
        yes.onClick.RemoveAllListeners();
        no.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() => animationHandler.CloseWindow());
        no.onClick.AddListener(() => animationHandler.CloseWindow());
        if (yesHandler != null)
        {
            yes.onClick.AddListener(() => yesHandler());
        }

        if (noHandler != null)
        {
            no.onClick.AddListener(() => noHandler());
        }
    }
}
