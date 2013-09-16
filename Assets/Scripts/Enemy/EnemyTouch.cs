﻿using UnityEngine;
using System.Collections;

public class EnemyTouch : MonoBehaviour {

    private GameObject lastGO = null;
    private EnemySight scriptSight;
    private EnemyMovement scriptMovement;

	// Use this for initialization
	void Start () 
    {
        scriptSight = transform.parent.GetComponent<EnemySight>();
        scriptMovement = transform.parent.GetComponent<EnemyMovement>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            scriptSight.playerIsTouching = true;
            scriptSight.playerInSight = true;
            scriptSight.playerHasTouched = true;
        }
        else if ((other.tag == "Hot" || other.tag == "Cold") && other.gameObject != lastGO)
        {
            scriptMovement.newPatrolPath = true;
            lastGO = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Character")
        {
            scriptSight.playerIsTouching = false;
        }
    }

}
