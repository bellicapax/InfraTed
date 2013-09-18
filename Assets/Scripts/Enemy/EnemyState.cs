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
        Padding = 5
    }

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
        if (scriptSight.playerInSight)
        {
            if (inTrigger)
            {
                if (scriptSight.FieldOfView())
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
