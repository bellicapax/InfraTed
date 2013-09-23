using UnityEngine;
using System.Collections;

public class CameraSight : MonoBehaviour {

    public bool canSeePlayer;

    private float maxVertAngle;
    private float minVertAngle;
    private float halfSpotAngle;
    private float angle;
    private Vector3 direction;
    private Transform myTransform;
    private GameObject goCharacter;
    private Light myLight;

	// Use this for initialization
	void Start () 
    {
        myLight = GetComponent<Light>();
        myTransform = this.transform;
        goCharacter = GameObject.Find("Character");
        halfSpotAngle = myLight.spotAngle / 2;
        maxVertAngle = myTransform.localEulerAngles.x + 90.0f + halfSpotAngle;  // X at 0 is 90 degrees from Vector3.up.  We need to add the Euler x and 90 to half of the spotangle.
        minVertAngle = myTransform.localEulerAngles.x + 90.0f - halfSpotAngle;
	}
	
	// Update is called once per frame
	void Update () 
    {
        FieldOfView();
	}

    void FieldOfView()
    {
        direction =  goCharacter.transform.position - myTransform.position;
        angle = Vector3.Angle(direction, Vector3.up);
        if (angle < maxVertAngle && angle > minVertAngle)
        {
            angle = Vector3.Angle(direction, myTransform.forward);
            if (angle < halfSpotAngle)                                  // If it's within the camera's circle view (both vertically and horizontally)
            {
                RaycastHit hit;

                if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == goCharacter)         // If we don't hit anything besides the character.
                    {
                        canSeePlayer = true;
                    }
                }
            }
        }
        else
            canSeePlayer = false;
    }
}
