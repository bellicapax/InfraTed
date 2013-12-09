using UnityEngine;
using System.Collections;

using Pathfinding;
using System.Collections.Generic;

public class SensorBotMovement : MonoBehaviour {

public bool changedStates = false;
    public bool newPatrolPath = false;
    public int decimalRounding = 3;
    public float secondsAllowedStationary = 0.5f;
    public float secondsBetweenSlowerUpdate = 0.2f;
    public float normalSpeed = 1.0f;
    public float alertedSpeed = 2.0f;
    public float normalRotateSpeed = 1.0f;
    public float fastRotateSpeed = 4.0f;
    public float searchLookSpeed = 50.0f;
    public float nextWaypointDistance = 1.0f;
    public float percentOfFOVToContinuePath = 0.3f;
    public List<Transform> listTransPatrol = new List<Transform>();
    public Transform xCurrentHotColdTrans;
    public LayerMask groundMask;


    private bool saidIt = false;
    private bool calculatingPath = false;
    private bool setSearchRotation = false;
    private bool doneSearching = false;
    private bool iAmStuck = false;
    private bool clearPath = true;
    private bool sprayingCoolant = false;
    private int currentWaypoint = 0;
    private int patrolCounter = 0;
    private float stuckCounter;
    private float radiusOfCharControl;
    private string hot = "Hot";
    private string cold = "Cold";
    private Vector3 lastMyPosition;
    private Quaternion endFirstDirection;
    private Quaternion endSecondDirection;
    private Quaternion targetSearchRotation;
    private GameObject goCharacter;
    private List<GameObject> listHotColdObjects = new List<GameObject>();
    private CharacterController myCharContro;
    private Transform myTransform;
    private Transform transCharacter;
    private ParticleSystem prtSystems = new ParticleSystem();
    private HeatControl scriptHeat;
    private Seeker scriptSeeker;
    private SensorBotState scriptState;
    private SensorBotSight scriptSight;
    //private EnemyBump scriptBump;
    private Path myPath;
    private SensorBotState.CurrentState lastState;
	
	void Awake()
    {
        listTransPatrol.RemoveAll(t => t == null);
    }

	// Use this for initialization
	void Start () 
    {
        stuckCounter = secondsAllowedStationary;
        myCharContro = this.GetComponent<CharacterController>();
        radiusOfCharControl = myCharContro.radius;
        //groundMask = 1 << LayerMask.NameToLayer("Ground");
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
        transCharacter = goCharacter.transform;

        prtSystems = GetComponentInChildren<ParticleSystem>();
        scriptState = GetComponentInChildren<SensorBotState>();
        scriptSeeker = GetComponent<Seeker>();
        scriptSight = GetComponentInChildren<SensorBotSight>();
		

        lastState = scriptState.nmeCurrentState;
        lastMyPosition = myTransform.position;

        listTransPatrol.TrimExcess();

        InvokeRepeating("LessFrequentUpdate", secondsBetweenSlowerUpdate, secondsBetweenSlowerUpdate);
	}
	
	void FixedUpdate () 
    {
		
        if (lastState != scriptState.nmeCurrentState)
        {
            if (!((lastState == SensorBotState.CurrentState.Chasing && scriptState.nmeCurrentState == SensorBotState.CurrentState.Firing) || (lastState == SensorBotState.CurrentState.Firing && scriptState.nmeCurrentState == SensorBotState.CurrentState.Chasing))) // If we're not just changing between chasing and firing
            {
                changedStates = true;
                //print("Last state: " + lastState + "  Current state: " + scriptState.nmeCurrentState);
            }
        }
        else
        {
            changedStates = false;
        }
        switch (scriptState.nmeCurrentState)
        {
            case SensorBotState.CurrentState.Patroling:
                StopCoolant();
                Patrol();
                break;

            case SensorBotState.CurrentState.Chasing:
                StopCoolant();
                Chasing(alertedSpeed);
                break;

            case SensorBotState.CurrentState.Firing:
                Chasing(normalSpeed);
                SprayCoolant();
                break;

            case SensorBotState.CurrentState.Turning:
                StopCoolant();
                FaceTarget(transCharacter.position, fastRotateSpeed, true);
                break;

            case SensorBotState.CurrentState.Padding:

                break;

            case SensorBotState.CurrentState.Searching:
                StopCoolant();
                Searching();
                break;

            case SensorBotState.CurrentState.Stationary:
                StopCoolant();
                break;
        }
        lastState = scriptState.nmeCurrentState;
	}

