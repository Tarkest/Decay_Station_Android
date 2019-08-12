using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class TrainWheels : MonoBehaviour
{
    private Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    IEnumerator BeginMoving()
    {
        _anim.SetTrigger("isMoving");
        for (float i = 0; i < 13f; i+=0.003f)
        {
            _anim.SetFloat("whellsSpeed", i);
            yield return null;
        }
    }

    IEnumerator StopMoving()
    {
        for (float i = 13f; i>0; i-=0.003f)
        {
            _anim.SetFloat("whellsSpeed", i);
            yield return null;
        }
        _anim.SetTrigger("isIdle");
    }
}
