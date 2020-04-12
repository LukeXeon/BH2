using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DigitalGlitch))]
    public class DigitalGlitchEditor : Editor
    {
        SerializedProperty _intensity;

        void OnEnable()
        {
            _intensity = serializedObject.FindProperty("_intensity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_intensity);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