    void Patrol()
    {

        if (RemoveLukewarmObjects())    // Since we are only patroling between objects that are currently out of the room temperature range, check to see if they are still in that range
        {
            return;						// If removing the objects necessitates creating a new path, return
        }
		
        if (WeNeedANewPath(myTransform.forward, true, false))
        {
            return;
        }

        FaceTarget(myPath.vectorPath[currentWaypoint], normalRotateSpeed, true);                //Face the waypoint as we are going there
        Vector3 dir = (myPath.vectorPath[currentWaypoint] - myTransform.position).normalized;   //Get the normalized direction to the next waypoint
        MoveTowards(dir, normalSpeed);                                                          //Move towards that waypoint

        if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance)  //If we are close enough to the current waypoint, start moving towards the next waypoint.
        {
            currentWaypoint++;
            //print("New Waypoint: " + currentWaypoint);
        }

    }

    void Chasing(float parChaseSpeed)
    {
        if (clearPath)
        {
            //print("Clear!");
            Vector3 direction = transCharacter.position - myTransform.position;
            FaceTarget(transCharacter.position, fastRotateSpeed, true);
            MoveTowards(direction.normalized, parChaseSpeed);
        }
        else if (WeNeedANewPath(transCharacter.position, false, true))
        {
            return;
        }
        else
        {
            if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance && ((currentWaypoint + 1) < myPath.vectorPath.Count))  // If we are close enough to the current waypoint and there is another waypoint left, start moving towards the next waypoint.
            {                                                                                                       // I decided to put this before moving so that we don't move towards a waypoint unnecessarily 
                currentWaypoint++;
                //print("New Waypoint: " + currentWaypoint);
            }
            FaceTarget(transCharacter.position, fastRotateSpeed, true);
            Vector3 dir = (myPath.vectorPath[currentWaypoint] - myTransform.position).normalized;
            MoveTowards(dir, parChaseSpeed);

        }
    }

    void SprayCoolant()
    {
        if (!sprayingCoolant)
        {
            prtSystems.Play();
            sprayingCoolant = true;
        }
    }        
    
    void StopCoolant()
    {
        if (sprayingCoolant)
        {
            prtSystems.Stop();
            sprayingCoolant = false;
        }
    }

    void Searching()
    {
        if (Vector3.Distance(myTransform.position, new Vector3(scriptSight.personalLastSighting.x, myTransform.position.y, scriptSight.personalLastSighting.z)) > nextWaypointDistance)  // We don't want to check the difference in y's because the player might be above the enemy where it can't get.
        {
            if (clearPath)
            {
                FaceTarget(scriptSight.personalLastSighting, fastRotateSpeed, true);
                Vector3 direction = scriptSight.personalLastSighting - myTransform.position;
                MoveTowards(direction.normalized, alertedSpeed);
            }
            else if (WeNeedANewPath(scriptSight.personalLastSighting, false, false))
            {
                //print("Getting a new path while searching" + myTransform.name);
                return;
            }
            else
            {
                //print("Moving towards " + myPath.vectorPath[myPath.vectorPath.Count - 1]);
                FaceTarget(scriptSight.personalLastSighting, fastRotateSpeed, true);
                Vector3 direction = myPath.vectorPath[currentWaypoint] - myTransform.position;
                MoveTowards(direction.normalized, alertedSpeed);

                if (Vector3.Distance(myPath.vectorPath[currentWaypoint], myTransform.position) < nextWaypointDistance)  //If we are close enough to the current waypoint, start moving towards the next waypoint.
                {
                    currentWaypoint++;
                    //print("New Waypoint: " + currentWaypoint);
                }
            }
        }
        else
        {
            scriptState.justLostEm = false;
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

    bool WeNeedANewPath(Vector3 pathTarget, bool parOnPatrol, bool parChasing)
    {
        if (calculatingPath)
        {
            //print("Calculating path.");
            FaceTarget(pathTarget, fastRotateSpeed, true);
            return true;
        }
        else if (changedStates)
        {
            //print("New path because changed states.");
            myPath = null;
            GetAPath(pathTarget, parOnPatrol);
            changedStates = false;
            return true;
        }
        else if (myPath == null)             // If we have no path, get one
        {
            //print("Path is null.");
            GetAPath(pathTarget, parOnPatrol);
            return true;
        }
        else if (currentWaypoint >= myPath.vectorPath.Count)                            //If we have reached the end of the path
        {
            //print("Finished path.");
            GetAPath(pathTarget, parOnPatrol);
            return true;
        }
        else if (parOnPatrol && newPatrolPath)                                           //If we have touched the object we are trying to reach (but can't get close enough to the waypoint to have reached the end of the path)
        {
            //print("Touched the objective.");
            GetAPath(pathTarget, parOnPatrol);
            newPatrolPath = false;
            return true;
        }
        //else if (scriptBump.isBumping)
        //{
        //    print("Bumped into an obstacle.");
        //    if (parOnPatrol && nameBump != nameTouch)
        //    {
        //        scriptSeeker.StartPath(myTransform.position, xCurrentHotColdTrans.position, OnPathComplete);     // We don't want a new path based on the new, incremented patrol counter.  Just use the current objective.
        //        scriptBump.isBumping = false;
        //        return false;                                                                                   // So it doesn't hiccup when we change paths, don't return and don't set calculating path to true
        //    }
        //    else
        //    {
        //        Vector3 bumpTarget = pathTarget;
        //        print("Bumping.");
        //        FaceTarget(pathTarget, fastRotateSpeed, true);
        //        scriptSeeker.StartPath(myTransform.position, bumpTarget, OnPathComplete);
        //        return false;
        //    }
        //}
        else if (parChasing && WaypointPlayerAngle())
        {
            //print("Path end no longer leads to player.");
            FaceTarget(transCharacter.position, fastRotateSpeed, true);
            scriptSeeker.StartPath(myTransform.position, transCharacter.position, OnPathComplete);
            calculatingPath = true;
            return true;
        }
        else if (iAmStuck)
        {
            //print("I'm stuck!");
            GetAPath(pathTarget, parOnPatrol);
            iAmStuck = false;
            return true;
        }
        else
            return false;
    }

    bool WaypointPlayerAngle()
    {
        Vector3 pathDir = myPath.vectorPath[myPath.vectorPath.Count - 1] - myTransform.position;            // Get a vector direction between myself and the final point of the path
        Vector3 playerDir = transCharacter.position - myTransform.position;
        float pathPlayerAngle = Vector3.Angle(pathDir, playerDir);
        //print("Way X: " + pathDir.x + " Way Y: " + pathDir.y + " Way Z: " + pathDir.z + " Player X: " + playerDir.x + " Player Y: " + playerDir.y + " Player Z: " + playerDir.z + " Angle: " + pathPlayerAngle);
        if (pathPlayerAngle > scriptSight.fieldOfViewAngle * percentOfFOVToContinuePath)
            return true;
        else
            return false;
    }

    void GetAPath(Vector3 getPathTarget, bool onPatrol)
    {
        if (onPatrol)
            GetANewPatrolPath();
        else
        {
            Vector3 _target = getPathTarget;
            //print("The path target is:" + _target);
            FaceTarget(_target, fastRotateSpeed, true);
            scriptSeeker.StartPath(myTransform.position, getPathTarget, OnPathComplete);
        }
        calculatingPath = true;
    }

    void GetANewPatrolPath()
    {
        xCurrentHotColdTrans = listTransPatrol[patrolCounter]; // Assign the current target to the item in the array equal to the patrolCounter

        patrolCounter++;                                    // Increment the patrolCounter

        if (patrolCounter >= listTransPatrol.Count)         // If the patrolCounter is greater than or equal to the count, reset it to zero (since the counter and the array start at zero)
        {
            patrolCounter = 0;
        }
        if (xCurrentHotColdTrans != null)
        {
            scriptSeeker.StartPath(myTransform.position, xCurrentHotColdTrans.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path parPath)
    {
        if (parPath.error)
        {
            Debug.LogError(parPath.errorLog);
        }
        else
        {
            if (myPath != null)
                myPath.Release(this);

            myPath = parPath;
            myPath.Claim(this);

            currentWaypoint = 0;
            calculatingPath = false;
        }
    }

    bool RemoveLukewarmObjects()
    {
        if (listTransPatrol.Count > 0)
        {
            for (int i = 0; i < listTransPatrol.Count; i++)
            {
                scriptHeat = listTransPatrol[i].GetComponent<HeatControl>();
                if (listTransPatrol[i].tag != hot && listTransPatrol[i].tag != cold && scriptHeat.xInHeatSensorRange)
                {
                    if (listTransPatrol[i] == xCurrentHotColdTrans)      // If the item in the list is no longer hot or cold, we need to remove it from the list
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
        }
        return false;
    }

    void LookRightLeft()
    {
        if (!saidIt)
        {
            //print("Looking left and right.");
            saidIt = true;
        }
        if (!setSearchRotation)
        {
            Vector3 firstDirection;
            Vector3 secondDirection;
            if (Random.Range(0, 1) == 0)
            {
                firstDirection = myTransform.right;
            }
            else
            {
                firstDirection = -myTransform.right;
            }

            secondDirection = -firstDirection;
            endFirstDirection = Quaternion.LookRotation(firstDirection);
            endSecondDirection = Quaternion.LookRotation(secondDirection);
            targetSearchRotation = endFirstDirection;
            setSearchRotation = true;
        }
        if (myTransform.rotation == endFirstDirection && targetSearchRotation == endFirstDirection)
        {
            if (!doneSearching)
            {
                targetSearchRotation = endSecondDirection;
            }
            else
                scriptState.justLostEm = false;
        }
        else if (myTransform.rotation == endSecondDirection && targetSearchRotation == endSecondDirection)
        {
            targetSearchRotation = endFirstDirection;
            doneSearching = true;
        }

        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetSearchRotation, Time.deltaTime * searchLookSpeed);
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    void LessFrequentUpdate()
    {
        switch (scriptState.nmeCurrentState)
        {
            case SensorBotState.CurrentState.Stationary:
            case SensorBotState.CurrentState.Turning:
            case SensorBotState.CurrentState.Padding:
                break;
            case SensorBotState.CurrentState.Patroling:
                Invoke("IHaveBeenStuck", secondsAllowedStationary - secondsBetweenSlowerUpdate);
                break;
            case SensorBotState.CurrentState.Chasing:
            case SensorBotState.CurrentState.Firing:
                Invoke("IHaveBeenStuck", secondsAllowedStationary - secondsBetweenSlowerUpdate);
                PathIsClear(transCharacter.position, true, false);
                break;
        }
    }

    void IHaveBeenStuck()
    {
        if (!calculatingPath)
        {
            if (new Vector3(Round(myTransform.position.x, decimalRounding), Round(myTransform.position.y, decimalRounding), Round(myTransform.position.z, decimalRounding)) == new Vector3(Round(lastMyPosition.x, decimalRounding), Round(lastMyPosition.y, decimalRounding), Round(lastMyPosition.z, decimalRounding)))   //If we aren't moving, get a new path
            {
                //print("I'm stuck!");
                iAmStuck = true;
                //stuckCounter -= Time.deltaTime;
                //if (stuckCounter <= 0)
                //{
                //    stuckCounter = secondsAllowedStationary;
                //    return true;
                //}
            }
            else
            {
                iAmStuck = false;
                //    stuckCounter = secondsAllowedStationary;
            }
            //return false;
        }
        lastMyPosition = myTransform.position;

    }

    void PathIsClear(Vector3 target, bool characterReturns, bool distanceReturns)
    {
        RaycastHit hit;
        Vector3 direction = target - myTransform.position;
        if (Physics.SphereCast(myTransform.position, radiusOfCharControl, direction.normalized, out hit, Mathf.Infinity, groundMask.value))
        {
            //print(hit.transform.name);
            if (characterReturns && hit.collider.gameObject == goCharacter)
            {
                clearPath = true;
            }
            else if (distanceReturns && Vector3.Distance(myTransform.position, hit.transform.position) > Vector3.Distance(myTransform.position, target)) // If the distance between the hit object and the guard is more than the distance to the target
            {
                clearPath = true;
            }
            else
                clearPath = false;
        }
        else
            clearPath = true;
    }

}
