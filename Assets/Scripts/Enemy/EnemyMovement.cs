using UnityEngine;
using System.Collections;

using Pathfinding;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

    public bool changedStates = false;
    public bool newPatrolPath = false;
    public int decimalRounding = 2;
    public int framesAllowedStationary = 10;
    public float normalSpeed = 1.0f;
    public float alertedSpeed = 2.0f;
    public float normalRotateSpeed = 1.0f;
    public float fastRotateSpeed = 4.0f;
    public float nextWaypointDistance = 3.0f;
    public string nameTouch;
    public string nameBump;
    public List<Transform> listTransPatrol = new List<Transform>();

    private bool calculatingPath = false;
    private int currentWaypoint = 0;
    private int patrolCounter = 0;
    private int stuckCounter;
    private string hot = "Hot";
    private string cold = "Cold";
    private Vector3 lastMyPosition;
    private GameObject goCharacter;
    private List<GameObject> listHotColdObjects = new List<GameObject>();
    private CharacterController myCharContro;
    private Transform myTransform;
    private Transform currentHotColdTrans;
    private HeatControl scriptHeat;
    private Seeker scriptSeeker;
    private EnemyState scriptState;
    private EnemySight scriptSight;
    private EnemyBump scriptBump;
    private Path myPath;
    private EnemyState.CurrentState lastState;


	// Use this for initialization
	void Start () 
    {
        stuckCounter = framesAllowedStationary;
        myCharContro = this.GetComponent<CharacterController>();
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
        scriptState = GetComponentInChildren<EnemyState>();
        scriptSeeker = GetComponent<Seeker>();
        scriptSight = GetComponentInChildren<EnemySight>();
        scriptBump = GetComponentInChildren<EnemyBump>();
        lastState = scriptState.nmeCurrentState;
	}
	
	void FixedUpdate () 
    {
        if (lastState != scriptState.nmeCurrentState)
        {
            changedStates = true;
            print("Last state: " + lastState + "  Current state: " + scriptState.nmeCurrentState);
        }
        switch (scriptState.nmeCurrentState)
        {
            case EnemyState.CurrentState.Patroling:
                Patrol();
                break;

            case EnemyState.CurrentState.Chasing:
                Chasing(alertedSpeed);
                break;

            case EnemyState.CurrentState.Firing:
                Chasing(normalSpeed);
                break;

            case EnemyState.CurrentState.Turning:
                FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
                break;

            case EnemyState.CurrentState.Padding:

                break;

            case EnemyState.CurrentState.Stationary:

                break;
        }
        lastState = scriptState.nmeCurrentState;
        lastMyPosition = myTransform.position;
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
        if (calculatingPath)            // If we are waiting on the pathfinding, don't do anything else until we have a path
        {
            return;
        }
        if (scriptSight.useSphericalHeatSensor) // If we are only patroling between objects that are currently out of the room temperature range, check to see if they are still in that range
        {
            if (RemoveLukewarmObjects())    // If removing the objects necessitates creating a new path, return
            {
                return;
            }
        }
        if (WeNeedANewPath())
        {
            return;
        }

        FaceTarget(myPath.vectorPath[currentWaypoint], normalRotateSpeed, true);                //Face the waypoint as we are going there
        Vector3 dir = (myPath.vectorPath[currentWaypoint] - myTransform.position).normalized;   //Get the normalized direction to the next waypoint
        MoveTowards(dir, normalSpeed);                                                          //Move towards that waypoint

        if (IHaveBeenStuck(false))
            return;

        if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance)  //If we are close enough to the current waypoint, start moving towards the next waypoint.
        {
            currentWaypoint++;
            //print("New Waypoint: " + currentWaypoint);
        }

    }

    void Chasing(float parChaseSpeed)
    {
        if (PathIsClear())
        {
            print("Path is clear.");
            Vector3 direction = goCharacter.transform.position - myTransform.position;
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            MoveTowards(direction.normalized, parChaseSpeed);
        }
        else if (GettingAPathToCharacter())
        {
            print("Finding a path to character.");
            return;
        }
        else
        {
            print("Moving along path.");
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            Vector3 dir = (myPath.vectorPath[currentWaypoint] - myTransform.position).normalized;
            MoveTowards(dir, parChaseSpeed);
            if (IHaveBeenStuck(true))
                return;
            print("Last position: " + lastMyPosition + " Current position: " + myTransform.position);
            if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance)  //If we are close enough to the current waypoint, start moving towards the next waypoint.
            {
                currentWaypoint++;
                //print("New Waypoint: " + currentWaypoint);
            }
        }
    }

    void GetANewPatrolPath()
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

    bool RemoveLukewarmObjects()
    {
        for (int i = 0; i < listTransPatrol.Count; i++)
        {
            scriptHeat = listTransPatrol[i].GetComponent<HeatControl>();
            if (listTransPatrol[i].tag != hot && listTransPatrol[i].tag != cold && scriptHeat.inHeatSensorRange)
            {
                if (listTransPatrol[i] == currentHotColdTrans)      // If the item in the list is no longer hot or cold, we need to remove it from the list
                {
                    listTransPatrol.RemoveAt(i);
                    //print("Patrol counter: " + patrolCounter +  " List count: " + listTransPatrol.Count);
                    if (patrolCounter != 0)                         // If it's not zero (e.g., if the last in the sequence got removed, so the counter would already be at zero)
                    {
                        patrolCounter--;                            // Decrement by one to get it to target the "next" transform
                    }
                    //print("Removed current target from list.  Patrol counter = " + patrolCounter);
                    GetANewPatrolPath();
                    calculatingPath = true;
                    return true;                                    // !@#$ What happens here when two objects need removing and only one gets removed because we calculate a new path?  Would it be better or worse to do one at a time?
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
        return false;
    }

    bool WeNeedANewPath()
    {
        if (myPath == null)             // If we have no path, get one
        {
            GetANewPatrolPath();
            calculatingPath = true;
            return true;
        }
        else if (changedStates)
        {
            GetANewPatrolPath();
            calculatingPath = true;
            changedStates = false;
            return true;
        }
        else if (currentWaypoint >= myPath.vectorPath.Count)                            //If we have reached the end of the path
        {
            GetANewPatrolPath();
            calculatingPath = true;
            return true;
        }
        else if (newPatrolPath)                                                         //If we have touched the object we are trying to reach (but can't get close enough to the waypoint to have reached the end of the path)
        {
            GetANewPatrolPath();
            print("Touching objective in the path.");
            calculatingPath = true;
            newPatrolPath = false;
            return true;
        }
        else if (scriptBump.isBumping)
        {
            if (nameBump != nameTouch)
            {
                scriptSeeker.StartPath(myTransform.position, currentHotColdTrans.position, OnPathComplete);     // We don't want a new path based on the new, incremented patrol counter.  Just use the current objective.
                print("Bumped into obstacle while patroling.");
                scriptBump.isBumping = false;
                return false;                                                                                   // So it doesn't hiccup when we change paths, don't return and don't set calculating path to true
            }
            else
                return false;
        }
        else
            return false;
    }

    bool PathIsClear()
    {
        RaycastHit hit;
        Vector3 direction = goCharacter.transform.position - myTransform.position;
        if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == goCharacter)
            {
                return true;
            }
        }
        return false;
    }

    bool GettingAPathToCharacter()
    {
        if (calculatingPath)
        {
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            return true;
        }
        else if (changedStates)                                                         // We might have a path left over from another state, so clear the path and get a new one, but look at the character while we are waiting on the path
        {
            myPath = null;
            scriptSeeker.StartPath(myTransform.position, goCharacter.transform.position, OnPathComplete);
            changedStates = false;
            calculatingPath = true;
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            return true;
        }
        else if (myPath == null)
        {
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            scriptSeeker.StartPath(myTransform.position, goCharacter.transform.position, OnPathComplete);
            calculatingPath = true;
            return true;
        }
        else if (currentWaypoint >= myPath.vectorPath.Count)                            //If we have reached the end of the path
        {
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            scriptSeeker.StartPath(myTransform.position, goCharacter.transform.position, OnPathComplete);
            calculatingPath = true;
            return true;
        }
        else if (scriptBump.isBumping)
        {
            print("Bumped into obstacle while chasing.");
            FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
            scriptSeeker.StartPath(myTransform.position, goCharacter.transform.position, OnPathComplete);
            calculatingPath = true;
            scriptBump.isBumping = false;
            return true;
        }
        return false;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    bool IHaveBeenStuck(bool parChasing)
    {
        if (new Vector3(Round(myTransform.position.x, decimalRounding), Round(myTransform.position.y, decimalRounding), Round(myTransform.position.z, decimalRounding)) == new Vector3(Round(lastMyPosition.x, decimalRounding), Round(lastMyPosition.y, decimalRounding), Round(lastMyPosition.z, decimalRounding)))   //If we aren't moving, get a new path
        {
            stuckCounter--;
            if (stuckCounter <= 0)
            {
                if (parChasing)
                {
                    FaceTarget(goCharacter.transform.position, fastRotateSpeed, true);
                    scriptSeeker.StartPath(myTransform.position, goCharacter.transform.position, OnPathComplete);
                    calculatingPath = true;
                    stuckCounter = framesAllowedStationary;
                }
                else
                {
                    GetANewPatrolPath();
                    calculatingPath = true;
                    stuckCounter = framesAllowedStationary;
                }
                return true;
            }
        }
        else
        {
            stuckCounter = framesAllowedStationary;
        }
        return false;
    }
}
