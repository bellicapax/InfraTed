using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    public Vector3 endingEuler;
    public int numberOfGuardsToSpawn;
    public float rotateSpeed = 10;
    public float alertedRotateSpeed = 20;
    public float secondsTillDetection = 3.0f;
    public float guardSpawnWaitTime = 1.0f;
    public Transform transGuardEntrance;
    public GameObject goSharedVariables;
    public GameObject goGuard;

    private Quaternion endingQuat;
    private Quaternion startingQuat;
    private Quaternion targetRotation;
    private Transform myTransform;
    private GameObject goCharacter;
    private CameraSight scriptCamSight;
    private EnemyShared scriptShared;

	// Use this for initialization
	void Start () 
    {
        if (endingEuler != Vector3.zero)
        {
            endingQuat = Quaternion.Euler(endingEuler);
            targetRotation = endingQuat;
        }
        else
        {
            Debug.Log("Assign camera's ending euler angles in Camera Movement, please.");
        }
        if (!goSharedVariables)
        {
            Debug.Log("Assign the EnemySharedVariables object in the Camera Movement script, please.");
        }
        else
        {
            scriptShared = goSharedVariables.GetComponent<EnemyShared>();
        }
        if (!goGuard)
        {
            Debug.Log("Assign the Guard prefab in the Camera Movement script, please.");
        }
        if (!transGuardEntrance)
        {
            Debug.Log("Assign the guard's entrance transform in the Camera Movement script, please.");
        }
        myTransform = this.transform;
        startingQuat = myTransform.rotation;
        scriptCamSight = GetComponent<CameraSight>();
        goCharacter = GameObject.Find("Character");

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!scriptCamSight.canSeePlayer)
        {
            SweepArea();
        }
        else
        {
            TrackPlayer();
            StartCoroutine(DetectionCountdown());
        }
	}

    void SweepArea()
    {
        if (myTransform.rotation == startingQuat && targetRotation == startingQuat)
        {
            targetRotation = endingQuat;
        }
        else if (myTransform.rotation == endingQuat && targetRotation == endingQuat)
        {
            targetRotation = startingQuat;
        }
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }

    void TrackPlayer()
    {
        scriptShared.sharedLastKnownLocation = goCharacter.transform.position;      // Update the enemySharedVariables

        Quaternion target = Quaternion.LookRotation(goCharacter.transform.position - myTransform.position);
        target = Quaternion.Euler(myTransform.rotation.eulerAngles.x, target.eulerAngles.y, myTransform.rotation.eulerAngles.z); // Keep the X & Z rotation the same (in Euler Angles)
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target, Time.deltaTime * alertedRotateSpeed);
    }

    IEnumerator DetectionCountdown()
    {
        yield return new WaitForSeconds(secondsTillDetection);
        if (!scriptShared.cameraSummonedGuards)
        {
            StartCoroutine(SpawnGuards());
        }
        
    }

    IEnumerator SpawnGuards()
    {
        scriptShared.cameraSummonedGuards = true;
        for (int i = 0; i < numberOfGuardsToSpawn; i++)
        {
            GameObject clone = GameObject.Instantiate(goGuard, transGuardEntrance.position, transGuardEntrance.rotation) as GameObject;
            EnemyState scriptState = clone.GetComponentInChildren<EnemyState>();
            scriptState.justLostEm = true;
            yield return new WaitForSeconds(guardSpawnWaitTime);
        }
    }

}
