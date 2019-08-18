using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LocomotiveAgent : TrainAgent
{
    private int _id;
    private int _level;
    public Sprite[] sprites;
    public Vector3[] buildingsPositions;
    public LocomotiveForeground foreground;
    private TrainBuilding[] _currentBuildings;

    /// <summary>
    /// Initialize data of locomotive agent
    /// </summary>
    /// <param name="info">Locomotive data from account</param>
    public void LoadInstance(Locomotive info)
    {
        _id = info.id;
        _level = info.level;
        _sr.sprite = sprites[_level - 1];
        if(foreground != null)
        {
            foreground.LoadInstance(info.level);
        }
        _currentBuildings = new TrainBuilding[info.outer.Length];
        for (int i = 0; i < info.outer.Length; i++)
        {
            TrainBuilding instance = (Instantiate(Resources.Load("Locomotive/Buildings/Outer/" + info.outer[i].name), buildingsPositions[i], Quaternion.identity) as GameObject).GetComponent<TrainBuilding>();
            instance.LoadInstance(info.outer[i]);
            _currentBuildings[i] = instance;
        }
    }
}
