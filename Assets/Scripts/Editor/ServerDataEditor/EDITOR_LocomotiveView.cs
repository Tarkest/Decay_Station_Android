using UnityEngine;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using System;
using UnityEditor;

public class EDITOR_LocomotiveView
{
    #region DataBuffer

    private LocomotiveModel[] _locomotivesData;
    private ItemServerData[] _itemsData;
    private LocomotiveServerData _creationBuffer;

    #endregion

    private EDITOR_ServerData _window;
    private string _token;

    #region RenderVariables

    private Vector2 _instancesViewPos;
    private string _newLocomotiveName = "";
    private int _choosedLocomotiveIndex = -1;
    private int _choosedUpgradeLevel = -1;
    private bool _locomotivesTypesLoaded = false;
    private bool _itemsDataLoaded = false;
    private bool _upgradeEditMode = false;
    private bool _upgradeLevelChoosed = false;
    private bool _itemChoosingMode = false;
    private bool _itemAddingMode = false;
    private bool _createMode = false;

    #endregion

    public EDITOR_LocomotiveView(EDITOR_ServerData window, string token)
    {
        _window = window;
        _token = token;
        GetLocomotives();
    }

    #region Components

    public void LocomotivesView()
    {
        if (_locomotivesTypesLoaded && _itemsDataLoaded)
        {
            if (_createMode)
            {
                CreateView();
            }
            else
            {
                if(_upgradeEditMode)
                {
                    if(_upgradeLevelChoosed)
                    {
                        UpgradesList();
                    }
                    else
                    {
                        UpgradesLevelsList();
                    }
                }
                else
                {
                    LocomotivesList();
                }
            }
        }
        else
        {
            GUILayout.FlexibleSpace();
            LabelField("Loading...");
            GUILayout.FlexibleSpace();
        }
    }

    private void LocomotivesList()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _locomotiveIndex = 0; _locomotiveIndex < _locomotivesData.Length; _locomotiveIndex++)
        {
            BeginVertical("box");
            if(GUILayout.Button(_locomotivesData[_locomotiveIndex].serverData.name))
            {
                _choosedLocomotiveIndex = _locomotiveIndex;
                AssetDatabase.OpenAsset(_locomotivesData[_locomotiveIndex].prefab);
            }
            if(_locomotiveIndex == _choosedLocomotiveIndex)
            {
                LabelField(new GUIContent("Max level", "Maximum level of carriage"));
                _locomotivesData[_locomotiveIndex].serverData.maxLevel = IntSlider(_locomotivesData[_locomotiveIndex].serverData.maxLevel, 5, 12);
                if (GUILayout.Button("Change upgrades"))
                {
                    _upgradeEditMode = true;
                }
            }
            EndVertical();
        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button(new GUIContent("Create new locomotive")))
        {
            ClearData();
            _creationBuffer = new LocomotiveServerData();
            _createMode = true;
        }
    }

    private void UpgradesLevelsList()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _levelIndex = 0; _levelIndex < _locomotivesData[_choosedLocomotiveIndex].serverData.maxLevel - 1; _levelIndex++)
        {
            if(GUILayout.Button("Level " + (_levelIndex + 1)))
            {
                _choosedUpgradeLevel = _levelIndex;
                _upgradeLevelChoosed = true;
            }
        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button("Back"))
        {
            _upgradeEditMode = false;
        }
    }

    private void UpgradesList()
    {
        LocomotiveUpgradeItem[] _upgradesBuffer = (
            from _upgrade in _locomotivesData[_choosedLocomotiveIndex].serverData.upgradesRecipes
            where _upgrade.level == (_choosedUpgradeLevel + 1)
            select _upgrade).ToArray();
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _upgradeIndex = 0; _upgradeIndex < _upgradesBuffer.Length; _upgradeIndex++)
        {
            BeginVertical("box");
            BeginHorizontal();
            if (GUILayout.Button(_upgradesBuffer[_upgradeIndex].item.name))
            {
                _itemChoosingMode = true;
            }
            if(GUILayout.Button("X"))
            {

            }
            EndHorizontal();
            LabelField(new GUIContent("Count", "Count of items required for upgrade"));
            _upgradesBuffer[_upgradeIndex].count = IntSlider(_upgradesBuffer[_upgradeIndex].count, 1, _upgradesBuffer[_upgradeIndex].item.maxCount);
            EndVertical();
        }
        if(GUILayout.Button(new GUIContent("Add Item", "Add upgrade item to this level")))
        {

        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button("Back"))
        {
            _upgradeLevelChoosed = false;
        }
    }

    private void CreateView()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        LabelField("Name of new locomotive");
        _creationBuffer.name = TextField(_creationBuffer.name);
        LabelField("Max level");
        _creationBuffer.maxLevel = IntSlider(_creationBuffer.maxLevel, 5, 12);
        EndScrollView();
        GUILayout.Space(30);
        if(_creationBuffer.name.Length > 0)
        {
            if (GUILayout.Button("Create " + _creationBuffer.name))
            {
                CreateLocomotive();
            }
        }
        if (GUILayout.Button("Back"))
        {
            _createMode = false;
        }
    }

    private void ClearData()
    {

    }

    #endregion

    #region Network Methods

    private void GetLocomotives()
    {
        GetItemsData();
        GetLocomotivesTypes();
    }

    private void GetItemsData()
    {
        _itemsDataLoaded = false;
        EDITOR_Utility.GET("items", GetItemsDataCallback, _token);
    }

    private void GetLocomotivesTypes()
    {
        _locomotivesTypesLoaded = false;
        EDITOR_Utility.GET("locomotives", GetLocomotivesDataCallback, _token);
    }

    private void CreateLocomotive()
    {
        EDITOR_Utility.POST("locomotives", JSON.ToJSON(_creationBuffer), CreateLocomotiveCallback, _token);
    }

    #endregion

    #region Network CallBacks

    private void GetItemsDataCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _itemsData = JSON.FromJSONArray<ItemServerData>(data);
            _itemsDataLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void GetLocomotivesDataCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _locomotivesData = EDITOR_Utility.AssociateModels<LocomotiveModel, LocomotiveServerData, GameObject>(
                JSON.FromJSONArray<LocomotiveServerData>(data),
                (from _obj
                 in Resources.LoadAll("Locomotive/Instances")
                 let _typeObject = (GameObject)_obj
                 select _typeObject
                ).ToArray()
                 );
            _locomotivesTypesLoaded = true;
        }
        else
        {
            _window.SetError(error);
        }
    }

    private void CreateLocomotiveCallback(string data, string error)
    {
        _window.Focus();
        if (error == null)
        {
            _createMode = false;
            GetLocomotives();
        }
        else
        {
            _window.SetError(error);
        }
    }

    #endregion
}
