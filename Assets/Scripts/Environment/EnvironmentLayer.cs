using UnityEngine;

public class EnvironmentLayer : MonoBehaviour
{
    private float _layerLenght, _parallaxEffect;
    private int _layerIndex, _size;
    private EnvironmentLayerData _layerData;
    private EnvironmentController _parent;

    public void LoadLayer(EnvironmentLayerData layerData, int layerIndex, int size, EnvironmentController parent)
    {
        _parent = parent;
        _layerData = layerData;
        _layerLenght = layerData.size;
        _parallaxEffect = layerIndex;
        _layerIndex = layerIndex;
        _size = size - 1;

        for (int i = -1; i < _size; i++)
        {
            InstantiateLayerInstance(_layerData, i, _layerIndex, _layerLenght);
        }
    }

    void FixedUpdate()
    {
        float _distance = 0;

        if (_parent.currentSpeed > 0)
        {
            _distance = (-_parent.currentSpeed - _layerData.staticMoveSpeed) * _parallaxEffect * Time.fixedDeltaTime;
        }
        else if (_layerData.isMovingWhenStatic)
        {
            _distance = -_layerData.staticMoveSpeed * _parallaxEffect * Time.fixedDeltaTime;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform _childTransform = transform.GetChild(i);
            _childTransform.position = new Vector3(_childTransform.position.x + _distance, transform.position.y, transform.position.z);
        }

        Transform _lastChild = transform.GetChild(transform.childCount - 1);
        if (_lastChild.position.x + _distance <= -_size * _layerLenght)
        {
            _lastChild.position = new Vector3(transform.GetChild(0).position.x + _layerLenght, transform.position.y, transform.position.z);
            _lastChild.SetSiblingIndex(0);
            _lastChild.gameObject.GetComponent<SpriteRenderer>().sprite = _layerData.GetSprite();
        }
    }

    private void InstantiateLayerInstance(EnvironmentLayerData data, int instanceIndex, int layerIndex, float layerLenght)
    {
        GameObject _buffer = new GameObject(instanceIndex + 1 + " instance");
        _buffer.transform.position = new Vector3(-layerLenght * instanceIndex, transform.position.y, transform.position.z);
        _buffer.transform.parent = transform;
        _buffer.transform.SetSiblingIndex(instanceIndex + 1);
        SpriteRenderer _spriteRenderer = _buffer.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = data.GetSprite();
        _spriteRenderer.sortingLayerName = layerIndex.ToString();
        _spriteRenderer.sortingOrder = layerIndex;
    }
}
