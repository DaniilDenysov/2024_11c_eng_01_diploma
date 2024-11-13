using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.CommandPattern
{
    public abstract class Command
    {
        public abstract void Apply();
        public abstract void Revert();
    }
}
