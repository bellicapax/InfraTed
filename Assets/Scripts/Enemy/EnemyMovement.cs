using UnityEngine;
using System.Collections;

using Pathfinding;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

    public bool newPatrolPath = false;
    public float normalSpeed = 1.0f;
    public float alertedSpeed = 2.0f;
    public float normalRotateSpeed = 1.0f;
    public float fastRotateSpeed = 4.0f;
    public float nextWaypointDistance = 3.0f;
    public Transform[] aryTransPatrol;

    private bool calculatingPath = false;
    private int currentWaypoint = 0;
    private int patrolCounter = 0;
    private string hot = "Hot";
    private string cold = "Cold";
    private GameObject goCharacter;
    private List<GameObject> lisHotColdObjects = new List<GameObject>();
    private EnemySight scriptSight;
    private EnemyState scriptState;
    private Rigidbody myRigidbody;
    private Transform myTransform;
    private Transform currentHotColdTrans;
    private Seeker scriptSeeker;
    private Path myPath;
    


	// Use this for initialization
	void Start () 
    {
        myRigidbody = this.rigidbody;
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
        scriptSight = GetComponent<EnemySight>();
        scriptState = GetComponentInChildren<EnemyState>();
        scriptSeeker = GetComponent<Seeker>();
	}
	
	void FixedUpdate () 
    {
        switch (scriptState.nmeCurrentState)
        {
            case EnemyState.CurrentState.Patroling:
                Patrol();
                break;

            case EnemyState.CurrentState.Chasing:
                FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
                break;

            case EnemyState.CurrentState.Firing:
                FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
                break;

            case EnemyState.CurrentState.Turning:
                FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
                break;

            case EnemyState.CurrentState.Stationary:

                break;
                

        }
	}

    void FaceTarget(Vector3 parTarget, float parRotateSpeed, bool parConstrainXZAxes)
    {
        Quaternion target = Quaternion.LookRotation(parTarget - myTransform.position);
        if (parConstrainXZAxes)
        {
            target.x = 0.0f;
            target.z = 0.0f;
        }
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target, Time.deltaTime * parRotateSpeed);
    }

    void MoveTowards(Vector3 parTarget, float parMoveSpeed)
    {
        myRigidbody.MovePosition(rigidbody.position + new Vector3(parTarget.x, 0.0f, parTarget.z) * Time.fixedDeltaTime * parMoveSpeed);
    }

    Transform FindNearestHotOrColdObject()
    {
        lisHotColdObjects.AddRange(GameObject.FindGameObjectsWithTag(hot));     //Put all the hot objects in the list
        lisHotColdObjects.AddRange(GameObject.FindGameObjectsWithTag(cold));    //Put all the cold objects in the list

        float nearestSqr = Mathf.Infinity;
        Transform nearestTran = null;

        foreach (GameObject aGO in lisHotColdObjects)
        {
            float distanceSqr = (aGO.transform.position - myTransform.position).sqrMagnitude;
            //print("Name: " + aGO.name + " Magnitude Squared: " + distanceSqr);

            if (distanceSqr < nearestSqr && aGO.transform != currentHotColdTrans)
            {
                nearestSqr = distanceSqr;
                nearestTran = aGO.transform;
                //print(true);
            }
        }

        return nearestTran;
    }

    void OnPathComplete(Path parPath)
    {
        if (parPath.error)
        {
            Debug.LogError(parPath.errorLog);
        }
        else
        {
            myPath = parPath;
            currentWaypoint = 0;
            calculatingPath = false;
        }
    }
    
    void Patrol()
    {
        if (calculatingPath)
        {
            return;
        }
        if (myPath == null)
        {
            GetANewPath();
            print("myPath is null!");
            calculatingPath = true;
            return;
        }
        else if (currentHotColdTrans.tag != hot && currentHotColdTrans.tag != cold)     //If the object is no longer hot or cold, get a new path to a hot or cold object.  (The non-hot or cold object will not be included in the new list)
        {
            GetANewPath();
            print("Current object no longer hot or cold!");
            calculatingPath = true;
            return;
        }
        else if (currentWaypoint >= myPath.vectorPath.Count)                            //If we have reached the end of the path
        {
            GetANewPath();
            print("Reached the end of the path!");
            calculatingPath = true;
            return;
        }
        else if (newPatrolPath)
        {
            GetANewPath();
            print("Touching objective in the path.");
            calculatingPath = true;
            newPatrolPath = false;
            return;
        }
        FaceTarget(myPath.vectorPath[currentWaypoint], normalRotateSpeed, true);
        Vector3 dir = (myPath.vectorPath[currentWaypoint] - myTransform.position).normalized;   //Get the normalized direction to the next waypoint
        MoveTowards(dir, normalSpeed);                                                          //Move towards that waypoint
        if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance)  //If we are close enough to the current waypoint, start moving towards the next waypoint.
        {
            currentWaypoint++;
            //print("New Waypoint: " + currentWaypoint);
        }

    }

    void GetANewPath()
    {
        if (patrolCounter + 1 >= aryTransPatrol.Length)
        {
            patrolCounter = 0;
        }
            currentHotColdTrans = aryTransPatrol[patrolCounter];
            patrolCounter++;
        if (currentHotColdTrans != null)
        {
            scriptSeeker.StartPath(myTransform.position, currentHotColdTrans.position, OnPathComplete);
        }
    }
}
