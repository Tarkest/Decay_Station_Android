using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LocomotiveAgent : MonoBehaviour
{
    private int _id;
    private int _level;
    public Sprite[] sprites;
    public Vector3[] buildingsPositions;
    public TrainWheels[] wheels = new TrainWheels[2];
    private GameObject[] _currentBuildings;

    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void LoadInstance(Locomotive info)
    {
        _id = info.id;
        _level = info.level;
        _sr.sprite = sprites[_level - 1];
    }

    public void BeginMoving()
    {
        foreach  (TrainWheels i in wheels)
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
