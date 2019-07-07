using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class CarriageAgent : MonoBehaviour
{
    private int _id;
    public Vector3[] buildingsPositions;
    public TrainWheels[] wheels = new TrainWheels[2];
    private GameObject[] _currentBuildings;

    public void LoadInstance(Locomotive info)
    {
        _id = info.id;
    }

    public void BeginMoving()
    {
        foreach (TrainWheels i in wheels)
        {
            i.StartCoroutine("BeginMoving");
        }
    }

    public void StopMoving()
    {
        foreach (TrainWheels i in wheels)
        {
            i.StartCoroutine("StopMoving");
        }
    }
}
