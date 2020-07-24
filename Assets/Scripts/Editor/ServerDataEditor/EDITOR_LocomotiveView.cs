using UnityEngine;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using System;
using UnityEditor;
using System.Collections.Generic;

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
    private int _choosedLocomotiveIndex = -1;
    private int _choosedUpgradeLevel = -1;
    private int _choosedUpgradeIndex = -1;
    private bool _locomotivesDataLoaded = false;
    private bool _itemsDataLoaded = false;
    private bool _upgradeEditMode = false;
    private bool _upgradeLevelChoosed = false;
    private bool _itemChoosingMode = false;
    private bool _buildingSlotsEdit = false;
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
        if (_locomotivesDataLoaded && _itemsDataLoaded)
        {
            if (_createMode)
            {
                if(_upgradeEditMode)
                {
                    if(_upgradeLevelChoosed)
                    {
                        if (_itemChoosingMode)
                        {
                            ItemChooseView();
                        }
                        else
                        {
                            UpgradesList();
                        }
                    }
                    else
                    {
                        UpgradesLevelsList();
                    }
                }
                else
                {
                    CreateView();
                }
            }
            else
            {
                if(_upgradeEditMode)
                {
                    if(_upgradeLevelChoosed)
                    {
                        if(_itemChoosingMode)
                        {
                            ItemChooseView();
                        }
                        else
                        {
                            UpgradesList();
                        }
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
            if (GUILayout.Button(_locomotivesData[_locomotiveIndex].serverData.name))
            {
                _choosedLocomotiveIndex = _locomotiveIndex;
                AssetDatabase.OpenAsset(_locomotivesData[_locomotiveIndex].prefab);
            }
            if (_locomotiveIndex == _choosedLocomotiveIndex)
            {
                LabelField(new GUIContent("Max level", "Maximum level of carriage"));
                int _currentMaxLevel = _locomotivesData[_locomotiveIndex].serverData.maxLevel;
                int _newMaxLevel = IntSlider(_currentMaxLevel, 5, 12);
                if (_currentMaxLevel != _newMaxLevel)
                {
                    Array.Resize(ref _locomotivesData[_locomotiveIndex].serverData.upgradesRecipes, _newMaxLevel);
                    for (int _addedIndex = 0; _addedIndex < _newMaxLevel - _currentMaxLevel; _addedIndex++)
                    {
                        _locomotivesData[_locomotiveIndex].serverData.upgradesRecipes[_currentMaxLevel + _addedIndex] = new LocomotiveUpgradeItem(_currentMaxLevel + _addedIndex);
                    }
                }
                _locomotivesData[_locomotiveIndex].serverData.maxLevel = _newMaxLevel;
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
        if(_createMode)
        {
            for (int _levelIndex = 0; _levelIndex < _creationBuffer.maxLevel - 1; _levelIndex++)
            {
                if (GUILayout.Button("Level " + (_levelIndex + 1)))
                {
                    _choosedUpgradeLevel = _levelIndex;
                    _upgradeLevelChoosed = true;
                }
            }
        }
        else
        {
            for (int _levelIndex = 0; _levelIndex < _locomotivesData[_choosedLocomotiveIndex].serverData.maxLevel - 1; _levelIndex++)
            {
                if (GUILayout.Button("Level " + (_levelIndex + 1)))
                {
                    _choosedUpgradeLevel = _levelIndex;
                    _upgradeLevelChoosed = true;
                }
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
        LocomotiveUpgradeItem[] _upgradesBuffer;
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        if (_createMode)
        {
            if(_creationBuffer.upgradesRecipes == null)
            {
                _creationBuffer.upgradesRecipes = new LocomotiveUpgradeItem[0];
            }
            _upgradesBuffer = (
                from _upgrade in _creationBuffer.upgradesRecipes
                where _upgrade.level == (_choosedUpgradeLevel + 1)
                select _upgrade)
            .ToArray();
        }
        else
        {
            _upgradesBuffer = (
                from _upgrade in _locomotivesData[_choosedLocomotiveIndex].serverData.upgradesRecipes
                where _upgrade.level == (_choosedUpgradeLevel + 1)
                select _upgrade)
            .ToArray();
        }
        for (int _upgradeIndex = 0; _upgradeIndex < _upgradesBuffer.Length; _upgradeIndex++)
        {
            BeginVertical("box");
            if (GUILayout.Button(_upgradesBuffer[_upgradeIndex].item != null ? _upgradesBuffer[_upgradeIndex].item.name : "Not choosed"))
            {
                _itemChoosingMode = true;
                if(_createMode)
                {
                    _choosedUpgradeIndex = Array.IndexOf(_creationBuffer.upgradesRecipes, _upgradesBuffer[_upgradeIndex]);
                }
                else
                {
                    _choosedUpgradeIndex = Array.IndexOf(_locomotivesData[_choosedLocomotiveIndex].serverData.upgradesRecipes, _upgradesBuffer[_upgradeIndex]);
                }
            }
            if(_upgradesBuffer[_upgradeIndex].item != null)
            {
                LabelField(new GUIContent("Count", "Count of items required for upgrade"));
                _upgradesBuffer[_upgradeIndex].count = IntSlider(_upgradesBuffer[_upgradeIndex].count, 1, _upgradesBuffer[_upgradeIndex].item.maxCount);
            }
            EndVertical();
        }
        if (GUILayout.Button(new GUIContent("+", "Add item")))
        {
            if (_createMode)
            {
                Array.Resize(ref _creationBuffer.upgradesRecipes, _creationBuffer.upgradesRecipes.Length + 1);
                _creationBuffer.upgradesRecipes[_creationBuffer.upgradesRecipes.Length - 1] = new LocomotiveUpgradeItem(_choosedUpgradeLevel + 1);
            }
        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button("Back"))
        {
            _upgradeLevelChoosed = false;
        }
    }

    private void ItemChooseView()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _itemIndex = 0; _itemIndex < _itemsData.Length; _itemIndex++)
        {
            if(GUILayout.Button(_itemsData[_itemIndex].name))
            {
                if(_createMode)
                {
                    _creationBuffer.upgradesRecipes[_choosedUpgradeIndex].item = _itemsData[_itemIndex];
                    _itemChoosingMode = false;
                }
            }
        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button("Cancel"))
        {
            _itemChoosingMode = false;
        }
    }

    private void BuildingSlotsView()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        for (int _buildingSlotsIndex = 0; _buildingSlotsIndex < _itemsData.Length; _buildingSlotsIndex++)
        {

        }
        EndScrollView();
        GUILayout.Space(30);
        if (GUILayout.Button("Back"))
        {
            _buildingSlotsEdit = false;
        }
    }

    private void CreateView()
    {
        _instancesViewPos = BeginScrollView(_instancesViewPos);
        LabelField("Name of new locomotive");
        _creationBuffer.name = TextField(_creationBuffer.name);
        LabelField("Max level");
        _creationBuffer.maxLevel = IntSlider(_creationBuffer.maxLevel, 5, 12);
        if(GUILayout.Button("Edit upgrades"))
        {
            _upgradeEditMode = true;
        }
        if (GUILayout.Button("Edit buildings slots"))
        {
            _buildingSlotsEdit = true;
        }
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
        _choosedLocomotiveIndex = -1;
        _choosedUpgradeLevel = -1;
        _choosedUpgradeIndex = -1;
        _upgradeEditMode = false;
        _upgradeLevelChoosed = false;
        _itemChoosingMode = false;
        _createMode = false;
        _creationBuffer = null;
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
        _locomotivesDataLoaded = false;
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
            _locomotivesDataLoaded = true;
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
            ClearData();
            LocomotiveServerData _data = JSON.FromJSON<LocomotiveServerData>(data);
            GameObject _newLocomotive = new GameObject(_data.name, typeof(SpriteRenderer), typeof(EdgeCollider2D), typeof(LocomotiveAgent));
            GameObject _foreground = new GameObject("Foreground", typeof(SpriteRenderer), typeof(LocomotiveForeground));
            GameObject _wheels1 = new GameObject("Wheel 1", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
            GameObject _wheels2 = new GameObject("Wheel 2", typeof(SpriteRenderer), typeof(Animator), typeof(TrainWheels));
            _foreground.transform.SetParent(_newLocomotive.transform);
            _wheels1.transform.SetParent(_newLocomotive.transform);
            _wheels2.transform.SetParent(_newLocomotive.transform);
            _foreground.transform.SetSiblingIndex(0);
            _wheels1.transform.SetSiblingIndex(1);
            _wheels2.transform.SetSiblingIndex(2);
            LocomotiveAgent _newLocomotiveAgent = _newLocomotive.GetComponent<LocomotiveAgent>();
            _newLocomotiveAgent.ApplyDataFromEditor(_data.maxLevel);
            _newLocomotiveAgent.wheels = new TrainWheels[] { _wheels1.GetComponent<TrainWheels>(), _wheels2.GetComponent<TrainWheels>() };
            PrefabUtility.SaveAsPrefabAsset(_newLocomotive, "Assets/Resources/Locomotive/Instances/" + _newLocomotive.name + ".prefab");
            UnityEngine.Object.DestroyImmediate(_newLocomotive);
            GetLocomotives();
        }
        else
        {
            _window.SetError(error);
        }
    }

    #endregion
}
