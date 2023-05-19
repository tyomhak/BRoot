using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Blob : MonoBehaviour
{
    Dictionary<Rigidbody2D, SpringJoint2D> m_springJoints;

    private void Awake()
    {
        m_springJoints = new();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Blob>(out var blob))
            return;

        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out var otherRB))
        {
            if (!m_springJoints.ContainsKey(otherRB)) 
            {
                ConnectJoint(otherRB);
            }
        }
    }


    public void ConnectJoint(Rigidbody2D otherRB)
    {
        var joint = transform.AddComponent<SpringJoint2D>();
        joint.connectedBody = otherRB;

        m_springJoints[otherRB] = joint;
    }

}
