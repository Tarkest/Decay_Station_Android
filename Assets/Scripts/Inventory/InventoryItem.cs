using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public int Id { get; private set; }
    public int Amount { get; private set; }
    public Item ItemInstance { get; private set; }
    public Text AmountText;
    public Image IconImage;

    public void Init(int Id, string Name, int Amount)
    {
        this.Id = Id;
        this.Amount = Amount;
        AmountText.text = $"{Amount}";
        ItemInstance = ItemSystem.GetItem(Name);
        IconImage.sprite = ItemInstance ? ItemInstance.Icon : null; // Instead of null there'll be empty slot prefab
    }

    public void onClick()
    {
        if(TrainInventoryAgent.choosenItem == -1)
        {
            TrainInventoryAgent.choosenItem = Id;
        }
        else
        {
            TrainInventoryAgent.SwitchItems(Id);
        }
    }
}
