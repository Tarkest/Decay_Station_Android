using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Worktable : TrainAgentTarget
{
    public int id;

    public void LoadInstance(Building info)
    {
        id = info.id;
    }
}
