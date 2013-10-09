using UnityEngine;
using System.Collections;

public class EnemyState : MonoBehaviour {

    public CurrentState nmeCurrentState;
    public enum CurrentState
    {
        Stationary = 0,
        Patroling = 1,
        Chasing = 2,
        Firing = 3,
        Turning = 4,
        Padding = 5,
        Searching = 6
    }
    public bool justLostEm;
    public bool sensingRobotsSearch;

    private bool inTrigger;
    private EnemySight scriptSight;
    private EnemyMovement scriptMovement;

    // Use this for initialization
	void Start () 
    {
        nmeCurrentState = CurrentState.Stationary;
        scriptSight = transform.parent.GetComponentInChildren<EnemySight>();
        scriptMovement = transform.parent.GetComponent<EnemyMovement>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if ((nmeCurrentState == CurrentState.Chasing || nmeCurrentState == CurrentState.Firing) && !scriptSight.playerInSight) // If our last state was with the player in sight and now the player is not in sight, we need to search
        {
            if(!scriptSight.useSphericalHeatSensor || sensingRobotsSearch) // If we aren't using the sphere collider OR we are letting the heat sensors search
                justLostEm = true;
        }
        if (scriptSight.playerInSight)
        {
            justLostEm = false;                             // If we can see the player, we don't need to search anymore.
            if (inTrigger)
            {
                if (scriptSight.JustFOVAngle())
                {
                    nmeCurrentState = CurrentState.Firing;
                }
                else
                {
                    nmeCurrentState = CurrentState.Turning;
                }
            }
            else
            {
                nmeCurrentState = CurrentState.Chasing;
            }
        }
        else if (scriptSight.playerHasTouched)
        {
            nmeCurrentState = CurrentState.Turning;
        }
        else if (justLostEm)
        {
            nmeCurrentState = CurrentState.Searching;
        } 
        else if (scriptMovement.listTransPatrol.Count > 1)
        {
            nmeCurrentState = CurrentState.Patroling;
        }
        else if (scriptMovement.listTransPatrol.Count == 1)
        {
            nmeCurrentState = CurrentState.Padding;
        }
        else
        {
            nmeCurrentState = CurrentState.Stationary;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            inTrigger = false;
        }
    }
}
