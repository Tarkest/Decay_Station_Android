using UnityEngine;
using UnityEditor;
using System.Linq;

public class ItemSystemWindow : EditorWindow
{
    enum WindowStage
    {
        Main,
        Add,
        Change
    }
    
    readonly string path = "Items/";

    WindowStage stage = WindowStage.Main;
    Vector2 scrollPosition = new Vector2();
    Item[] items = new Item[0];
    string input = "";
    int selGridInt = -1;

    Editor itemEditor;
    ItemCopy tmpItem;

    [MenuItem("Window/ItemSystem")]
    private static void ShowWindow()
    {
        GetWindow<ItemSystemWindow>(typeof(ItemSystemWindow));
    }

    private void OnEnable()
    {
        Get();
    }

    private void OnGUI()
    {
        switch(stage)
        {
            case WindowStage.Main:
                MainWindow();
                break;
            case WindowStage.Add:
                AddWindow();
                break;
            case WindowStage.Change:
                ChangeWindow();
                break;
        }
    }

    #region Windows

    private void MainWindow()
    {
        // Input
        GUILayout.Label("Enter item's name:");
        input = GUILayout.TextField(input);

        // Add and Get buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            WillAdd();
            stage = WindowStage.Add;
        }
        if (GUILayout.Button("Get"))
        {
            Get();
        }
        GUILayout.EndHorizontal();

        // Change and remove buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Change item"))
        {
            if (selGridInt != -1)
            {
                WillChange();
                stage = WindowStage.Change;
            }
        }
        if (GUILayout.Button("Remove item"))
        {
            if (selGridInt != -1)
            {
                Delete(items[selGridInt].Name);
                Get();
            }
        }
        GUILayout.EndHorizontal();

        // Items
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        if (items.Length == 0)
            GUILayout.Label("No match");
        else
            selGridInt = GUILayout.SelectionGrid(selGridInt, items.Select((item) => new GUIContent($"name: {item.Name}")).ToArray(), 1);
        GUILayout.EndScrollView();

    }

    private void AddWindow()
    {
        EditorGUIUtility.labelWidth = 60;

        // Add item label
        #region Heading
        GUILayout.BeginHorizontal();
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            EditorGUILayout.LabelField("Add item", style, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        #endregion

        itemEditor.OnInspectorGUI();

        // Apply/Cancel buttons
        #region Footer   
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
            {
                if(ItemEditor.Validation((itemEditor.target as Item)))
                    Add();
            }
            if (GUILayout.Button("Cancel"))
            {
                Cancel();
            }
            GUILayout.Space(10);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        #endregion
    }

    private void ChangeWindow()
    {
        EditorGUIUtility.labelWidth = 60;

        // Change item label
        #region Heading
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        EditorGUILayout.LabelField("Change item", style, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        #endregion

        // Item inspector
        itemEditor.OnInspectorGUI();

        // Apply/Cancel buttons
        #region Footer   
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Apply"))
        {
            if (ItemEditor.Validation((itemEditor.target as Item)))
                Change();
        }
        if (GUILayout.Button("Cancel"))
        {
            Cancel();
        }
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        #endregion
    }

    #endregion

    #region Actions

    /// <summary>
    /// Define itemEditor of empty item providing input fields
    /// </summary>
    private void WillAdd()
    {
        itemEditor = Editor.CreateEditor(Resources.Load($"{path}_defaultItem"));
        (itemEditor.target as Item).ApplyCopy(new ItemCopy());
        (itemEditor.target as Item).Name = input;
    }

    /// <summary>
    /// Append item with derived options
    /// </summary>
    private void Add()
    {
        Item newItem = CreateInstance<Item>();
        newItem.ApplyCopy(new ItemCopy((itemEditor.target as Item)));
        AssetDatabase.CreateAsset(newItem, $"Assets/Resources/{path}{newItem.Name}.asset");
        stage = WindowStage.Main;
        Get();
    }

    /// <summary>
    /// Removes asset with derived name
    /// </summary>
    /// <param name="name">Name of item asset file</param>
    private void Delete(string name)
    {
        AssetDatabase.DeleteAsset($"Assets/Resources/{path}{name}.asset");
    }

    /// <summary>
    /// Gets all items in Resources/Items folder
    /// </summary>
    private void Get()
    {
        items = Resources.LoadAll<Item>($"{path}");
        items = items.Where((item) => item.name == "_defaultItem" ? 
            false : input.Length < item.Name.Length ? 
                input == item.Name.Substring(0, input.Length) : input.Length == item.Name.Length ? 
                    input == item.Name : false).ToArray();
        selGridInt = -1;
    }

    /// <summary>
    /// Action that invoked before transition to change window to save prev state of item (provides undo possibility)
    /// </summary>
    private void WillChange()
    {
        itemEditor = Editor.CreateEditor(Resources.Load($"{path}{items[selGridInt].Name}"));
        tmpItem = new ItemCopy(itemEditor.target as Item);
    }

    /// <summary>
    /// Undo all changes to asset and change window
    /// </summary>
    private void Cancel()
    {
        (itemEditor.target as Item).ApplyCopy(tmpItem);
        stage = WindowStage.Main;
        Get();
    }

    /// <summary>
    /// Accepts all changes on change window and changes window to main window
    /// </summary>
    private void Change()
    {
        AssetDatabase.RenameAsset($"Assets/Resources/{path}{tmpItem.Name}.asset", (itemEditor.target as Item).Name);
        stage = WindowStage.Main;
    }

    #endregion

}
