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
    public List<Transform> listTransPatrol = new List<Transform>();

    private bool calculatingPath = false;
    private int currentWaypoint = 0;
    private int patrolCounter = 0;
    private string hot = "Hot";
    private string cold = "Cold";
    private GameObject goCharacter;
    private List<GameObject> listHotColdObjects = new List<GameObject>();
    private EnemyState scriptState;
    private CharacterController myCharContro;
    private Transform myTransform;
    private Transform currentHotColdTrans;
    private Seeker scriptSeeker;
    private Path myPath;
    


	// Use this for initialization
	void Start () 
    {
        myCharContro = this.GetComponent<CharacterController>();
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
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
        myCharContro.SimpleMove(new Vector3(parTarget.x, 0.0f, parTarget.z) * Time.fixedDeltaTime * parMoveSpeed);
    }

    Transform FindNearestHotOrColdObject()
    {
        listHotColdObjects.AddRange(GameObject.FindGameObjectsWithTag(hot));     //Put all the hot objects in the list
        listHotColdObjects.AddRange(GameObject.FindGameObjectsWithTag(cold));    //Put all the cold objects in the list

        float nearestSqr = Mathf.Infinity;
        Transform nearestTran = null;

        foreach (GameObject aGO in listHotColdObjects)
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
        for (int i = 0; i < listTransPatrol.Count; i++)
        {
            if (listTransPatrol[i].tag != hot && listTransPatrol[i].tag != cold)
            {
                if (listTransPatrol[i] == currentHotColdTrans)      // If the item in the list is no longer hot or cold, we need to remove it from the list
                {
                    listTransPatrol.RemoveAt(i);
                    if (patrolCounter >= listTransPatrol.Count)     // If the counter is now greater than the number of items, set it back to zero
                    {
                        patrolCounter = 0;
                    }
                    else if(patrolCounter != 0)         // If it's not zero (e.g., if the last in the sequence got removed, so the counter would already be at zero)
                    {
                        patrolCounter--;                // Decrement by one to get it to target the "next" transform
                    }
                    GetANewPath();
                }
                else
                {
                    listTransPatrol.RemoveAt(i);
                    if (patrolCounter >= listTransPatrol.Count)
                    {
                        patrolCounter = 0;
                    }
                }
            }
        }
        if (myPath == null)             // If we have no path, get one
        {
            GetANewPath();
            print("myPath is null!");
            calculatingPath = true;
            return;
        }
        else if (currentHotColdTrans.tag != hot && currentHotColdTrans.tag != cold)     //If the object is no longer hot or cold, get a new path to a hot or cold object.  (The non-hot or cold object will not be included in the new list)
        {
            print("Original Count " + listTransPatrol.Count);
            listTransPatrol.RemoveAt(patrolCounter);
            print("After removeat " + listTransPatrol.Count);
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
        else if (newPatrolPath)                                                         //If we have touched the object we are trying to reach (but can't get close enough to the waypoint to have reached the end of the path)
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
        currentHotColdTrans = listTransPatrol[patrolCounter]; // Assign the current target to the item in the array equal to the patrolCounter

        patrolCounter++;                                    // Increment the patrolCounter
        
        if (patrolCounter >= listTransPatrol.Count)         // If the patrolCounter is greater than or equal to the count, reset it to zero (since the counter and the array start at zero)
        {
            patrolCounter = 0;
        }
        if (currentHotColdTrans != null)
        {
            scriptSeeker.StartPath(myTransform.position, currentHotColdTrans.position, OnPathComplete);
        }
    }
}
