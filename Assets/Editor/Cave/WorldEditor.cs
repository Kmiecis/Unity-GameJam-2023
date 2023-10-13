using Game;
using UnityEditor;
using UnityEngine;

namespace GameEditor
{
    [CustomEditor(typeof(World))]
    public class WorldEditor : Editor
    {
        private World Script
            => (World)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Next"))
            {
                Script.NextLevel();
            }
        }
    }
}