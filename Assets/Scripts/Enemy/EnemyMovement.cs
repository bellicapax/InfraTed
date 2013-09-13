using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public float enemySpeed = 1.0f;
    public float enemyRotateSpeed = 1.0f;

    private GameObject goCharacter;
    private EnemySight scriptEnemySight;
    private Rigidbody myRigidbody;
    private Transform myTransform;


	// Use this for initialization
	void Start () 
    {
        myRigidbody = this.rigidbody;
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
        scriptEnemySight = GetComponent<EnemySight>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (scriptEnemySight.playerInSight)
        {
            myRigidbody.MovePosition(rigidbody.position + (new Vector3(goCharacter.transform.position.x, 0.0f, goCharacter.transform.position.z) - rigidbody.position).normalized * Time.deltaTime * enemySpeed);
            //myTransform.LookAt(goCharacter.transform.position);
            Quaternion target = Quaternion.LookRotation(scriptEnemySight.direction);
            //target.x = 0.0f;
            //target.z = 0.0f;
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target, Time.deltaTime * enemyRotateSpeed);
        }
	}
}
