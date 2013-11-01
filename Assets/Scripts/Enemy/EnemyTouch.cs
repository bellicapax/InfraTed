using UnityEngine;
using System.Collections;

public class EnemyTouch : MonoBehaviour {

    private GameObject lastGO = null;
    private EnemySight scriptSight;
    private SensorBotMovement scriptMovement;

	// Use this for initialization
	void Start () 
    {
        scriptSight = transform.parent.GetComponentInChildren<EnemySight>();
        scriptMovement = transform.parent.GetComponent<SensorBotMovement>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (scriptMovement.changedStates)
        {
            lastGO = null;
        }
        if (other.tag == "Player")
        {
            scriptSight.playerIsTouching = true;
            scriptSight.playerInSight = true;
            scriptSight.playerHasTouched = true;
        }
        else if ((other.tag == "Hot" || other.tag == "Cold") && other.gameObject != lastGO)
        {
            scriptMovement.newPatrolPath = true;
            scriptMovement.nameTouch = other.name;
            lastGO = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            scriptSight.playerIsTouching = false;
        }
        else if ((other.tag == "Hot" || other.tag == "Cold") && other.gameObject != lastGO)
        {
            scriptMovement.nameTouch = "";
        }
    }

}
