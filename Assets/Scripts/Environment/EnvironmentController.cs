using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public float currentSpeed;
    public EnvironmentData currentEnvironment;
    public EnvironmentData nextEnvironment;
    private int _environmentLenght;

    private void Start()
    {
        LoadEnvironment(currentEnvironment.name, 77);
    }

    public void LoadEnvironment(string environmentName, float trainLenght)
    {
        currentEnvironment = Resources.Load<EnvironmentData>("Environments/Train/" + environmentName);
        _environmentLenght = (int)Mathf.Ceil(trainLenght / currentEnvironment.size) + 2;

        LoadLayers(0, 10);

        if(currentEnvironment.railsBackground.notEmpty)
            InstantiateLayer(currentEnvironment.railsBackground, 10);

        if (currentEnvironment.rails.notEmpty)
            InstantiateLayer(currentEnvironment.rails, 15);

        if (currentEnvironment.railsForeground.notEmpty)
            InstantiateLayer(currentEnvironment.railsForeground, 19);

        LoadLayers(20, 30);
    }

    public void ChangeEnvironment()
    {

    }

    public void ChangeSpeed(float value)
    {
        currentSpeed += value;
    }

    private void LoadLayers(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            int _layerIndex = from > 0 ? i - 10 : i;
            if (currentEnvironment.environmentSprites[_layerIndex].notEmpty)
            {
                InstantiateLayer(currentEnvironment.environmentSprites[_layerIndex], _layerIndex);
            }
        }
    }

    private void InstantiateLayer(EnvironmentLayerData layerData, int layerIndex)
    {
        GameObject _buffer = new GameObject(layerIndex + " layer");
        _buffer.transform.position = Vector3.zero;
        _buffer.transform.parent = transform;
        EnvironmentLayer _layerBuffer = _buffer.AddComponent<EnvironmentLayer>();
        _layerBuffer.LoadLayer(layerData, layerIndex, _environmentLenght, this);
    }
}
