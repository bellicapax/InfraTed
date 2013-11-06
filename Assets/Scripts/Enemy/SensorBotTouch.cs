using UnityEngine;
using System.Collections;

public class SensorBotTouch : MonoBehaviour {

    private EnemySight scriptSight;
    private SensorBotMovement scriptMovement;

    // Use this for initialization
    void Start()
    {
        scriptSight = transform.parent.GetComponentInChildren<EnemySight>();
        scriptMovement = transform.parent.GetComponent<SensorBotMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            scriptSight.xPlayerIsTouching = true;
            scriptSight.xPlayerInSight = true;
            scriptSight.xPlayerHasTouched = true;
        }
        else if (other.transform == scriptMovement.xCurrentHotColdTrans)
        {
            scriptMovement.newPatrolPath = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            scriptSight.xPlayerIsTouching = false;
        }
    }

}
