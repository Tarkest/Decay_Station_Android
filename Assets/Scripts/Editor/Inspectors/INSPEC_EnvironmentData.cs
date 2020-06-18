using UnityEngine;
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
    SerializedProperty _environmentSplice;
    SerializedProperty _names;
    AnimBool[] _showArray = new AnimBool[26];

    private void OnEnable()
    {
        _environmentSprites = serializedObject.FindProperty("environmentSprites");
        _environmentRailsBackground = serializedObject.FindProperty("railsBackground");
        _environmentRails = serializedObject.FindProperty("rails");
        _environmentRailsForeground = serializedObject.FindProperty("railsForeground");
        _environmentSplice = serializedObject.FindProperty("environmentSplice");
        _names = serializedObject.FindProperty("names");
        for (int i = 0; i < 26; i++)
        {
            _showArray[i] = new AnimBool(false);
            _showArray[i].valueChanged.AddListener(Repaint);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _showArray[0].target = BeginFoldoutHeaderGroup(_showArray[0].target, "Names");
        if (BeginFadeGroup(_showArray[0].faded))
        {
            LabelField("English");
            _names.GetArrayElementAtIndex(0).stringValue = TextArea(_names.GetArrayElementAtIndex(0).stringValue);
            Space();
            LabelField("Russian");
            _names.GetArrayElementAtIndex(1).stringValue = TextArea(_names.GetArrayElementAtIndex(1).stringValue);
            Space();
            LabelField("German");
            _names.GetArrayElementAtIndex(2).stringValue = TextArea(_names.GetArrayElementAtIndex(2).stringValue);
            Space();
            LabelField("French");
            _names.GetArrayElementAtIndex(3).stringValue = TextArea(_names.GetArrayElementAtIndex(3).stringValue);
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        Space();
        LabelField(new GUIContent("Background"));
        for (int i = 1; i < 11; i++)
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
                    }
                    EndHorizontal();
                }
                Space();
                if (GUILayout.Button("Add variation"))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize);
                }
                Space();
                BeginHorizontal();
                if (_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue = Toggle(new GUIContent("Moving when static", "Check if layer must move when train static"), _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue = FloatField(new GUIContent("Move speed", "Speed with what layer will move"), _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue);
                }
                EndHorizontal();
                if (_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue && _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue <= 0)
                    HelpBox("Move speed must be higher then 0", MessageType.Error, true);
                Space();
                EndVertical();
            }
            EndFadeGroup();
            EndFoldoutHeaderGroup();
        }
        Space();
        LabelField(new GUIContent("Rails"));
        _showArray[11].target = BeginFoldoutHeaderGroup(_showArray[11].target, "Rails Background");
        if (BeginFadeGroup(_showArray[11].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRailsBackground.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRailsBackground.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRailsBackground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            Space();
            if (GUILayout.Button("Add variation"))
            {
                _environmentRailsBackground.FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentRailsBackground.FindPropertyRelative("variation").arraySize);
            }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showArray[12].target = BeginFoldoutHeaderGroup(_showArray[12].target, "Rails");
        if (BeginFadeGroup(_showArray[12].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRails.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRails.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRails.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            Space();
            if (GUILayout.Button("Add variation"))
            {
                _environmentRails.FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentRails.FindPropertyRelative("variation").arraySize);
            }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showArray[13].target = BeginFoldoutHeaderGroup(_showArray[13].target, "Rails Foreground");
        if (BeginFadeGroup(_showArray[13].faded))
        {
            BeginVertical("box");
            for (int sprI = 0; sprI < _environmentRailsForeground.FindPropertyRelative("variation").arraySize; sprI++)
            {
                BeginHorizontal();
                ObjectField(_environmentRailsForeground.FindPropertyRelative("variation").GetArrayElementAtIndex(sprI), typeof(Sprite), new GUIContent($"{sprI + 1} variation"));
                if (GUILayout.Button("X"))
                {
                    _environmentRailsForeground.FindPropertyRelative("variation").DeleteArrayElementAtIndex(sprI);
                }
                EndHorizontal();
            }
            Space();
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
        for (int i = 11; i < 21; i++)
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
                    }
                    EndHorizontal();
                }
                Space();
                if (GUILayout.Button("Add variation"))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").InsertArrayElementAtIndex(_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("variation").arraySize);
                }
                Space();
                BeginHorizontal();
                if (_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue = Toggle(new GUIContent("Moving when static", "Check if layer must move when train static"), _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue))
                {
                    _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue = FloatField(new GUIContent("MoveSpeed", "Speed with what layer will move"), _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue);
                }
                EndHorizontal();
                if (_environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("isMovingWhenStatic").boolValue && _environmentSprites.GetArrayElementAtIndex(i).FindPropertyRelative("staticMoveSpeed").floatValue <= 0)
                    HelpBox("Move speed must be higher then 0", MessageType.Error, true);
                Space();
                EndVertical();
            }
            EndFadeGroup();
            EndFoldoutHeaderGroup();
        }
        Space();
        _showArray[25].target = BeginFoldoutHeaderGroup(_showArray[25].target, $"Splice");
        if (BeginFadeGroup(_showArray[24].faded))
        {
            BeginVertical("box");
                    ObjectField(_environmentSplice.FindPropertyRelative("sprites").GetArrayElementAtIndex(0), typeof(Sprite), new GUIContent($"Start sprite"));
                    ObjectField(_environmentSplice.FindPropertyRelative("sprites").GetArrayElementAtIndex(1), typeof(Sprite), new GUIContent($"Middle sprite"));
                    ObjectField(_environmentSplice.FindPropertyRelative("sprites").GetArrayElementAtIndex(2), typeof(Sprite), new GUIContent($"End sprite"));
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
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
