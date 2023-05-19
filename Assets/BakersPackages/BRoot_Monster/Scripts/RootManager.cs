using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootManager : MonoBehaviour
{
    [SerializeField] RootTentacle _tentaclePF;
    [SerializeField] int _initialCount;
    Stack<RootTentacle> _rootsFree;
    List<RootTentacle> _rootsInUse;

    [Header("Raycasts")]
    [SerializeField] int _numOfRays;
    [SerializeField] LayerMask _layerMaskWalls;

    [SerializeField] bool _minLengthEnabled = true;
    [SerializeField] float _minRootLength = 1f;

    [SerializeField] bool _minDistanceBetweenRootsEnabled = true;
    [SerializeField] float _minDistanceBetweenRootEnds = 0f;
    Vector3 _lastAttachedPosition;

    [SerializeField] bool _rootAttachCooldown = true;
    [SerializeField] float _minTimeBetweenRootAttachements = 0f;
    float _lastAttachTime = 0f;

    private void Awake()
    {
        _lastAttachedPosition = Vector3.zero;
        _lastAttachTime = Time.time; // questionable choice.

        _rootsFree = new();
        _rootsInUse = new();

        AddRoots(_initialCount);
    }

    public bool SelectAttachPoint(Vector3 targetPosition, out Vector3 resultPoint, float aimFov)
    {
        bool resultFound = false;
        resultPoint = Vector3.zero;

        Vector3 targetDirection = targetPosition - transform.position;
        float degreeInterval = aimFov / _numOfRays;

        float longestSqrDist = 0f;

        var startDirection = Quaternion.Euler(0, 0, -aimFov / 2f) * targetDirection;
        for (int i = 0; i < _numOfRays; ++i)
        {
            Vector3 currDirection = Quaternion.Euler(0, 0, degreeInterval * i) * startDirection;
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, currDirection, RootTentacle.MaxLength, _layerMaskWalls);
            if (hit)
            {
                if (resultFound)
                {
                    Vector3 point = hit.point;
                    var currSqrDist = Vector3.SqrMagnitude(point - transform.position);
                    if (currSqrDist > longestSqrDist)
                        resultPoint = point;
                }
                else
                {
                    resultPoint = hit.point;
                    longestSqrDist = Vector3.SqrMagnitude(resultPoint - transform.position);
                    resultFound = true;
                }
            }
        }

        return resultFound;
    }

    public bool SelectAttachPoints(Vector3 targetPosition, out List<Vector3> resultPoints, float aimFov)
    {
        bool resultFound = false;
        resultPoints = new();

        Vector3 targetDirection = targetPosition - transform.position;
        float degreeInterval = aimFov / _numOfRays;

        var startDirection = Quaternion.Euler(0, 0, -aimFov / 2f) * targetDirection;
        for (int i = 0; i < _numOfRays; ++i)
        {
            Vector3 currDirection = Quaternion.Euler(0, 0, degreeInterval * i) * startDirection;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currDirection, RootTentacle.MaxLength, _layerMaskWalls);
            if (hit)
            {
                resultPoints.Add(hit.point);
                resultFound = true;
            }
        }

        return resultFound;
    }

    public void AttachTo(Vector2 targetPosition)
    {
        if (!IsValidTarget(targetPosition))
            return;

        if (TryEmployRoot(out RootTentacle root))
        {
            root.SetTarget(targetPosition);
            root.gameObject.SetActive(true);
        }
    }

    private bool IsValidTarget(Vector2 targetPosition)
    {
        // min root length check
        if (_minLengthEnabled && Vector2.Distance(transform.position, targetPosition) < _minRootLength)
            return false;

        // min distance between root ends check
        if (_minDistanceBetweenRootsEnabled && Vector2.Distance(targetPosition, _lastAttachedPosition) < _minDistanceBetweenRootEnds)
            return false;

        // cooldown check
        if (_rootAttachCooldown)
        {
            var currTime = Time.time;
            if (currTime - _lastAttachTime < _minTimeBetweenRootAttachements)
                return false;
            else
                _lastAttachTime = currTime;
        }

        return true;
    }


    private bool TryEmployRoot(out RootTentacle root)
    {
        root = null;

        if (_rootsFree.Count > 0)
        {
            root = _rootsFree.Pop();
            _rootsInUse.Add(root);
            root.OnRootBreak += Root_OnRootBreak;
            return true;
        }

        return false;
    }

    private void Root_OnRootBreak(RootTentacle root)
    {
        ReleaseRoot(root);
    }

    private void ReleaseRoot(RootTentacle root)
    {
        root.OnRootBreak -= Root_OnRootBreak;
        _rootsInUse.Remove(root);
        
        _rootsFree.Push(root);
        root.gameObject.SetActive(false);
    }

    public void AddRoots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var tent = Instantiate(_tentaclePF, transform);
            tent.gameObject.SetActive(false);
            _rootsFree.Push(tent);
        }
    }

    //private RootTentacle EmployRoot()
    //{
    //    if (_rootsFree.Count < 1)
    //    {
    //        ReleaseRoot();
    //    }

    //    RootTentacle root = _rootsFree.Pop();
    //    _rootsInUse.Add(root);
    //    root.OnRootBreak += Root_OnRootBreak;
    //    return root;
    //}

    //private void ReleaseRoot()
    //{
    //    if (_rootsInUse.Count > 0)
    //    {
    //        RootTentacle root = _rootsInUse[0];
    //        ReleaseRoot(root);
    //    }
    //    else
    //        AddRoots(1);
    //}

}
