using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    private Rigidbody myRigidbody;
    private GameObject goChar;
    private HingeJoint myJoint;
    

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
        myJoint = GetComponent<HingeJoint>();
        goChar = GameObject.Find("Character");

        Vector3 direction = (new Vector3(transform.position.x, 0.0f, transform.position.z) - new Vector3(goChar.transform.position.x, 0.0f, goChar.transform.position.z));
        Vector3 perpendicularAxis = Vector3.Cross(direction, Vector3.up);
        myJoint.axis = perpendicularAxis;
        myRigidbody.AddForce(-goChar.transform.forward * 100.0f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
