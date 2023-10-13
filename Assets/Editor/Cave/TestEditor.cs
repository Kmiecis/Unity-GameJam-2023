using Game;
using UnityEditor;
using UnityEngine;

namespace GameEditor
{
    [CustomEditor(typeof(Test))]
    public class TestEditor : Editor
    {
        private Test Script
            => (Test)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Damage"))
            {
                Script.ApplyDamage();
            }
        }
    }
}