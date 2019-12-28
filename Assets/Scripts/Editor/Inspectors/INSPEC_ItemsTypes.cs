using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(ItemsTypesData))]
public class INSPEC_ItemsTypes : Editor
{
    SerializedProperty names;
    SerializedProperty color;
    AnimBool opened = new AnimBool(false);

    private void OnEnable()
    {
        names = serializedObject.FindProperty("names");
        color = serializedObject.FindProperty("viewColor");
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
        LabelField(new GUIContent("Color at view", "Color for text where rarity value is showing"));
        color.colorValue = ColorField(color.colorValue);
        serializedObject.ApplyModifiedProperties();
    }
}
