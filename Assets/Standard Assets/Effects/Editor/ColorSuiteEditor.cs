using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ColorSuite)), CanEditMultipleObjects]
public class ColorSuiteEditor : Editor
{
    SerializedProperty propColorTemp;
    SerializedProperty propColorTint;

    SerializedProperty propToneMapping;
    SerializedProperty propExposure;

    SerializedProperty propSaturation;

    SerializedProperty propRCurve;
    SerializedProperty propGCurve;
    SerializedProperty propBCurve;
    SerializedProperty propCCurve;

    SerializedProperty propDitherMode;

    GUIContent labelColorTemp;
    GUIContent labelColorTint;

    void OnEnable()
    {
        propColorTemp = serializedObject.FindProperty("_colorTemp");
        propColorTint = serializedObject.FindProperty("_colorTint");

        propToneMapping = serializedObject.FindProperty("_toneMapping");
        propExposure    = serializedObject.FindProperty("_exposure");

        propSaturation = serializedObject.FindProperty("_saturation");

        propRCurve = serializedObject.FindProperty("_rCurve");
        propGCurve = serializedObject.FindProperty("_gCurve");
        propBCurve = serializedObject.FindProperty("_bCurve");
        propCCurve = serializedObject.FindProperty("_cCurve");

        propDitherMode = serializedObject.FindProperty("_ditherMode");

        labelColorTemp = new GUIContent("Color Temperature");
        labelColorTint = new GUIContent("Tint (green-purple)");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propToneMapping);
        if (propToneMapping.hasMultipleDifferentValues || propToneMapping.boolValue)
        {
            EditorGUILayout.Slider(propExposure, 0, 5);
            if (QualitySettings.activeColorSpace != ColorSpace.Linear)
                EditorGUILayout.HelpBox("Linear space lighting should be enabled for tone mapping.", MessageType.Warning);
        }

        EditorGUILayout.Space();

        EditorGUILayout.Slider(propColorTemp, -1.0f, 1.0f, labelColorTemp);
        EditorGUILayout.Slider(propColorTint, -1.0f, 1.0f, labelColorTint);

        EditorGUILayout.Space();

        EditorGUILayout.Slider(propSaturation, 0, 2);
        
        EditorGUILayout.LabelField("Curves (R, G, B, Combined)");
        EditorGUILayout.BeginHorizontal();
        var doubleHeight = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);
        EditorGUILayout.PropertyField(propRCurve, GUIContent.none, doubleHeight);
        EditorGUILayout.PropertyField(propGCurve, GUIContent.none, doubleHeight);
        EditorGUILayout.PropertyField(propBCurve, GUIContent.none, doubleHeight);
        EditorGUILayout.PropertyField(propCCurve, GUIContent.none, doubleHeight);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(propDitherMode);

        serializedObject.ApplyModifiedProperties();
    }
}
