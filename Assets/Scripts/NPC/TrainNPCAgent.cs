using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class TrainNPCAgent : MonoBehaviour
{
    public TrainAgentTarget currentTarget;
    public float timeForAction;
    public float moveSpeed;
    public float minGroundNormalY = 0.65f;
    private Vector2 _move;
    private SpriteRenderer _sr;
    private Animator _anim;
    private Vector2 _targetVelocity;
    private Vector2 _groundNormal;
    private Vector2 _velocity;
    private Rigidbody2D _rb2d;
    private const float minMoveDistance = 0.001f;
    private const float shellRadius = 0.01f;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>();
    private Animation _currentState = Animation.Idle;
    public enum Animation
    {
        Idle,
        Walk,
        Run,
        Workbench,
        Oven,
        Smelter,
        MedicalLab,
        ScienceLab,
        Armory,
        FloorClean
    }

    void OnEnable()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter.useLayerMask = true;
    }

    void Update()
    {
        if(currentTarget != null)
        {
            if (timeForAction != -1)
                timeForAction -= Time.deltaTime;
            if(timeForAction > -1 && timeForAction <= 0)
            {
                currentTarget.Release();
                currentTarget = null;
                _targetVelocity = Vector2.zero;
                return;
            }
            if(transform.position.x <= (currentTarget.position.x + 0.05f) && transform.position.x >= (currentTarget.position.x - 0.05f))
            {
                if (_currentState == Animation.Walk || _currentState == Animation.Run)
                {
                    _move = Vector2.zero;
                    ComputeVelosity();
                    transform.position = currentTarget.position;
                    _currentState = currentTarget.animationName;
                    _anim.SetInteger("AnimState", (int)_currentState);
                    Debug.Log(_currentState);
                }
            }
            else
            {
                if (_currentState == Animation.Idle)
                {
                    if (Mathf.Abs(currentTarget.position.x - transform.position.x) > 2f)
                    {
                        moveSpeed = 2f;
                        _currentState = Animation.Run;
                        _anim.SetInteger("AnimState", (int)_currentState);
                        Debug.Log(_currentState);
                    }
                    else
                    {
                        moveSpeed = 1f;
                        _currentState = Animation.Walk;
                        _anim.SetInteger("AnimState", (int)_currentState);
                        Debug.Log(_currentState);
                    }
                    _anim.SetFloat("MoveSpeed", moveSpeed);
                    _targetVelocity = Vector2.zero;
                    _move.x = currentTarget.position.x > transform.position.x ? 1 : -1;
                    ComputeVelosity();
                }
            }
        }
        else
        {
            if(_currentState != Animation.Idle)
            {
                _currentState = Animation.Idle;
                _anim.SetInteger("AnimState", (int)_currentState);
                Debug.Log(_currentState);
            }
            _move = Vector2.zero;
        }
    }

    private void ComputeVelosity()
    {
        bool flipSprite = (_sr.flipX ? (_move.x > 0.01f) : (_move.x < -0.01f));
        if (flipSprite)
        {
            _sr.flipX = !_sr.flipX;
        }
        _targetVelocity = _move * moveSpeed;
    }

    void FixedUpdate()
    {
        _velocity += Physics2D.gravity * Time.deltaTime;
        _velocity.x = _targetVelocity.x;
        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    void Movement(Vector2 move, bool YMovement)
    {
        float distance = move.magnitude;
        if (distance > minMoveDistance)
        {
            int count = _rb2d.Cast(move, _contactFilter, _hitBuffer, distance + shellRadius);
            _hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }
            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    if (YMovement)
                    {
                        _groundNormal = currentNormal;
                        _groundNormal.x = 0;
                    }
                }
                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }
                float modifiedDistance = _hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        _rb2d.position = _rb2d.position + move.normalized * distance;
    }
}
