using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(EdgeCollider2D))]
public class LocomotiveAgent : TrainAgent
{
    public int id;
    private int _level;
    public Sprite[] sprites;
    public Vector3[] buildingsPositions;
    public LocomotiveForeground foreground;

    public void ApplyDataFromEditor(int maxLevel)
    {
        Array.Resize(ref buildingsPositions, maxLevel);
        Array.Resize(ref sprites, maxLevel - 1);
    }

    /// <summary>
    /// Initialize data of locomotive agent
    /// </summary>
    /// <param name="info">Locomotive data from account</param>
    public void LoadInstance(Locomotive info)
    {
        id = info.id;
        _level = info.level;
        _sr.sprite = sprites[_level - 1];
        if(foreground != null)
        {
            foreground.LoadInstance(info.level);
        }
    }

    /// <summary>
    /// Destroy all train buildings
    /// </summary>
    public void ResetBuilding()
    {

    }
}
