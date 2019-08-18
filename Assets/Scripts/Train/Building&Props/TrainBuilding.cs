using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TrainBuilding : TrainAgentTarget
{
    private int _id;

    public void LoadInstance(Building info)
    {
        _id = info.id;
    }
}
