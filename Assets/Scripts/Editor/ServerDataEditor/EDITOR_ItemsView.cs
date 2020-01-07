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
    private int _typeChoosed = 0;

    private bool _itemsLoaded = false;
    private bool _typesLoaded = false;
    private bool _raririesLoaded = false;
    private int _selectedTypeIndex = 0;
    private int _selectedRarityIndex = 0;

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
            _instancesViewPos = BeginScrollView(_instancesViewPos);
            for (int _itemIndex = 0; _itemIndex < _items.Length; _itemIndex++)
            {
                BeginVertical("box");
                if (GUILayout.Button(_items[_itemIndex].serverData.name))
                {
                    _choosedItemIndex = _itemIndex;

                }
                if(_itemIndex == _choosedItemIndex)
                {
                    if(_items[_itemIndex].serverData.updateBuffer != null)
                    {
                        _typeChoosed = GUILayout.Toolbar(_typeChoosed, new string[] { "Current", "Incomming" });
                        switch(_typeChoosed)
                        {
                            case 0:
                                RenderOpen(_items[_itemIndex].serverData, false);
                                break;

                            case 1:
                                RenderOpen(_items[_itemIndex].serverData, true);
                                break;
                        }

                    }
                }
                
                EndVertical();
            }
            EndScrollView();
            GUILayout.Space(30);
            LabelField("Create new item");
            BeginHorizontal();
            _newItemName = TextField(_newItemName);
            if(_newItemName.Length > 0)
            {
                if (GUILayout.Button(new GUIContent("Add", "Add " + _newItemName + " to items")))
                {

                }
            }
            EndHorizontal();
        }
        else
        {
            GUILayout.FlexibleSpace();
            LabelField("Loading...");
            GUILayout.FlexibleSpace();
        }
    }

    private void RenderOpen(ItemServerData data, bool editable)
    {
        if(editable)
        {
            LabelField(new GUIContent("Max count in stack", "Maximum count in one invetory slot"));
            data.maxCount = IntField(data.maxCount);
            LabelField(new GUIContent("", ""));
            _selectedTypeIndex = Popup(_selectedTypeIndex, (from _data in _itemsTypes select _data.name).ToArray());
        }
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
            _items = EDITOR_Utility.AssociateModels<ItemModel, ItemServerData, ItemsTypesData>(
                    JSON.FromJSONArray<ItemServerData>(data),
                    (from _obj
                     in Resources.LoadAll("UI/UIData/ItemsTypesData")
                     let _typeObject = (ItemsTypesData)_obj
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

    #endregion
}
