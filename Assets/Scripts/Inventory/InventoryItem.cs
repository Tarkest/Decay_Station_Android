using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public int id;

    public void onClick()
    {
        if(TrainInventoryAgent.choosenItem == -1)
        {
            TrainInventoryAgent.choosenItem = id;
        }
        else
        {
            TrainInventoryAgent.SwitchItems(id);
        }
    }
}
