using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobManager : MonoBehaviour
{
    [SerializeField] Health _health;

    List<BlobController> _blobs;
    public int BlobCount => _blobs.Count;

    private void Awake()
    {
        _blobs = new();
    }


    void AddBlob()
    {

    }

    void RemoveBlob()
    {

    }
}
