using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {

    public float fieldOfViewAngle = 110.0f;
    public float angle;
    public bool playerInSight;
    public bool useFieldOfVision;
    public bool playerIsTouching = false;
    public bool playerHasTouched = false;
    public bool useSphericalHeatSensor;
    public Vector3 personalLastSighting;
    public Vector3 direction;
    public GameObject goRoomThermostat;

    private Vector3 previousSighting;
    private GameObject goCharacter;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;
    private Transform myTransform;


	// Use this for initialization
	void Start () 
    {
        myTransform = this.transform;

        if (goRoomThermostat)
        {
            scriptRoomHeat = goRoomThermostat.GetComponent<RoomHeatVariables>();
        }
        else
        {
            Debug.LogError("Room thermostat not assigned in Inspector.");
        }
        goCharacter = GameObject.Find("Character");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (useFieldOfVision)
        {
            FieldOfView();
        }
        if (playerHasTouched)
        {
            if (FieldOfView())
            {
                playerHasTouched = false;
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        HeatControl itsHeat;
        if (useSphericalHeatSensor && (itsHeat = other.GetComponent<HeatControl>()))
        {
            itsHeat.inHeatSensorRange = true;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (useSphericalHeatSensor)
        {
            if (scriptCharEnergy.currentEnergy >= 0)
            {
                if (other.name == "Character")
                {
                    if (IsOutOfTemperatureThreshold(scriptCharEnergy.currentEnergy))
                    {
                        playerInSight = true;
                        personalLastSighting = goCharacter.transform.position;
                    }
                    else if (!playerIsTouching)
                    {
                        playerInSight = false;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        HeatControl itsHeat;
        if (useSphericalHeatSensor && (itsHeat = other.GetComponent<HeatControl>()))
        {
            itsHeat.inHeatSensorRange = false;
        }
    }


    public bool FieldOfView()
    {
        if (!playerIsTouching)
        {        
            //if the other conditions are not met, playerInSight should be false
            playerInSight = false;
        }

        if (scriptCharEnergy.currentEnergy >= 0)
        {
            direction = goCharacter.transform.position - myTransform.position;
            angle = Vector3.Angle(direction, myTransform.forward);

            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;

                if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == goCharacter)
                    {
                        playerInSight = true;
                        personalLastSighting = goCharacter.transform.position;   //Update this so that one script has the position for all to reference
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool IsOutOfTemperatureThreshold(float temp)
    {
        if (temp < scriptRoomHeat.minStealthTemp || temp > scriptRoomHeat.maxStealthTemp)
            return true;
        else
            return false;
    }

    public bool JustFOVAngle()
    {
        direction = goCharacter.transform.position - myTransform.position;
        angle = Vector3.Angle(direction, myTransform.forward);

        if (angle < fieldOfViewAngle * 0.5f)
            return true;
        else
            return false;
    }
}
