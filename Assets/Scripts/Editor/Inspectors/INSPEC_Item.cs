using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(ItemObject))]
public class INSPEC_Item : Editor
{
    SerializedProperty names;
    SerializedProperty descriptions;
    SerializedProperty icon;
    AnimBool namesOpened = new AnimBool(false);
    AnimBool descriptionsOpened = new AnimBool(false);

    private void OnEnable()
    {
        names = serializedObject.FindProperty("names");
        descriptions = serializedObject.FindProperty("descriptions");
        icon = serializedObject.FindProperty("icon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ObjectField(icon, typeof(Sprite), new GUIContent("Item icon", "Icon what will be displayed at inventory"));
        namesOpened.target = BeginFoldoutHeaderGroup(namesOpened.target, "Names");
        if (BeginFadeGroup(namesOpened.faded))
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
        Space();
        descriptionsOpened.target = BeginFoldoutHeaderGroup(descriptionsOpened.target, "Descriptions");
        if (BeginFadeGroup(descriptionsOpened.faded))
        {
            LabelField("English");
            descriptions.GetArrayElementAtIndex(0).stringValue = TextArea(names.GetArrayElementAtIndex(0).stringValue);
            Space();
            LabelField("Russian");
            descriptions.GetArrayElementAtIndex(1).stringValue = TextArea(names.GetArrayElementAtIndex(1).stringValue);
            Space();
            LabelField("German");
            descriptions.GetArrayElementAtIndex(2).stringValue = TextArea(names.GetArrayElementAtIndex(2).stringValue);
            Space();
            LabelField("French");
            descriptions.GetArrayElementAtIndex(3).stringValue = TextArea(names.GetArrayElementAtIndex(3).stringValue);
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
