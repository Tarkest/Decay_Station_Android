using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using static UnityEditor.EditorGUILayout;
using UnityEngine;

public class EDITOR_Locomotive : EditorWindow
{
    private Vector2 instancesViewPos;
    private Vector2 inspectorViewPos;
    private List<GameObject> _localLocomotiveData;
    private List<GameObject> _localLocomotiveDataFiltered;
    private int _selectedInstanceIndex = -1;
    private string _instancesFilter = "";

    [MenuItem("Editors/Locomotive")]
    public static void ShowLocomotiveEditorWindow()
    {
        GetWindow<EDITOR_Locomotive>("Locomotive Editor");
    }

    private void OnEnable()
    {
        _selectedInstanceIndex = -1;
        _localLocomotiveData = (from _obj in Resources.LoadAll("Locomotive/Instances") let _locoObj = (GameObject)_obj select _locoObj).ToList();

        

    }

    void OnGUI()
    {
        BeginHorizontal();
        BeginVertical("box");
        _instancesFilter = TextField(_instancesFilter, GUILayout.Width(300));
        instancesViewPos = BeginScrollView(instancesViewPos, GUILayout.Width(300), GUILayout.ExpandHeight(true));
        if(_instancesFilter.Length > 0)
        {
            _localLocomotiveDataFiltered = (from _obj in _localLocomotiveData where _obj.name.Contains(_instancesFilter) select _obj).ToList();
            for (int _locoIndex = 0; _locoIndex < _localLocomotiveDataFiltered.Count; _locoIndex++)
            {
                BeginVertical("box");
                if (GUILayout.Button(_localLocomotiveDataFiltered[_locoIndex].name))
                {
                    _selectedInstanceIndex = _localLocomotiveData.IndexOf(_localLocomotiveDataFiltered[_locoIndex]);
                }
                if(_selectedInstanceIndex == _localLocomotiveData.IndexOf(_localLocomotiveDataFiltered[_locoIndex]))
                {
                    BeginVertical("box");
                    GUIStyle _statusStyle = new GUIStyle();
                    _statusStyle.normal.textColor = Color.red;
                    BeginHorizontal();
                    LabelField(new GUIContent("Server status: ", "Locomotive state on server batabase"), GUILayout.Width(85));
                    LabelField(new GUIContent("Not Loaded", "Instance not associated with entity on server"), _statusStyle, GUILayout.Width(120));
                    EndHorizontal();
                    LabelField("Associated with: " + "00.00.001" + " version");
                    EndVertical();
                }
                EndVertical();
            }
        }
        else
        {
            for (int _locoIndex = 0; _locoIndex < _localLocomotiveData.Count; _locoIndex++)
            {
                BeginVertical("box");
                if (GUILayout.Button(_localLocomotiveData[_locoIndex].name))
                {
                    _selectedInstanceIndex = _locoIndex;
                }
                if (_selectedInstanceIndex == _locoIndex)
                {
                    BeginVertical("box");
                    GUIStyle _statusStyle = new GUIStyle();
                    _statusStyle.normal.textColor = Color.red;
                    BeginHorizontal();
                    LabelField(new GUIContent("Server status: ", "Locomotive state on server batabase"), GUILayout.Width(85));
                    LabelField(new GUIContent("Not Loaded", "Instance not associated with entity on server"), _statusStyle, GUILayout.Width(120));
                    EndHorizontal();
                    LabelField("Associated with: " + "00.00.001" + " version");
                    EndVertical();
                }
                EndVertical();
            }
        } 
        EndScrollView();
        if (GUILayout.Button(new GUIContent("Create instance", "Create new local locomotive instence"), GUILayout.Width(300)))
        {

        }
        EndVertical();
        BeginVertical("box", GUILayout.MinWidth(300), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EndVertical();
        BeginVertical("box");
        inspectorViewPos = BeginScrollView(inspectorViewPos, GUILayout.MinWidth(300), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if(_selectedInstanceIndex != -1)
        {
            LocomotiveAgent _agent = _localLocomotiveData[_selectedInstanceIndex].GetComponent<LocomotiveAgent>();
            LocomotiveForeground _forreground = _localLocomotiveData[_selectedInstanceIndex].GetComponentInChildren<LocomotiveForeground>();
            if(_agent != null)
            {
                BeginHorizontal();
                LabelField(new GUIContent("Instance id: ", "Id of entity on server"), GUILayout.Width(85));
                SelectableLabel(_agent.id.ToString());
                EndHorizontal();
            }

        }
        else
        {
            
        }
        EndScrollView();
        EndVertical();
        EndHorizontal();
    }
}
