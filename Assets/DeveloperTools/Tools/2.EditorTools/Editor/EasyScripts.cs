using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NeanderGiljam.EasyScripts
{
    /// <summary>
    /// This editor script reacts too dragging&dropping in the editor
    /// and creates a new gameobject with the name of a monobehaviour script that is being dropped.
    /// </summary>
    [InitializeOnLoad]
    public class EasyScripts
    {
        /// <summary>
        /// A reference to the hierarchy window.
        /// </summary>
        private static EditorWindow hierarchy;

        /// <summary>
        /// The icon to be shown when the show new gameobect area is enabled.
        /// </summary>
        private static Texture plusIcon;


        /// <summary>
        /// The current objects in the hierarchy. Used for calculating the area where items can be dropped.
        /// </summary>
        private static Dictionary<int, Rect> hierarchyObjects = new Dictionary<int, Rect>();

        /// <summary>
        /// Initalization of all the needed elements.
        /// </summary>
        static EasyScripts()
        {
            plusIcon = (Texture)Resources.Load("Sprites/noun_Plus_586827", typeof(Texture));
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        }

        /// <summary>
        /// Get and focus the wanted unity editor window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static EditorWindow GetFocusedWindow(string window)
        {
            EditorApplication.ExecuteMenuItem("Window/General/" + window);
            return EditorWindow.focusedWindow;
        }

        /// <summary>
        /// This function gets called by the Unty editor and is used to intercept editor actions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rect"></param>
        private static void OnHierarchyItemGUI(int id, Rect rect)
        {
            Event evt = Event.current;

            // Get the current hierarchy objects and add them to the dictionary if they are not already in it.
            GameObject gO = (GameObject)EditorUtility.InstanceIDToObject(id);
            if (gO != null && !hierarchyObjects.ContainsKey(id))
            {
                hierarchyObjects.Add(id, rect);
            }

            switch (evt.type)
            {
                // Detect drag and drop so we can perform our own bit of code on it.
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    bool overObject = false;

                    // Use the filled dictionary to check if the cursor is being dragged over an object in the hierarchy.
                    foreach (KeyValuePair<int, Rect> pair in hierarchyObjects)
                    {
                        overObject = pair.Value.Contains(Event.current.mousePosition);
                        if (overObject) break;
                    }

                    if (!overObject)
                    {
                        if (evt.type == EventType.DragPerform)
                        {
                            if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is MonoScript)
                            {
                                // Check if the dragged object is a script
                                System.Type type = (DragAndDrop.objectReferences[0] as MonoScript).GetClass();
                                // Check if the script is a MonoBehaviour
                                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                                {
                                    // Create a new GameObject and add the script as a component.
                                    GameObject newGO = new GameObject(DragAndDrop.objectReferences[0].name);
                                    newGO.AddComponent(type);
                                }
                            }
                        }
                        Event.current.Use();
                    }
                    break;

                default:
                    break;
            }

            // Detect if a hierarchy item is being deleted so that we can update the hierarchy objects dictionary.
            if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
            {
                hierarchyObjects.Clear();
            }
                
            
        }
    }
}