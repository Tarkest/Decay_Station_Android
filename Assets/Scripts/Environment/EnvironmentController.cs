using UnityEngine;
using System;

public class EnvironmentController : MonoBehaviour
{
    public EnvironmentData currentEnvironment;
    public EnvironmentData nextEnvironment;

    public float speed;

    public GameObject[][] _positions = new GameObject[20][];

    //public void Initialize(EnvironmentData environment)
    private void Start()
    {
        //currentEnvironment = environment;
        for (int i = 0; i < 20; i++)
        {
            _positions[i] = new GameObject[5];
        }
        for (int i = 0; i < 10; i++)
        {
            if(currentEnvironment.environmentSprites[i].GetSprite())
            {
                for (int posI = -1; posI < 2; posI++)
                {
                    _positions[i][posI + 1] = InstantiateLayer(i, currentEnvironment.environmentSprites[i].GetSprite(), posI);
                }
            }
            else
            {
                continue;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < 20; i++)
        {
            foreach(GameObject _layer in _positions[i])
            {
                if(_layer)
                    _layer.transform.Translate(new Vector3(speed * (_layer.GetComponent<SpriteRenderer>().sortingOrder * 0.1f * Time.deltaTime), 0, 0));
            }
        }
    }

    private GameObject InstantiateLayer(int layerIndex, Sprite layerSprite, int positionIndex)
    {
        GameObject _layer = new GameObject($"{ layerIndex + 1 } Layer", typeof(SpriteRenderer));
        _layer.transform.parent = gameObject.transform;
        SpriteRenderer _sr = _layer.GetComponent<SpriteRenderer>();
        _sr.sprite = layerSprite;
        _layer.transform.position = new Vector3(_sr.bounds.size.x * positionIndex, 0);
        _sr.sortingLayerName = layerIndex +"";
        _sr.sortingOrder = layerIndex;
        return _layer;
    }
}
