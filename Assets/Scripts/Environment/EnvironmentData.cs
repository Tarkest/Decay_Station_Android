using UnityEngine;

[CreateAssetMenu(fileName = "New Environment", menuName = "Environment/Environment Data")]
public class EnvironmentData : ScriptableObject
{
    public EnvironmentLayerData[] environmentSprites = new EnvironmentLayerData[20];

    public EnvironmentLayerData rails;

    public EnvironmentLayerData railsForeground;

    public EnvironmentLayerData railsBackground;

    public EnvironmentSpliceLayer environmentSplice;

    public float size
    {
        get
        {
            foreach (EnvironmentLayerData i in environmentSprites)
            {
                if (i.size != 0)
                {
                    return i.size;
                }
                else
                {
                    continue;
                }
            }
            return 0;
        }
    }

    public EnvironmentData()
    {
        rails = new EnvironmentLayerData();
        railsForeground = new EnvironmentLayerData();
        railsBackground = new EnvironmentLayerData();
        environmentSplice = new EnvironmentSpliceLayer();
        for (int i = 0; i < environmentSprites.Length; i++)
        {
            environmentSprites[i] = new EnvironmentLayerData();
        }
    }
}

[System.Serializable]
public class EnvironmentLayerData
{
    public Sprite[] variation = new Sprite[0];

    public float size {

        get
        {
            try
            {
                return variation[0].bounds.size.x;
            }
            catch
            {
                return 0;
            }
        }
    }

    public bool isMovingWhenStatic;

    public float staticMoveSpeed;

    public bool notEmpty
    {
        get
        {
            try
            {
                return variation[0] != null;
            }
            catch
            {
                return false;
            }
        }
    }

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

[System.Serializable]
public class EnvironmentSpliceLayer
{
    public Sprite[] sprites = new Sprite[3];

    public float size
    {
        get
        {       
            try
            {
                return sprites[0].bounds.size.x;
            }
            catch
            {
                return 0;
            }
        }
    }
}
