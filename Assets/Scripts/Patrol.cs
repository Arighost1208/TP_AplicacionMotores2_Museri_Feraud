
using UnityEngine;

[ExecuteInEditMode]
public class Patrol : MonoBehaviour
{
   
    public float patrolSpeed = 0f;
    public float changeTargetDistance = 0.1f;
    public Transform[] patrolWaypoints;

    public int currentTarget = 0;

    void Update()
    {
        if (MoveToTarget())
        {
            currentTarget = GetNextTarget();
            Debug.Log("gato");
        }
    }

    public bool MoveToTarget()
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

    public int GetNextTarget()
    {
        currentTarget++;

        if(currentTarget >= patrolWaypoints.Length)
        {
            currentTarget = 0;
        }

        return currentTarget;
    }
}
