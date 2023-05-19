using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool _initialized = false;

    List<BlobController> _blobs;
    public int BlobCount => _blobs.Count;

    [Header("Movement")]
    [SerializeField] float _mainForce = 40f;
    [SerializeField] float _secondaryForce = 20f;

    [Space]
    [Header("Roots")]
    [SerializeField] bool _shootRoots;
    [SerializeField] float _mainAimFOV = 30f;
    [SerializeField] float _secondaryAimFOV = 60f;


    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0)) 
        {
            Vector2 targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var sideBlobs = GetBlobsWithLineOfSight(targetPoint);
            BlobController mainBlob = null;

            if (sideBlobs.Count > 0) 
                mainBlob = GetClosestBlob(sideBlobs, targetPoint);
            else
                mainBlob = GetClosestBlob(_blobs, targetPoint);

            mainBlob.SetTarget(targetPoint, _mainForce * BlobCount);

            if (_shootRoots)
            {
                {
                    var rootManag = mainBlob.GetComponent<RootManager>();
                    if (rootManag.SelectAttachPoint(targetPoint, out Vector3 attachPoint, _mainAimFOV))
                    {
                        rootManag.AttachTo(attachPoint);
                    }
                }

                foreach(var blob in sideBlobs)
                {
                    var rootManag = blob.GetComponent<RootManager>();
                    if (rootManag.SelectAttachPoint(targetPoint, out Vector3 attachPoint, _secondaryAimFOV))
                    {
                        rootManag.AttachTo(attachPoint);
                    }
                    else
                        Debug.Log("NoTargetFound");
                }
            }
        }
    }


    private BlobController GetMainBlob(Vector2 target)
    {
        BlobController mainBlob = _blobs[0];
        float distToMainBlob = Vector2.Distance(target, mainBlob.transform.position);

        foreach (BlobController blob in _blobs)
        {
            float newDistToBlob = Vector2.Distance(target, blob.transform.position);
            if (newDistToBlob < distToMainBlob)
            {
                mainBlob = blob;
                distToMainBlob = newDistToBlob;
            }
        }

        return mainBlob;
    }

    private BlobController GetClosestBlob(List<BlobController> blobs, Vector3 target)
    {
        float minDistance = float.MaxValue;
        BlobController closestBlob = null;

        foreach(BlobController blob in blobs)
        {
            var currDist = Vector2.Distance(blob.transform.position, target);
            if (currDist < minDistance)
            {
                closestBlob = blob;
                minDistance = currDist;
            }
        }

        return closestBlob;
    }

    private List<BlobController> GetBlobsWithLineOfSight(Vector2 target)
    {
        return _blobs.Where(blob => blob.LineOfSight(target) == true).ToList();
    }

    public void Init()
    {
        if (_initialized)
            return;

        _blobs = new List<BlobController>();
        _initialized = true;
    }

    public void AttachBlob(BlobController blob)
    {
        if (!_initialized)
            Init();

        _blobs.Add(blob);
    }

    public void RemoveBlob(BlobController blob)
    {
        _blobs.Remove(blob);
    }
}
