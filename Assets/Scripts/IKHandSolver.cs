using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandSolver : MonoBehaviour
{
    public Transform source;

    void LateUpdate()
    {
        transform.position = source.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
