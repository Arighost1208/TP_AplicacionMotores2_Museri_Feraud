
using UnityEngine;

[ExecuteInEditMode]
public class Patrol : MonoBehaviour
{

    float patrolSpeed = 5f;
    float changeTargetDistance = 0.5f;
    public Transform patrolWaypoints;
   
     public int currentTarget = 1;

    void Update()
    {
        if (patrolWaypoints)
        {
            if (MoveToTarget())
            {
                currentTarget = GetNextTarget();
            }
        }
        
    }

    public bool MoveToTarget()
    {
        //Vector3 distanceVector = patrolWaypoints[currentTarget].position - transform.position;
        Vector3 distanceVector = patrolWaypoints.GetComponentsInChildren<Transform>()[currentTarget].position - transform.position;
        if (distanceVector.magnitude < changeTargetDistance)
        {
            return true;          
        }

        Vector3 velocityVector = distanceVector.normalized;
        transform.position += velocityVector * patrolSpeed * Time.deltaTime;
      
        return false;
    }

    public int GetNextTarget()
    {
        currentTarget++;

        int _count = patrolWaypoints.GetComponentsInChildren<Transform>().Length - 1;
        if (currentTarget > _count)
        {
            currentTarget = 1;
        }

        return currentTarget;
    }
}
