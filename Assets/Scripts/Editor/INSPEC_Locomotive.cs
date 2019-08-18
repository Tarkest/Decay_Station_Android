using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(LocomotiveAgent))]
public class INSPEC_Locomotive : Editor
{
    SerializedProperty _sprites;
    SerializedProperty _buildingPosition;
    SerializedProperty _wheels;
    AnimBool _showSprites;
    AnimBool _showBuildings;
    AnimBool _showWheels;

    private void OnEnable()
    {
        _sprites = serializedObject.FindProperty("sprites");
        _buildingPosition = serializedObject.FindProperty("buildingsPositions");
        _wheels = serializedObject.FindProperty("wheels");
        _showSprites = new AnimBool(false);
        _showBuildings = new AnimBool(false);
        _showWheels = new AnimBool(false);
        _showSprites.valueChanged.AddListener(Repaint);
        _showBuildings.valueChanged.AddListener(Repaint);
        _showWheels.valueChanged.AddListener(Repaint);
        serializedObject.Update();
        if (_sprites.arraySize < 5)
        {
            _sprites.arraySize = 5;
        }
        if (_buildingPosition.arraySize < 5)
        {
            _buildingPosition.arraySize = 5;
        }
        if (_wheels.arraySize < 2)
        {
            _wheels.arraySize = 2;
        }
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _showSprites.target = BeginFoldoutHeaderGroup(_showSprites.target, "Sprites");
        if (BeginFadeGroup(_showSprites.faded))
        {
            BeginVertical("box");
                for (int i = 0; i < _sprites.arraySize; i++)
                {
                    ObjectField(_sprites.GetArrayElementAtIndex(i), typeof(Sprite), new GUIContent($"{i+1} level sprite", $"Sprite what will load at {i+1} level of locomotive"));
                }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showBuildings.target = BeginFoldoutHeaderGroup(_showBuildings.target, "Buildings");
        if (BeginFadeGroup(_showBuildings.faded))
        {
            BeginVertical("box");
                for (int i = 0; i < _buildingPosition.arraySize; i++)
                {
                    BeginHorizontal();
                        Vector3 _positionBuffer = _buildingPosition.GetArrayElementAtIndex(i).vector3Value;
                        _buildingPosition.GetArrayElementAtIndex(i).vector3Value = Vector2Field(new GUIContent($"{i + 1} building position", $"Value what specify position of the center of {i + 1} building object"), new Vector2(_positionBuffer.x, _positionBuffer.y));
                    EndHorizontal();
                }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        _showWheels.target = BeginFoldoutHeaderGroup(_showWheels.target, "Wheels");
        if (BeginFadeGroup(_showWheels.faded))
        {
            BeginVertical("box");
                for (int i = 0; i < _wheels.arraySize; i++)
                {
                    ObjectField(_wheels.GetArrayElementAtIndex(i), typeof(TrainWheels), new GUIContent($"{i + 1} wheel object", $"TrainWheels object for this locomotive instance"));
                }
            EndVertical();
        }
        EndFadeGroup();
        EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (_showBuildings.target)
        {
            for (int i = 0; i < _buildingPosition.arraySize; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 _newPosition = Handles.PositionHandle(_buildingPosition.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.Update();
                    Undo.RecordObject((LocomotiveAgent)target, "Change Look At Target Position");
                    _buildingPosition.GetArrayElementAtIndex(i).vector3Value = _newPosition;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
