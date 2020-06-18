using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(BuildingTypeData))]
public class INSPEC_BuildingTypeData : Editor
{
    SerializedProperty names;
    AnimBool opened = new AnimBool(false);

    private void OnEnable()
    {
        names = serializedObject.FindProperty("names");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        opened.target = BeginFoldoutHeaderGroup(opened.target, "Names");
        if (BeginFadeGroup(opened.faded))
        {
            LabelField("English");
            names.GetArrayElementAtIndex(0).stringValue = TextArea(names.GetArrayElementAtIndex(0).stringValue);
            Space();
            LabelField("Russian");
            names.GetArrayElementAtIndex(1).stringValue = TextArea(names.GetArrayElementAtIndex(1).stringValue);
            Space();
            LabelField("German");
            names.GetArrayElementAtIndex(2).stringValue = TextArea(names.GetArrayElementAtIndex(2).stringValue);
            Space();
            LabelField("French");
            names.GetArrayElementAtIndex(3).stringValue = TextArea(names.GetArrayElementAtIndex(3).stringValue);
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
