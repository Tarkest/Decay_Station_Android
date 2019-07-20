using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class CarriageAgent : TrainAgent
{
    private int _id;
    public Vector3[] buildingsPositions;
    private TrainBuilding[] _currentInnerBuildings;
    private TrainBuilding[] _currentOuterBuildings;

    /// <summary>
    /// Initialize data of carriage agent
    /// </summary>
    /// <param name="info">Carriage data from account</param>
    public void LoadInstance(Carriage info)
    {
        _id = info.id;
        _currentInnerBuildings = new TrainBuilding[info.inner.Length];
        for (int i = 0; i < info.inner.Length; i++)
        {
            TrainBuilding instance = (Instantiate(Resources.Load("Carriages/Buildings/Outer" + info.outer[i].name), buildingsPositions[i], Quaternion.identity) as GameObject).GetComponent<TrainBuilding>();
            instance.LoadInstance(info.outer[i]);
            _currentInnerBuildings[i] = instance;

        }
        _currentOuterBuildings = new TrainBuilding[info.outer.Length];
        for (int i = 0; i < info.outer.Length; i++)
        {
            TrainBuilding instance = (Instantiate(Resources.Load("Carriages/Buildings/Outer" + info.outer[i].name), buildingsPositions[i], Quaternion.identity) as GameObject).GetComponent<TrainBuilding>();
            instance.LoadInstance(info.outer[i]);
            _currentOuterBuildings[i] = instance;

        }
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(CarriageAgent))]
//public class CarrigeAgentInspector
//{

//}
//#endif
