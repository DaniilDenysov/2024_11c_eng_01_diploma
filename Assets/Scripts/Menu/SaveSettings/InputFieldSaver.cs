using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace SaveSystem
{
    /// <summary>
    /// Saves and loads the state of a Dropdown component.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldSaver : Saver // TODO: add better comments
    {
        [SerializeField] private TMP_InputField _inputField;

        public override void Construct()
        {
            _inputField = GetComponent<TMP_InputField>();
            base.Construct();
        }

        public override void Load(Tuple<string, object> data)
        {
            if (id != data.Item1) return;
            _inputField.SetTextWithoutNotify((string)data.Item2);
            Debug.Log($"Loaded data for Dropdown with GUID: {id}");
        }

        public override Tuple<string, object> Save()
        {
            return new Tuple<string, object>(id, _inputField.text);
        }
    }
}