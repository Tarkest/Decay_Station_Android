using UnityEngine;
using UnityEngine.UI;

public class TrainInventoryAgent : MonoBehaviour
{

    string _json = "{\"locomotives\":[{\"id\":1,\"items\":[{\"id\":1,\"name\":\"\",\"amount\":0,\"cellId\":0},{\"id\":2,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":1},{\"id\":3,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":2},{\"id\":4,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":3},{\"id\":5,\"name\":\"\",\"amount\":0,\"cellId\":4},{\"id\":6,\"name\":\"\",\"amount\":0,\"cellId\":5},{\"id\":7,\"name\":\"\",\"amount\":0,\"cellId\":6},{\"id\":8,\"name\":\"\",\"amount\":0,\"cellId\":7},{\"id\":9,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":8},{\"id\":10,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":9},{\"id\":11,\"name\":\"\",\"amount\":0,\"cellId\":10}]}],\"carriages\":[{\"id\":2,\"items\":[{\"id\":12,\"name\":\"kartoshka\",\"amount\":20,\"cellId\":0},{\"id\":13,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":2},{\"id\":14,\"name\":\"\",\"amount\":0,\"cellId\":3},{\"id\":15,\"name\":\"\",\"amount\":0,\"cellId\":4},{\"id\":16,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":5}]}],\"characters\":[{\"id\":3,\"items\":[{\"id\":17,\"name\":\"kartoshka\",\"amount\":20,\"cellId\":0},{\"id\":18,\"name\":\"\",\"amount\":\"\",\"cellId\":1},{\"id\":19,\"name\":\"kartoshka\",\"amount\":10,\"cellId\":2}]}]}";

    NInventory _locomotiveInventory;
    NInventory[] _characterInventories;
    NInventory[] _carriageInventories;

    public static int choosenItem = -1;

    public GameObject npcContent;
    public GameObject trainContent;
    public GameObject inventory;
    public GameObject item;

    private void Awake()
    {
        //HttpController.instance.GET("/api/inventories", GetCallback);
        GetCallback(_json);
    }

    /// <summary>
    /// Callback for GET all inventories request
    /// </summary>
    /// <param name="msg">NPCs's, Locomotive's and Carriages's inventories</param>
    private void GetCallback(string msg)
    {
        Inventories o = JsonUtility.FromJson<Inventories>(msg);
        _locomotiveInventory = o.locomotives[0];
        _characterInventories = o.characters;
        _carriageInventories = o.carriages;
        CreateView();
    }

    /// <summary>
    /// Test creating inventory visualization
    /// </summary>
    private void CreateView()
    {
        GameObject i;

        foreach (var instance in _characterInventories)
        {
            i = Instantiate(inventory, npcContent.transform);
            foreach(var it in instance.items)
            {
                var tmp = Instantiate(item, i.transform);
                tmp.GetComponent<InventoryItem>().id = it.id;
                tmp.GetComponentInChildren<Text>().text = it.name + " - " + it.amount;
            }
        }

        i = Instantiate(inventory, trainContent.transform);

        foreach (var it in _locomotiveInventory.items)
        {
            var tmp = Instantiate(item, i.transform);
            tmp.GetComponent<InventoryItem>().id = it.id;
            tmp.GetComponentInChildren<Text>().text = it.name + " - " + it.amount;
        }

        foreach (var instance in _carriageInventories)
        {
            i = Instantiate(inventory, trainContent.transform);
            foreach (var it in instance.items)
            {
                var tmp = Instantiate(item, i.transform);
                tmp.GetComponent<InventoryItem>().id = it.id;
                tmp.GetComponentInChildren<Text>().text = it.name + " - " + it.amount;
            }
        }
    }

    /// <summary>
    /// Send request for switch items
    /// </summary>
    /// <param name="id"></param>
    public static void SwitchItems(int id)
    {
        // There will be request and maybe callback
        Debug.Log(choosenItem + " " + id);
        choosenItem = -1;
    }


    /// <summary>
    /// Callback for red close button
    /// </summary>
    public void OnClickClose()
    {
        Destroy(gameObject);
    }

}
