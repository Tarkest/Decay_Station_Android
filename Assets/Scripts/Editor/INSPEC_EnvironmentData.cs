﻿using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(EnvironmentData))]
public class INSPEC_EnvironmentData : Editor
{
    SerializedProperty _environmentSprites;
    SerializedProperty _environmentRailsBackground;
    SerializedProperty _environmentRails;
    SerializedProperty _environmentRailsForeground;
    AnimBool[] _showArray = new AnimBool[24];
    EnvironmentData data;

    private void OnEnable()
    {
        _environmentSprites = serializedObject.FindProperty("environmentSprites");
        _environmentRailsBackground = serializedObject.FindProperty("railsBackground");
        _environmentRails = serializedObject.FindProperty("rails");
        _environmentRailsForeground = serializedObject.FindProperty("railsForeground");
        for (int i = 0; i < 24; i++)
        {
            _showArray[i] = new AnimBool(false);
            _showArray[i].valueChanged.AddListener(Repaint);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LabelField(new GUIContent("Background"));
        for (int i = 0; i < 10; i++)
        {
            _showArray[i].target = BeginFoldoutHeaderGroup(_showArray[i].target, $"{i + 1} layer sprites");
            if (BeginFadeGroup(_showArray[i].faded))
            {
                BeginVertical("box");
                for (int sprI = 0; sprI < _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize; sprI++)
                {
                    BeginHorizontal();
                    ObjectField(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                    if (GUILayout.Button("X"))
                    {
                        _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                        _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                    }
                    EndHorizontal();
                }
                if (GUILayout.Button("Add variation"))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize);
                }
                EndVertical();
            }
            EndFadeGroup();
            EndFoldoutHeaderGroup();
        }
        Space();
        LabelField(new GUIContent("Rails"));
        _showArray[10].target = BeginFoldoutHeaderGroup(_showArray[10].target, "Rails Background");
        if (BeginFadeGroup(_showArray[10].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRailsBackground.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRailsBackground.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRailsBackground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                    _environmentRailsBackground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            if (GUILayout.Button("Add variation"))
            {
                _environmentRailsBackground.FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentRailsBackground.FindPropertyRelative("variation").arraySize);
            }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showArray[11].target = BeginFoldoutHeaderGroup(_showArray[11].target, "Rails");
        if (BeginFadeGroup(_showArray[11].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRailsBackground.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRails.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRails.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                    _environmentRails.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            if (GUILayout.Button("Add variation"))
            {
                _environmentRails.FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentRails.FindPropertyRelative("variation").arraySize);
            }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showArray[12].target = BeginFoldoutHeaderGroup(_showArray[12].target, "Rails Foreground");
        if (BeginFadeGroup(_showArray[12].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRailsForeground.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRailsForeground.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRailsForeground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                    _environmentRailsForeground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            if (GUILayout.Button("Add variation"))
            {
                _environmentRailsForeground.FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentRailsForeground.FindPropertyRelative("variation").arraySize);
            }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        Space();
        LabelField(new GUIContent("Foreground"));
        for (int i = 10; i < 20; i++)
        {
            _showArray[i + 3].target = BeginFoldoutHeaderGroup(_showArray[i + 3].target, $"{i - 9} layer sprites");
            if (BeginFadeGroup(_showArray[i + 3].faded))
            {
                BeginVertical("box");
                for (int sprI = 0; sprI < _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize; sprI++)
                {
                    BeginHorizontal();
                    ObjectField(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                    if (GUILayout.Button("X"))
                    {
                        _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                        _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                    }
                    EndHorizontal();
                }
                if (GUILayout.Button("Add variation"))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize);
                }
                EndVertical();
            }
            EndFadeGroup();
            EndFoldoutHeaderGroup();
        }
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPreview(typeof(EnvironmentData))]
public class EnvironmentPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {

    }
}
