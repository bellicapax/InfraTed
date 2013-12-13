using UnityEngine;
using System.Collections;

public class SeeingBotArmPoint : MonoBehaviour {

    public float rotateSpeedWithAnimation = 15.0f;
    public float rotateSpeed = 10.0f;

    private string seeGuard = "Seeing Guard";
    private Quaternion originalRotation;
    private Transform myTransform;
    private Transform charTrans;
    private EnemyState scriptState;
    private EnemyMovement scriptMovement;

	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        originalRotation = myTransform.localRotation;
        scriptState = GameObject.Find(seeGuard).GetComponentInChildren<EnemyState>();
        scriptMovement = GameObject.Find(seeGuard).GetComponentInChildren<EnemyMovement>();
        charTrans = GameObject.Find("Character").transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (scriptState.nmeCurrentState == EnemyState.CurrentState.Firing && !scriptMovement.xIAmFrozen)
        {
            // Get the rotation for looking at the player
            Quaternion rot = Quaternion.LookRotation(charTrans.position - myTransform.position);

            // Smoothly transition to that rotation
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, rot * SeeingBotGunRotation.gunOffset, rotateSpeedWithAnimation * Time.deltaTime);
        }
        else
        {
            // Smoothly transition back to the original rotation
            myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation, originalRotation, rotateSpeed * Time.deltaTime);
        }

	}
}
