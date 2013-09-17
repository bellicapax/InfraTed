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
        Turning = 4
    }

    private bool inTrigger;
    private EnemySight scriptSight;
    
    // Use this for initialization
	void Start () 
    {
        nmeCurrentState = CurrentState.Stationary;
        scriptSight = transform.parent.GetComponentInChildren<EnemySight>();
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
        else if (GameObject.FindGameObjectsWithTag("Hot").Length != 0)
        {
            nmeCurrentState = CurrentState.Patroling;
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
