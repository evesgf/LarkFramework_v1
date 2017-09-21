using LarkFramework.UI;
using UnityEditor;
using UnityEngine;

namespace LarkFramework.Extension
{
    [CustomEditor(typeof(UIPanel),true)]
    public class GUIAnimSystemExtensionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Find GUI Anim Component"))
            {
                Debug.Log("Find（{0}）GUI Anim Component");
            }
        }

    }
}
