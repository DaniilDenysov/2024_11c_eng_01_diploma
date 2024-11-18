using UnityEngine;
using UnityEngine.UI;
using System;

namespace SaveSystem
{
    /// <summary>
    /// Saves and loads the state of a Slider component.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class SliderSaver : Saver // TODO: add better comments
    {
        [SerializeField] private Slider slider;

        public override void Construct()
        {
            slider = GetComponent<Slider>();
            base.Construct();
        }

        public override void Load(Tuple<string, object> data)
        {
            if (id != data.Item1) return;
            slider.value = (float)((double)data.Item2);
            Debug.Log("Loaded data for Slider with GUID: " + id);
        }

        public override Tuple<string, object> Save()
        {
            return new Tuple<string, object>(id, slider.value);
        }
    }
}