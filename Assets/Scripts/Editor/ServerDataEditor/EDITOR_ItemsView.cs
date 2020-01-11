using UnityEngine;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using System;
using UnityEditor;

public class EDITOR_ItemsView
{
    #region DataBuffer

    private ItemModel[] _items;
    private ConstantsServerData[] _itemsTypes;
    private ConstantsServerData[] _itemsRarities;

    #endregion

    private EDITOR_ServerData _window;
    private string _token;

    #region RenderVariables

    private Vector2 _instancesViewPos;
    private int _choosedItemIndex = -1;
    private string _newItemName = "";
    private int _newItemMaxCount = 0;
    private int _typeChoosed = -1;
    private int _lastTypeChoosed = -2;
    private bool _createMode = false;

    private bool _itemsLoaded = false;
    private bool _typesLoaded = false;
    private bool _raririesLoaded = false;
    private int _selectedTypeIndex = -1;
    private int _selectedRarityIndex = -1;

    #endregion

    public EDITOR_ItemsView(EDITOR_ServerData window, string token)
    {
        _window = window;
        _token = token;
        GetItems();
    }

    #region Components

    public void ItemsView()
    {
        if (_itemsLoaded && _typesLoaded && _raririesLoaded)
        {
            if(_createMode)
            {
                CreateView();
            }
            else
            {
                ListView();
            }
        }
        else
        {
            GUILayout.FlexibleSpace();
            LabelField("Loading...");
            GUILayout.FlexibleSpace();
        }
    }

