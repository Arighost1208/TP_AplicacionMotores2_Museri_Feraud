using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    
    public float patrolSpeed = 0f;
    public float changeTargetDistance = 0.1f;
    public Transform[] patrolWaypoints;

    int currentTarget = 0;
   
    void Update()
    {
        if (MoveToTarget())
        {
            currentTarget = GetNextTarget();
        }
    }

    private bool MoveToTarget()
    {
        Vector3 distanceVector = patrolWaypoints[currentTarget].position - transform.position;
        if(distanceVector.magnitude < changeTargetDistance)
        {
            return true;
        }

        Vector3 velocityVector = distanceVector.normalized;
        transform.position += velocityVector * patrolSpeed * Time.deltaTime;

        return false;
    }

    private int GetNextTarget()
    {
        currentTarget++;

        if(currentTarget >= patrolWaypoints.Length)
        {
            currentTarget = 0;
        }

        return currentTarget;
    }
}
