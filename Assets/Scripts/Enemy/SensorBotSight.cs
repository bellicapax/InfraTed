using UnityEngine;
using System.Collections;

public class SensorBotSight : MonoBehaviour {
	
	public float fieldOfViewAngle = 110.0f;
    public float angle;
    public bool xPlayerInSight;
    public bool xPlayerIsTouching = false;
    public bool xPlayerHasTouched = false;
    
    public Vector3 personalLastSighting;
    public GameObject goRoomThermostat;
	public LayerMask xWallMask;
	
	private string playerTag = "Player";
    private Vector3 direction;
    private Vector3 previousSighting;
    private GameObject goCharacter;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;
    private Transform myTransform;
    private Transform transCharacter;
	private SphereCollider myCollider;


	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
		myCollider = GetComponent<SphereCollider>();

        if (goRoomThermostat)
        {
            scriptRoomHeat = goRoomThermostat.GetComponent<RoomHeatVariables>();
        }
        else
        {
            Debug.LogError("Room thermostat not assigned in Inspector.");
        }
        goCharacter = GameObject.Find("Character");
        transCharacter = goCharacter.transform;
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (xPlayerHasTouched)
        {
            xPlayerInSight = true;
            xPlayerHasTouched = false;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        HeatControl itsHeat;
        if (itsHeat = other.GetComponent<HeatControl>())
        {
            itsHeat.xInHeatSensorRange = true;
        }

    }

    void OnTriggerStay(Collider other)
    {
            if (scriptCharEnergy.currentEnergy >= 0)
            {
                if (other.tag == playerTag)
                {
                    //if (IsOutOfTemperatureThreshold(scriptCharEnergy.currentEnergy))	
                    //{
                        //print("Player is in the trigger and has energy.");
						RaycastHit hit;
						direction = (other.transform.position - myTransform.position);
						if(Physics.Raycast(myTransform.position, direction, out hit, myCollider.radius, xWallMask))		// This layermask doesn't allow it to go through the Walls layer or the player
						{
                            print("Hit  " + hit.transform.root.name + " Tran name " + transCharacter.name);
                            if (hit.transform.root.name == transCharacter.name)
                            {
                                xPlayerInSight = true;
                                print("They match");
                            }
                            else
                                xPlayerInSight = false;
						}
						else 
						{
							xPlayerInSight = false;
						}
                    }
                    //else if (!xPlayerIsTouching)
                    //{
                    //    xPlayerInSight = false;
                    //}
                //}
            }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == playerTag)
        {
            xPlayerInSight = false;
        }
        HeatControl itsHeat;
        if (itsHeat = other.GetComponent<HeatControl>())
        {
            itsHeat.xInHeatSensorRange = false;
        }
    }


    //public bool FieldOfView()
    //{
    //    if (!xPlayerIsTouching)
    //    {        
    //        //if the other conditions are not met, xPlayerInSight should be false
    //        xPlayerInSight = false;
    //    }

    //    if (scriptCharEnergy.currentEnergy >= 0)
    //    {
    //        direction = transCharacter.position - myTransform.position;
    //        angle = Vector3.Angle(direction, myTransform.forward);

    //        if (angle < fieldOfViewAngle * 0.5f)
    //        {
    //            RaycastHit hit;

    //            if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
    //            {
    //                if (hit.collider.gameObject == goCharacter)
    //                {
    //                    xPlayerInSight = true;
    //                    personalLastSighting = transCharacter.position; 
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    return false;
    //}

    bool IsOutOfTemperatureThreshold(float temp)
    {
        if (temp < scriptRoomHeat.minStealthTemp || temp > scriptRoomHeat.maxStealthTemp)
            return true;
        else
            return false;
    }

    public bool JustFOVAngle()
    {
        direction = transCharacter.position - myTransform.position;
        angle = Vector3.Angle(direction, myTransform.forward);

        if (angle < fieldOfViewAngle * 0.5f)
            return true;
        else
            return false;
    }
}
