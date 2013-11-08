using UnityEngine;
using System.Collections;

public class SeeingBotArmPoint : MonoBehaviour {

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
            myTransform.LookAt(charTrans);
            myTransform.rotation *= SeeingBotGunRotation.gunOffset;
        }
        else
            myTransform.localRotation = originalRotation;
	}
}