    private void ListView()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _itemIndex = 0; _itemIndex < _items.Length; _itemIndex++)
        {
            BeginVertical("box");
            if (GUILayout.Button(_items[_itemIndex].serverData.name))
            {
                _choosedItemIndex = _itemIndex;
                AssetDatabase.OpenAsset(_items[_itemIndex].prefab);
            }
            if (_itemIndex == _choosedItemIndex)
            {
                if (_items[_itemIndex].serverData.updateBuffer.name != null)
                {
                    _typeChoosed = GUILayout.Toolbar(_typeChoosed, new string[] { "Current", "Incomming" });
                    switch (_typeChoosed)
                    {
                        case 0:
                            if (_lastTypeChoosed != _typeChoosed) UpdateValues(_items[_itemIndex].serverData);
                            RenderOpen(_items[_itemIndex].serverData);
                            break;

                        case 1:
                            if (_lastTypeChoosed != _typeChoosed) UpdateValues(_items[_itemIndex].serverData.updateBuffer);
                            RenderOpenEdit(_items[_itemIndex].serverData.updateBuffer);
                            break;
                    }
                    _lastTypeChoosed = _typeChoosed;
                }
                else
                {
                    if(_newItemMaxCount == 0) UpdateValues(_items[_itemIndex].serverData);
                    RenderOpenEdit(_items[_itemIndex].serverData);
                }
            }
            EndVertical();
        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button(new GUIContent("Create new item")))
        {
            ClearData();
            _createMode = true;
        }
    }

    private void RenderOpenEdit(ItemData data)
    {
        LabelField(new GUIContent("Max count in stack", "How many items can be stored in one inventory slot"));
        _newItemMaxCount = IntField(_newItemMaxCount);
        LabelField(new GUIContent("Item type", "Type of item"));
        _selectedTypeIndex = Popup(_selectedTypeIndex, (from _data in _itemsTypes select _data.name).ToArray());
        LabelField(new GUIContent("New item rarity", "Rarity of item what will be created"));
        _selectedRarityIndex = Popup(_selectedRarityIndex, (from _data in _itemsRarities select _data.name).ToArray());
        BeginHorizontal();
        if (GUILayout.Button("Update"))
        {
            UpdateItem(data.id, _itemsTypes[_selectedTypeIndex].id, _itemsRarities[_selectedRarityIndex].id, _newItemMaxCount);
        }
        if(data is ItemServerData)
        {
            if (GUILayout.Button(
                (data as ItemServerData).inRotation ?
                new GUIContent("Remove", "Removes item from pool what server can give to player") :
                new GUIContent("Add", "Add item from pool what server can give to player")
            ))
            {
                UpdateRotationStatus(data.id);
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Remove", "Remove planned update from object")))
            {
                RemoveUpdate(data.id);
            }
        }
        if (GUILayout.Button("Delete"))
        {

        }
        EndHorizontal();
    }

    private void RenderOpen(ItemData data)
    {
        LabelField(new GUIContent("Max count in stack", "How many items can be stored in one inventory slot"));
        LabelField(_newItemMaxCount.ToString());
        LabelField(new GUIContent("Item type", "Type of item"));
        LabelField((from _data in _itemsTypes select _data.name).ToArray()[_selectedTypeIndex]);
        LabelField(new GUIContent("New item rarity", "Rarity of item what will be created"));
        LabelField((from _data in _itemsRarities select _data.name).ToArray()[_selectedRarityIndex]);
        if (GUILayout.Button(
            (data as ItemServerData).inRotation ?
            new GUIContent("Remove", "Removes item from pool what server can give to player") :
            new GUIContent("Add", "Add item from pool what server can give to player")
        ))
        {
            UpdateRotationStatus(data.id);
        }
    }

    private void CreateView()
    {
        LabelField(new GUIContent("New item name"));
        _newItemName = TextField(_newItemName);
        LabelField(new GUIContent("New item type", "Type of item what will be created"));
        _selectedTypeIndex = Popup(_selectedTypeIndex, (from _data in _itemsTypes select _data.name).ToArray());
        LabelField(new GUIContent("New item rarity", "Rarity of item what will be created"));
        _selectedRarityIndex = Popup(_selectedRarityIndex, (from _data in _itemsRarities select _data.name).ToArray());
        LabelField(new GUIContent("Max count in stack", "How many items can be stored in one inventory slot"));
        _newItemMaxCount = IntSlider(_newItemMaxCount, 1, 500);
        if(_newItemName.Length > 0)
        {
            if(GUILayout.Button("Create " + _newItemName))
            {
                CreateItem(_newItemName, _itemsTypes[_selectedTypeIndex].id, _itemsRarities[_selectedRarityIndex].id, _newItemMaxCount);
            }
        }
        if(GUILayout.Button("Cancel"))
        {
            ClearData();
        }
    }

    private void UpdateValues(ItemData data)
    {
        _newItemMaxCount = data.maxCount;
        _selectedTypeIndex = Array.FindIndex(_itemsTypes, (a) => a.id == data.type.id);
        _selectedRarityIndex = Array.FindIndex(_itemsRarities, (a) => a.id == data.rarity.id);
    }

    private void ClearData()
    {
        _lastTypeChoosed = -2;
        _createMode = false;
        _selectedTypeIndex = -1;
        _selectedRarityIndex = -1;
        _newItemMaxCount = 0;
        _newItemName = "";
    }

    #endregion

    #region NetworkMethods

    private void GetItems()
    {
        GetItemsTypes();
        GetItemsRarity();
        GetItemsData();
    }

    private void GetItemsTypes()
    {
        _typesLoaded = false;
        EDITOR_Utility.GET("constants/itemstypes", GetItemsTypeCallback, _token);
    }

    private void GetItemsRarity()
    {
        _raririesLoaded = false;
        EDITOR_Utility.GET("constants/itemsrarities", GetItemsRarityCallback, _token);
    }

    private void GetItemsData()
    {
        _itemsLoaded = false;
        EDITOR_Utility.GET("items", GetItemsDataCallback, _token);
    }

    private void CreateItem(string name, int typeid, int rarityId, int maxCount)
    {
        EDITOR_Utility.POST("items", JSON.ToJSON(new ItemServerData(name, maxCount, typeid, rarityId)), CreateItemCallBack, _token);
    }

    private void UpdateItem(int id, int typeid, int rarityId, int maxCount)
    {
        EDITOR_Utility.PUT("items", JSON.ToJSON(new ItemServerData(id, maxCount, typeid, rarityId)), UpdateItemCallback, _token);
    }

    private void UpdateRotationStatus(int id)
    {
        EDITOR_Utility.PUT("items/rotation", JSON.ToJSON(new ItemServerData(id)), UpdateItemCallback, _token);
    }

    private void RemoveUpdate(int id)
    {
        EDITOR_Utility.DELETE("items/update", id, UpdateItemCallback, _token);
    }

    #endregion

    #region NetworkCallbacks

    private void GetItemsTypeCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _itemsTypes = JSON.FromJSONArray<ConstantsServerData>(data);
            _typesLoaded = true;
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
            _itemsRarities = JSON.FromJSONArray<ConstantsServerData>(data);
            _raririesLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void GetItemsDataCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _items = EDITOR_Utility.AssociateModels<ItemModel, ItemServerData, ItemObject>(
                    JSON.FromJSONArray<ItemServerData>(data),
                    (from _obj
                     in Resources.LoadAll("Items")
                     let _typeObject = (ItemObject)_obj
                     select _typeObject
                    ).ToArray()
                     );
            _itemsLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void CreateItemCallBack(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            ClearData();
            ItemServerData _data = JSON.FromJSON<ItemServerData>(data);
            ItemObject _newItem = ScriptableObject.CreateInstance<ItemObject>();
            _newItem.name = _data.name;
            _newItem.names[0] = _data.name;
            AssetDatabase.CreateAsset(_newItem, "Assets/Resources/Items/" + _newItem.name + ".asset");
            GetItems();
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void UpdateItemCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            ClearData();
            GetItems();
        }
        else
        {
            _window.SetError(error);
        }
    }

    #endregion
}
