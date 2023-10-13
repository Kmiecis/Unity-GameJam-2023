using Game;
using UnityEditor;
using UnityEngine;

namespace GameEditor
{
    [CustomEditor(typeof(CaveManager))]
    public class CaveManagerEditor : Editor
    {
        private CaveManager Script
            => (CaveManager)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Damage"))
            {
                Script.Damage(Script.position.x, Script.position.y, Script.radius);
            }
        }
    }
}