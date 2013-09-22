using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    public Vector3 endingEuler;
    public float rotateSpeed = 10;
    public float alertedRotateSpeed = 20;

    private Quaternion endingQuat;
    private Quaternion startingQuat;
    private Quaternion targetRotation;
    private Transform myTransform;
    private GameObject goCharacter;
    private CameraSight scriptCamSight;

	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        startingQuat = myTransform.rotation;
        scriptCamSight = GetComponent<CameraSight>();
        if (endingEuler != Vector3.zero)
        {
            endingQuat = Quaternion.Euler(endingEuler);
        }
        else
        {
            Debug.Log("Assign camera's ending euler angles, please.");
        }
        targetRotation = endingQuat;
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
        Quaternion target = Quaternion.LookRotation(goCharacter.transform.position - myTransform.position);
        target = Quaternion.Euler(myTransform.rotation.eulerAngles.x, target.eulerAngles.y, myTransform.rotation.eulerAngles.z); // Keep the X & Z rotation the same (in Euler Angles)
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target, Time.deltaTime * alertedRotateSpeed);
    }

}
