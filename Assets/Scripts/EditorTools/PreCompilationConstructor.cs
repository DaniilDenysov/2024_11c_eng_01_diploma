using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTools
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class PreCompilationConstructor : Attribute
    {
    }
}
