using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class RootTentacle : MonoBehaviour
{
    public static float MaxLength = 15f;

    [Header("Breaking Config")]
    [SerializeField] float _breakAngleDot = 0;
    [SerializeField] float _maxRootLength = 20f;
    [SerializeField] LayerMask _layerMaskWalls;

    [Header("Pull Parent")]
    [SerializeField] bool _pullParent;
    [SerializeField] float _pullForce = 5f;
    Rigidbody2D _parentRB;

    LineRenderer _lr;
    Transform _target;
    Camera _cam;

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

    public void SetTarget(Vector3 targetPosition)
    {
        _target.position = targetPosition;
    }

    private void UpdatePositions()
    {
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, _target.position);
    }

    private void Update()
    {
        // length check
        var rootLength = Vector3.Distance(_target.position, transform.position);
        if (rootLength > MaxLength)
        {
            Break();
            return;
        }

        // direction check
        var rootDir = _target.position - transform.position;
        if (Input.GetMouseButton(0))
        {
            var mousePosWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            var moveDir = mousePosWorld - transform.position;

            if (Vector2.Dot(moveDir, rootDir) < _breakAngleDot)
            {
                Break();
                return;
            }
        }

        // raycast empty check
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rootDir, rootDir.magnitude - 0.1f, _layerMaskWalls);
        if (hit)
        {
            Break();
            return;
        }

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
}
