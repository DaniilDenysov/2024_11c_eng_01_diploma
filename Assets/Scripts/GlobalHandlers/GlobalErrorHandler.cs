using DesignPatterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalErrorHandler : Singleton<GlobalErrorHandler>
{
    [SerializeField] private ErrorWindow info, action;

    public override GlobalErrorHandler GetInstance()
    {
        return this;
    }

    public void DisplayInfoError(string text)
    {
        info.Construct(text);
    }

    public void DisplayActionError(string text, Action yesHandler = null, Action noHandler = null)
    {
        action.Construct(text,yesHandler,noHandler);
    }

}
