using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public float currentSpeed;
    public EnvironmentData currentEnvironment;
    public EnvironmentData nextEnvironment;
    private int _environmentLenght;
    private List<GameObject> _layers = new List<GameObject>();
    private EnvironmentSplice _splice;
    private float _trainLenght;

    private void Start()
    {
        LoadEnvironment(currentEnvironment.name, 77);
    }

    public void LoadEnvironment(string environmentName, float trainLenght)
    {
        currentEnvironment = Resources.Load<EnvironmentData>("Environments/Train/" + environmentName);
        _trainLenght = trainLenght;

        InstantiateLayers();
    }

    public void ChangeEnvironment(string environmentName)
    {
        nextEnvironment = Resources.Load<EnvironmentData>("Environments/Train/" + environmentName);
        InstantiateSplice();
    }

    public void ReloadEnvironment()
    {
        ClearLayers();
        currentEnvironment = nextEnvironment;
        InstantiateLayers();
        _splice.EndSplice();
    }

    public void ChangeSpeed(float value)
    {
        currentSpeed += value;
    }

    public void ChangeEnvironment()
    {
        ChangeEnvironment(nextEnvironment.name);
    }

    private void InstantiateLayers()
    {
        _environmentLenght = (int)Mathf.Ceil(_trainLenght / currentEnvironment.size) + 2;

        LoadLayers(0, 10);

        if (currentEnvironment.railsBackground.notEmpty)
            InstantiateLayer(currentEnvironment.railsBackground, 10);

        if (currentEnvironment.rails.notEmpty)
            InstantiateLayer(currentEnvironment.rails, 15);

        if (currentEnvironment.railsForeground.notEmpty)
            InstantiateLayer(currentEnvironment.railsForeground, 19);

        LoadLayers(20, 30);
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

    private void ClearLayers()
    {
        foreach(GameObject lay in _layers)
        {
            Destroy(lay);
        }
        _layers.Clear();
    }

    private void InstantiateLayer(EnvironmentLayerData layerData, int layerIndex)
    {
        GameObject _buffer = new GameObject(layerIndex + " layer");
        _buffer.transform.position = Vector3.zero;
        _buffer.transform.parent = transform;
        EnvironmentLayer _layerBuffer = _buffer.AddComponent<EnvironmentLayer>();
        _layerBuffer.LoadLayer(layerData, layerIndex, _environmentLenght, this);
        _layers.Add(_buffer);
    }

    private void InstantiateSplice()
    {
        GameObject _buffer = new GameObject("Splice layer");
        _buffer.transform.position = Vector3.zero;
        _buffer.transform.parent = transform;
        EnvironmentSplice _layerBuffer = _buffer.AddComponent<EnvironmentSplice>();
        _layerBuffer.BeginSplice(currentEnvironment.environmentSplice, this);
        _splice = _layerBuffer;
    }
}
