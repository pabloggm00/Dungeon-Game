using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSpawn : MonoBehaviour
{
    public int radius;
    public SphereCollider coll;
    public LayerMask layerMaskAreaEnemy;
    public Transform parentObjectScale;
    public bool isShowGizmos;

    private void Start()
    {
        coll.radius = radius * (1/parentObjectScale.localScale.x);
    }

    void OnDrawGizmos()
    {
        if (isShowGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    
    }


    public bool IsInAreaEnemy()
    {
        Collider[] colliders = new Collider[10];
        bool isInArea = Physics.OverlapSphereNonAlloc(this.transform.position, radius, colliders, layerMaskAreaEnemy) >= 1 ? true : false;
   
        return isInArea;
    }
}
