using UnityEngine;
using UnityEngine.UI;
using System;

namespace SaveSystem
{
    /// <summary>
    /// Saves and loads the state of a Toggle component.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class CheckboxSaver : Saver // TODO: add better comments
    {
        [SerializeField] private Toggle _toggle;

        public override void Construct()
        {
            _toggle = GetComponent<Toggle>();
            base.Construct();
        }

        public override void Load(Tuple<string, object> data)
        {
            if (id != data.Item1) return;
            _toggle.isOn = (bool)data.Item2;
            Debug.Log($"Loaded data for Checkbox with GUID: {id}");
        }

        public override Tuple<string, object> Save()
        {
            return new Tuple<string, object>(id, _toggle.isOn);
        }
    }
}