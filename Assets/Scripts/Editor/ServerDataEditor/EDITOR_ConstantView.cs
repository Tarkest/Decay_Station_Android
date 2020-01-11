using UnityEngine;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using System;
using UnityEditor;

public class EDITOR_ConstantView
{
    #region DataBuffer

    private ConstantsModel[] _itemsRarities;
    private ConstantsModel[] _itemsTypes;

    #endregion

    private EDITOR_ServerData _window;
    private string _token;

    #region RenderVariables

    public enum ConstantsTypes
    {
        ItemType,
        ItemRarity
    }

    private Vector2 _instancesViewPos;
    private ConstantsTypes _currentChoosedConstantType;
    private bool _constantTypeChoosed = false;
    private int _choosedConstantIndex = -1;
    private string _newConstantName = "";

    private bool _itemsTypesLoaded = false;
    private bool _raririesLoaded = false;

    #endregion

    public EDITOR_ConstantView(EDITOR_ServerData window, string token)
    {
        _window = window;
        _token = token;
        GetConstants();
    }

    #region Components

    public void ConstantsTypesView()
    {
        if (_itemsTypesLoaded && _raririesLoaded)
        {
            if (_constantTypeChoosed)
            {
                ConstantsTypeView();
            }
            else
            {
                BeginVertical("box");
                foreach (string type in Enum.GetNames(typeof(ConstantsTypes)))
                {
                    if (GUILayout.Button(type))
                    {
                        Enum.TryParse(type, out _currentChoosedConstantType);
                        _constantTypeChoosed = true;
                    }
                }
                EndVertical();
            }
        }
        else
        {
            GUILayout.FlexibleSpace();
            LabelField("Loading...");
            GUILayout.FlexibleSpace();
        }
    }

    private void ConstantsTypeView()
    {
        BeginVertical("box");
        switch (_currentChoosedConstantType)
        {
            case ConstantsTypes.ItemType:
                ConstantView(ConstantsTypes.ItemType, _itemsTypes);
                break;

            case ConstantsTypes.ItemRarity:
                ConstantView(ConstantsTypes.ItemRarity, _itemsRarities);
                break;
        }
        if (GUILayout.Button("Back"))
        {
            _choosedConstantIndex = -1;
            _constantTypeChoosed = false;
        }
        EndHorizontal();
    }

