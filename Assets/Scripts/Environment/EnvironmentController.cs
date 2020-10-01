using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [System.NonSerialized]
    public float currentSpeed;
    private EnvironmentData _currentEnvironment;
    private EnvironmentData _nextEnvironment;
    private int _environmentLenght;
    private List<GameObject> _layers = new List<GameObject>();
    private EnvironmentSplice _splice;
    private float _trainLenght;

    /// <summary>
    /// Load environment from Assets pack
    /// </summary>
    /// <param name="environmentName">Name of environment data object in Resources</param>
    /// <param name="trainLenght">Lenght of a train in units</param>
    public void LoadEnvironment(string environmentName, float trainLenght)
    {
        _currentEnvironment = Resources.Load<EnvironmentData>("Environments/Train/" + environmentName);
        _trainLenght = trainLenght;

        InstantiateLayers();
    }

    /// <summary>
    /// Load environment from Assets pack and invoke splice showing for user
    /// </summary>
    /// <param name="environmentName">Name of environment data object in Resources</param>
    public void ChangeEnvironment(string environmentName)
    {
        _nextEnvironment = Resources.Load<EnvironmentData>("Environments/Train/" + environmentName);
        InstantiateSplice();
    }

    /// <summary>
    /// Call this method for changing speed of environment move
    /// </summary>
    /// <param name="value">Speed change amount</param>
    public void ChangeSpeed(float value)
    {
        currentSpeed += value;
    }

    /// <summary>
    /// Callback for applying new environment and end splice showing
    /// </summary>
    private void ReloadEnvironment()
    {
        ClearLayers();
        _currentEnvironment = _nextEnvironment;
        InstantiateLayers();
        _splice.EndSplice();
    }

    /// <summary>
    /// Method for instantiating environment layers
    /// </summary>
    private void InstantiateLayers()
    {
        _environmentLenght = (int)Mathf.Ceil(_trainLenght / _currentEnvironment.size) + 2;

        LoadLayers(0, 10);

        if (_currentEnvironment.railsBackground.notEmpty)
            InstantiateLayer(_currentEnvironment.railsBackground, 10);

        if (_currentEnvironment.rails.notEmpty)
            InstantiateLayer(_currentEnvironment.rails, 15);

        if (_currentEnvironment.railsForeground.notEmpty)
            InstantiateLayer(_currentEnvironment.railsForeground, 19);

        LoadLayers(20, 30);
    }

    /// <summary>
    /// Intermediate method for instantiating background and foreground layers
    /// </summary>
    /// <param name="from">Layer index what depends on its render order from what instantiating will start</param>
    /// <param name="to">Layer index what depends on its render order on what instantiating will end</param>
    private void LoadLayers(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            int _layerIndex = from > 0 ? i - 10 : i;
            if (_currentEnvironment.environmentSprites[_layerIndex].notEmpty)
            {
                InstantiateLayer(_currentEnvironment.environmentSprites[_layerIndex], i);
            }
        }
    }

    /// <summary>
    /// Method for destroing whole environment except splice
    /// </summary>
    private void ClearLayers()
    {
        foreach(GameObject lay in _layers)
        {
            Destroy(lay);
        }
        _layers.Clear();
    }

    /// <summary>
    /// Method for initializing layer GameObject
    /// </summary>
    /// <param name="layerData">Data object from environment data</param>
    /// <param name="layerIndex">Layer index what depends on its render order</param>
    private void InstantiateLayer(EnvironmentLayerData layerData, int layerIndex)
    {
        GameObject _buffer = new GameObject(layerIndex + " layer");
        _buffer.transform.position = Vector3.zero;
        _buffer.transform.parent = transform;
        EnvironmentLayer _layerBuffer = _buffer.AddComponent<EnvironmentLayer>();
        _layerBuffer.LoadLayer(layerData, layerIndex, _environmentLenght, this);
        _layers.Add(_buffer);
    }

    /// <summary>
    /// Method for initializing splice GameObject
    /// </summary>
    private void InstantiateSplice()
    {
        GameObject _buffer = new GameObject("Splice layer");
        _buffer.transform.position = Vector3.zero;
        _buffer.transform.parent = transform;
        EnvironmentSplice _layerBuffer = _buffer.AddComponent<EnvironmentSplice>();
        _layerBuffer.BeginSplice(_currentEnvironment.environmentSplice, this, ReloadEnvironment);
        _splice = _layerBuffer;
    }
}
