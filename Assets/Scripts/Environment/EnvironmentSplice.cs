using System;
using UnityEngine;

public class EnvironmentSplice : MonoBehaviour
{
    private EnvironmentSpliceLayer _spliceData;
    private EnvironmentController _parent;
    private float _layerLenght, _parallaxEffect = 30f;
    private bool _ending, _ended;
    private Action _reloadCallback;

    /// <summary>
    /// Method for instantiating and load splice GameObject
    /// </summary>
    /// <param name="spliceData">Data object from environment data</param>
    /// <param name="parent">Environment contoller object</param>
    /// <param name="callback">Method what will be invoked when splice is ended</param>
    public void BeginSplice(EnvironmentSpliceLayer spliceData, EnvironmentController parent, Action callback)
    {
        _spliceData = spliceData;
        _layerLenght = spliceData.size;
        _parent = parent;
        _reloadCallback = callback;
        for(int i = 1; i < 4; i++)
        {
            InstantiateSpliceInstance(_spliceData, i, i == 1 ? 0 : 1, _layerLenght);
        }
        _ended = false;
        _ending = false;
    }

    /// <summary>
    /// Method what will start splice destroying sequence
    /// </summary>
    public void EndSplice()
    {
        _ending = true;
    }

    private void FixedUpdate()
    {
        float _distance = 0;

        if (_parent.currentSpeed > 0)
        {
            _distance = -_parent.currentSpeed * _parallaxEffect * Time.fixedDeltaTime;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform _childTransform = transform.GetChild(i);
            _childTransform.position = new Vector3(_childTransform.position.x + _distance, transform.position.y, transform.position.z);
        }

        Transform _lastChild = transform.GetChild(transform.childCount - 1);
        if(_lastChild.position.x + _distance <= Camera.main.transform.position.x - _layerLenght)
        {
            if (_ending)
            {
                if(_ended)
                {
                    Destroy(_lastChild.gameObject);
                    if (transform.childCount <= 1) 
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    _lastChild.position = new Vector3(transform.GetChild(0).position.x + _layerLenght, transform.position.y, transform.position.z);
                    _lastChild.gameObject.GetComponent<SpriteRenderer>().sprite = _spliceData.sprites[2];
                    _ended = true;
                }
            }
            else
            {
                _lastChild.position = new Vector3(transform.GetChild(0).position.x + _layerLenght, transform.position.y, transform.position.z);
                _lastChild.SetSiblingIndex(0);
                _lastChild.gameObject.GetComponent<SpriteRenderer>().sprite = _spliceData.sprites[1];
                _reloadCallback();
            }
        }
    }

    /// <summary>
    /// Method what instantiate and initialize graphic GameObject instance on splice layer
    /// </summary>
    /// <param name="data">Data object from environment data</param>
    /// <param name="instanceIndex">Index of instance from left to right</param>
    /// <param name="instanceType">Index of sprite for this instance (0 - start, 1 - middle, 2 - end)</param>
    /// <param name="layerLenght">Layer sprite size</param>
    private void InstantiateSpliceInstance(EnvironmentSpliceLayer data, int instanceIndex, int instanceType, float layerLenght)
    {
        GameObject _buffer = new GameObject(instanceIndex + " splice instance");
        _buffer.transform.position = new Vector3(Camera.main.transform.position.x + (layerLenght * instanceIndex), transform.position.y, transform.position.z);
        _buffer.transform.parent = transform;
        _buffer.transform.SetSiblingIndex(0);
        SpriteRenderer _spriteRenderer = _buffer.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = data.sprites[instanceType];
        _spriteRenderer.sortingLayerName = "30";
        _spriteRenderer.sortingOrder = 30;
    }
}
