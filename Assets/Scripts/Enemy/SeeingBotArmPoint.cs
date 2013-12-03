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

	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        originalRotation = myTransform.localRotation;
        scriptState = GameObject.Find(seeGuard).GetComponentInChildren<EnemyState>();
        charTrans = GameObject.Find("Character").transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (scriptState.nmeCurrentState == EnemyState.CurrentState.Firing)
        {
            //myTransform.LookAt(charTrans);
            Quaternion rot = Quaternion.LookRotation(charTrans.position - myTransform.position);
            //myTransform.rotation = rot * SeeingBotGunRotation.gunOffset;
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, rot * SeeingBotGunRotation.gunOffset, rotateSpeedWithAnimation * Time.deltaTime);
            //myTransform.rotation *= SeeingBotGunRotation.gunOffset;
        }
        else
        {
            //myTransform.localRotation = originalRotation;
            myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation, originalRotation, rotateSpeed * Time.deltaTime);
        }

	}
}
