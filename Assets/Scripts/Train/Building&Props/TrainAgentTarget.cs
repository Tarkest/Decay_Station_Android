using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TrainAgentTarget : MonoBehaviour
{
    public bool occupied { private set; get; }
    public TrainNPCAgent.Animation animationName;
    private SpriteRenderer rend;
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    private void OnEnable()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void Engage()
    {
        occupied = true;
        rend.enabled = false;
    }

    public void Release()
    {
        occupied = false;
        rend.enabled = true;
    }
}
