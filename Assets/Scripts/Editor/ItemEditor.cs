using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        // Name input field
        GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ", GUILayout.Width(80));
            item.Name = GUILayout.TextField(item.Name);
        GUILayout.EndHorizontal();

        // Icon field
        GUILayout.BeginHorizontal();
            item.Icon = (Sprite)EditorGUILayout.ObjectField("Icon: ",item.Icon, typeof(Sprite), allowSceneObjects: true, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

    }

    public static bool Validation(Item item)
    {
        // Name empty
        if (string.IsNullOrEmpty(item.Name))
            return false;

        // Icon = null
        if (item.Icon == null)
            return false;

        return true;
    }
}
