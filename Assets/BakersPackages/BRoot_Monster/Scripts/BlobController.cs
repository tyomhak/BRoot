using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobController : MonoBehaviour
{
    PlayerController _pc;
    Rigidbody2D _rb;

    [Header("Movement")]
    [SerializeField] float _deadzone;

    [Header("Raycast config")]
    [SerializeField] float _rayOffset = 1f;
    [SerializeField] LayerMask _layerMask_lineOfSight;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (!_pc && transform.parent.TryGetComponent(out PlayerController pc))
        {
            _pc = pc;
            _pc.AttachBlob(this);
        }
    }

    private void OnDisable()
    {
        if (_pc)
        {
            _pc.RemoveBlob(this);
        }
    }

    public void AddForce(Vector2 force)
    {
        _rb.AddForce(force, ForceMode2D.Force);
    }

    public void SetTarget(Vector3 target, float force)
    {
        var forceDir = target - transform.position;

        if (forceDir.magnitude < _deadzone)
            return;

        _rb.AddForce(forceDir * force, ForceMode2D.Force);
    }

    public bool LineOfSight(Vector3 target)
    {
        target.z = 0;
        var toTarget = target - transform.position;
        var toTargetDir = toTarget.normalized;

        var startOffset = toTargetDir * _rayOffset;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + startOffset, toTargetDir, toTarget.magnitude - _rayOffset - 0.1f, _layerMask_lineOfSight);
        if (hit)
        {
            return false;
        }

        return true;
    }
}
