using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif
using UnityEngine;
using CustomTools;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class EditorPreCompilationUnit : MonoBehaviour
{
    /*
        private static bool subscribed = false;
        private static float lastUpdated = 0f, updateRate = 10f;

        [InitializeOnLoadMethod]
        private static void EnsureExistsInScene()
        {
            if (subscribed == true) return;
            Debug.Log("Subscribed");
            EditorApplication.hierarchyChanged += OnUpdated;
            subscribed = true;
        }

    private static void OnUpdated()
    {
        if (lastUpdated + updateRate > Time.time) return;
        if (!Application.isPlaying)
        {
            Debug.Log("Updated");
            lastUpdated = Time.time;
            // Iterate over all root game objects in the active scene
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject rootObject in rootObjects)
            {
                // Get all MonoBehaviour components attached to the root object and its children
                MonoBehaviour[] monoBehaviours = rootObject.GetComponentsInChildren<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    if (monoBehaviour == null) continue;

                    Type type = monoBehaviour.GetType();
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                    foreach (MethodInfo method in methods)
                    {
                        // Check if the method has the PreCompilationAttribute
                        if (method.GetCustomAttribute(typeof(PreCompilationConstructor)) != null)
                        {
                            Debug.Log($"Method {method.Name} in {type.Name} has the [PreCompilationConstructor] attribute.");

                            // Invoke the method if it has no parameters
                            if (method.GetParameters().Length == 0)
                            {
                                method.Invoke(monoBehaviour, null);
                            }
                        }
                    }
                }
            }
        }
  }*/
}
#endif
