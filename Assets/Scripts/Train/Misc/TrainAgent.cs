using UnityEngine;

public abstract class TrainAgent : MonoBehaviour
{
    public TrainWheels[] wheels;
    [HideInInspector]
    public SpriteRenderer _sr;
    /// <summary>
    /// Return sprite of train agent
    /// </summary>
    [HideInInspector]
    public Sprite sprite
    {
        get
        {
            return _sr.sprite;
        }
    }

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Invoking wheels starting animation
    /// </summary>
    public void BeginMoving()
    {
        foreach (TrainWheels i in wheels)
        {
            i.StartCoroutine("BeginMoving");
        }
    }

    /// <summary>
    /// Method what invoking wheels stop animation
    /// </summary>
    public void StopMoving()
    {
        foreach (TrainWheels i in wheels)
        {
            i.StartCoroutine("StopMoving");
        }
    }
}