    private void ConstantView(ConstantsTypes model, ConstantsModel[] data)
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int constantIndex = 0; constantIndex < data.Length; constantIndex++)
        {
            BeginHorizontal();
            if (_choosedConstantIndex == constantIndex)
            {
                GUI.color = Color.cyan;
            }
            if (GUILayout.Button(data[constantIndex].serverData.name))
            {
                AssetDatabase.OpenAsset(data[constantIndex].prefab);
                _choosedConstantIndex = constantIndex;
            }
            if (_choosedConstantIndex == constantIndex)
            {
                GUI.color = Color.white;
            }
            if (GUILayout.Button(new GUIContent("X", "Delete " + data[constantIndex].serverData.name + " " + model.ToString()), GUILayout.Width(25)))
            {
                DeleteConstantType(model, data[constantIndex].serverData.id);
            }
            EndHorizontal();
        }
        EndScrollView();
        GUILayout.Space(30);
        LabelField("Create new " + model.ToString());
        BeginHorizontal();
        _newConstantName = TextField(_newConstantName);
        if(_newConstantName.Length > 0)
        {
            if (GUILayout.Button(new GUIContent("Add", "Add " + _newConstantName + " to " + model.ToString())))
            {
                CreateConstant(model, _newConstantName);
            }
        }
        EndHorizontal();
    }

    #endregion

    #region Network Methods

    private void GetConstants()
    {
        GetItemsTypes();
        GetItemsRarity();
    }

    private void GetItemsTypes()
    {
        _itemsTypesLoaded = false;
        EDITOR_Utility.GET("constants/itemstypes", GetItemsTypeCallback, _token);
    }

    private void GetItemsRarity()
    {
        _raririesLoaded = false;
        EDITOR_Utility.GET("constants/itemsrarities", GetItemsRarityCallback, _token);
    }

    private void CreateConstant(ConstantsTypes type, string name)
    {
        switch (type)
        {
            case ConstantsTypes.ItemType:
                EDITOR_Utility.POST("constants/itemstypes", JSON.ToJSON(new NewEntity(name)), CreateItemsTypeCallback, _token);
                break;

            case ConstantsTypes.ItemRarity:
                EDITOR_Utility.POST("constants/itemsrarities", JSON.ToJSON(new NewEntity(name)), CreateItemsRarityCallback, _token);
                break;
        }
    }

    private void DeleteConstantType(ConstantsTypes type, int id)
    {
        switch (type)
        {
            case ConstantsTypes.ItemType:
                EDITOR_Utility.DELETE("constants/itemstypes", id, DeleteItemsTypeCallback, _token);
                break;

            case ConstantsTypes.ItemRarity:
                EDITOR_Utility.DELETE("constants/itemsrarities", id, DeleteItemsRarityCallback, _token);
                break;
        }
    }

    #endregion

    #region Network CallBacks

    private void GetItemsTypeCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _itemsTypes = EDITOR_Utility.AssociateModels<ConstantsModel, ConstantsServerData, ItemsTypesData>(
                JSON.FromJSONArray<ConstantsServerData>(data),
                (from _obj
                 in Resources.LoadAll("UI/UIData/ItemsTypesData")
                 let _typeObject = (ItemsTypesData)_obj
                 select _typeObject
                ).ToArray()
                 );
            _itemsTypesLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void GetItemsRarityCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _itemsRarities = EDITOR_Utility.AssociateModels<ConstantsModel, ConstantsServerData, ItemsRaritiesData>(
                JSON.FromJSONArray<ConstantsServerData>(data),
                (from _obj
                 in Resources.LoadAll("UI/UIData/ItemsRaritiesData")
                 let _typeObject = (ItemsRaritiesData)_obj
                 select _typeObject
                ).ToArray()
                 );
            _raririesLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void CreateItemsTypeCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _newConstantName = "";
            ConstantsServerData _data = JSON.FromJSON<ConstantsServerData>(data);
            ItemsTypesData _newData = ScriptableObject.CreateInstance<ItemsTypesData>();
            _newData.name = _data.name;
            _newData.names[0] = _data.name;
            AssetDatabase.CreateAsset(_newData, "Assets/Resources/UI/UIData/ItemsTypesData/" + _newData.name + ".asset");
            GetItemsTypes();
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void CreateItemsRarityCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _newConstantName = "";
            ConstantsServerData _data = JSON.FromJSON<ConstantsServerData>(data);
            ItemsRaritiesData _newData = ScriptableObject.CreateInstance<ItemsRaritiesData>();
            _newData.name = _data.name;
            _newData.names[0] = _data.name;
            AssetDatabase.CreateAsset(_newData, "Assets/Resources/UI/UIData/ItemsRaritiesData/" + _newData.name + ".asset");
            GetItemsRarity();
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void DeleteItemsTypeCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            ConstantsServerData _deletedEntity = JSON.FromJSON<ConstantsServerData>(data);
            AssetDatabase.DeleteAsset("Assets/Resources/UI/UIData/ItemsTypesData/" + _deletedEntity.name + ".asset");
            GetItemsTypes();
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void DeleteItemsRarityCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            ConstantsServerData _deletedEntity = JSON.FromJSON<ConstantsServerData>(data);
            AssetDatabase.DeleteAsset("Assets/Resources/UI/UIData/ItemsRaritiesData/" + _deletedEntity.name + ".asset");
            GetItemsRarity();
        }
        else
        {
            _window.SetError(error);
        }
    }

    #endregion
}
