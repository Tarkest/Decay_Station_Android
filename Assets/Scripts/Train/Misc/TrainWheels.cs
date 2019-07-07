using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWheels : MonoBehaviour
{
    private Animator _anim;

    IEnumerator BeginMoving()
    {
        _anim.SetInteger("animState", 1);
        for (float i = 0; i < 1; i+=0.1f)
        {
            _anim.SetFloat("WhellsSpeed", i);
            yield return null;
        }
    }

    IEnumerator StopMoving()
    {
        for (float i = 1; i>0; i-=0.1f)
        {
            _anim.SetFloat("WhellsSpeed", i);
            yield return null;
        }
        _anim.SetInteger("animState", 0);
    }
}
