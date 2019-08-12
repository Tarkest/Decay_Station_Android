using UnityEngine;

[CreateAssetMenu(fileName = "New Environment", menuName = "Environment/Environment Data")]
public class EnvironmentData : ScriptableObject
{
    public EnvironmentLayer[] environmentSprites = new EnvironmentLayer[20];

    public EnvironmentLayer rails;

    public EnvironmentLayer railsForeground;

    public EnvironmentLayer railsBackground;

    public EnvironmentData()
    {
        rails = new EnvironmentLayer();
        railsForeground = new EnvironmentLayer();
        railsBackground = new EnvironmentLayer();
        for (int i = 0; i<environmentSprites.Length; i++)
        {
            environmentSprites[i] = new EnvironmentLayer();
        }
    }
}

[System.Serializable]
public class EnvironmentLayer
{
    public Sprite[] variation = new Sprite[0];

    public Sprite GetSprite()
    {
        int _random = Random.Range(0, variation.Length);
        try
        {
            return variation[_random];
        }
        catch
        {
            return null;
        }
    }
}
