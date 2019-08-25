using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 60;
        Item item = (Item)target;

        GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ", GUILayout.Width(80));
            item.Name = GUILayout.TextField(item.Name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("Amount: ", GUILayout.Width(80));
            item.Amount = EditorGUILayout.IntField(item.Amount);
        GUILayout.EndHorizontal();
    }
}
