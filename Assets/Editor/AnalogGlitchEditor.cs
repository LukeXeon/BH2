using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnalogGlitch))]
    public class AnalogGlitchEditor : Editor
    {
        SerializedProperty _scanLineJitter;
        SerializedProperty _verticalJump;
        SerializedProperty _horizontalShake;
        SerializedProperty _colorDrift;

        void OnEnable()
        {
            _scanLineJitter = serializedObject.FindProperty("_scanLineJitter");
            _verticalJump = serializedObject.FindProperty("_verticalJump");
            _horizontalShake = serializedObject.FindProperty("_horizontalShake");
            _colorDrift = serializedObject.FindProperty("_colorDrift");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_scanLineJitter);
            EditorGUILayout.PropertyField(_verticalJump);
            EditorGUILayout.PropertyField(_horizontalShake);
            EditorGUILayout.PropertyField(_colorDrift);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
