using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class RootTentacle : MonoBehaviour
{
    public static float MaxLength = 15f;

    private enum AttachType
    {
        Instant,
        Animated
    }

    [Header("Root Movement")]
    [SerializeField] AttachType _attachType = AttachType.Instant;

    Transform _target;
    LineRenderer _lr;

    [SerializeField] float _rootMoveSpeed = 1f;
    [SerializeField] float _targetErrorMargin = 0.05f;
    Vector3 _targetPosition; // animated only
    private float _attachStartTime;
    

    [Space]
    [Header("Breaking Config")]
    [SerializeField] float _breakAngleDot = 0;
    [SerializeField] float _maxRootLength = 20f;
    [SerializeField] LayerMask _layerMaskWalls;
    Camera _cam;

    [Space]
    [Header("Pull Parent")]
    [SerializeField] bool _pullParent;
    [SerializeField] float _pullForce = 5f;
    Rigidbody2D _parentRB;

    
    

    public delegate void RootEvent(RootTentacle root);
    public event RootEvent OnRootBreak;

    private void Awake()
    {
        _target = new GameObject("Root Target").transform;

        MaxLength = _maxRootLength;

        _lr = GetComponent<LineRenderer>();
        UpdatePositions();
    }

    private void Start()
    {
        _cam = Camera.main;
        _parentRB = transform.parent.GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Vector2 targetPosition)
    {
        if (_attachType == AttachType.Instant)
            SetTargetInstant(targetPosition);
        else if (_attachType == AttachType.Animated)
            SetTargetMoving(targetPosition);
    }

    private void SetTargetMoving(Vector2 targetPosition)
    {
        _target.position = transform.position;
        _targetPosition = targetPosition;
        _attachStartTime = Time.time;
    }

    private void SetTargetInstant(Vector2 targetPosition) => _target.position = targetPosition;


    private void UpdatePositions()
    {
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, _target.position);
    }

    private void UpdateTargetPosition()
    {
        if (_attachType == AttachType.Animated)
        {
            _target.position = Vector2.Lerp(transform.position, _targetPosition, (Time.time - _attachStartTime) * _rootMoveSpeed);
        }
    }

    private void Update()
    {
        if (!IsValidRoot())
        {
            Break();
            return;
        }

        UpdateTargetPosition();
        UpdatePositions();
    }

    private void FixedUpdate()
    {
        if (_pullParent)
            PullParent();
    }

    private void Break()
    {
        OnRootBreak?.Invoke(this);
    }

    public void PullParent()
    {
        var pullDir = _target.position - _parentRB.transform.position;
        _parentRB?.AddForce(pullDir * _pullForce);
    }

    private bool IsValidRoot()
    {
        // length check
        var rootLength = Vector3.Distance(_target.position, transform.position);
        if (rootLength > MaxLength)
            return false;

        // direction check
        var rootDir = _target.position - transform.position;
        if (Input.GetMouseButton(0))
        {
            var mousePosWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            var moveDir = mousePosWorld - transform.position;

            if (Vector2.Dot(moveDir, rootDir) < _breakAngleDot)
                return false;
        }

        // raycast empty check
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rootDir, rootDir.magnitude - 0.1f, _layerMaskWalls);
        if (hit)
            return false;

        return true;
    }
}
