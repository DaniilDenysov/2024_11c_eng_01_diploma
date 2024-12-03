namespace CustomTools
{
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UnityEngine.Object), true), CanEditMultipleObjects]
    internal class ObjectEditor : Editor
    {
        private ButtonsDrawer _buttonsDrawer;
        private readonly GUIStyle _style = new GUIStyle();

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
            _style.fontStyle = FontStyle.Bold;
            _style.normal.textColor = Color.white;
        }

        // Custom inspector GUI
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _buttonsDrawer.DrawButtons(targets);
        }

        // Custom scene GUI
       /* public void OnSceneGUI()
        {
            var targetObject = target as UnityEngine.Object;

            if (targetObject == null)
                return;

            var targetType = targetObject.GetType();
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var fieldType = field.FieldType;

                if (fieldType == typeof(Vector2))
                {
                    var vector2Value = (Vector2)field.GetValue(targetObject);
                    Handles.Label(vector2Value, field.Name);
                    var newPosition = (Vector2)Handles.PositionHandle(vector2Value, Quaternion.identity);
                    if (vector2Value != newPosition)
                    {
                        Undo.RecordObject(targetObject, "Move Vector2 Handle");
                        field.SetValue(targetObject, newPosition);
                    }
                }
                else if (fieldType.IsArray && fieldType.GetElementType() == typeof(Vector2))
                {
                    var arrayValue = (Vector2[])field.GetValue(targetObject);
                    for (int i = 0; i < arrayValue.Length; i++)
                    {
                        var vector2Value = arrayValue[i];
                        Handles.Label(vector2Value, $"{field.Name}[{i}]");
                        var newPosition = (Vector2)Handles.PositionHandle(vector2Value, Quaternion.identity);
                        if (vector2Value != newPosition)
                        {
                            Undo.RecordObject(targetObject, "Move Vector2 Handle");
                            arrayValue[i] = newPosition;
                            field.SetValue(targetObject, arrayValue);
                        }
                    }
                }
            }
        }*/


        private void DrawHandleForVector2Property(SerializedProperty property)
        {
            Handles.Label(property.vector2Value, property.displayName);
            property.vector2Value = Handles.PositionHandle(property.vector2Value, Quaternion.identity);
        }
    }
}
