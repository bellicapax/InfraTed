using UnityEngine;
using System.Collections;

public class SensorBotSight : MonoBehaviour {
	
	public float fieldOfViewAngle = 110.0f;
    public float angle;
    public bool playerInSight;
    public bool playerIsTouching = false;
    public bool playerHasTouched = false;
    
    public Vector3 personalLastSighting;
    public Vector3 direction;
    public GameObject goRoomThermostat;
	public LayerMask xWallMask;
	
	private string playerTag = "Player";
    private Vector3 previousSighting;
    private GameObject goCharacter;
    private GameObject goEnemySharedVars;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;
    private EnemyShared scriptShared;
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
        goEnemySharedVars = GameObject.Find("EnemySharedVariables");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();
        scriptShared = goEnemySharedVars.GetComponent<EnemyShared>();
        if (!scriptShared)
            Debug.Log("Unable to access EnemyShared script from EnemySight script");
	}
	
	// Update is called once per frame
	void Update () 
    {
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
        if (itsHeat = other.GetComponent<HeatControl>())
        {
            itsHeat.xInHeatSensorRange = true;
        }

    }

    void OnTriggerStay(Collider other)
    {
            if (scriptCharEnergy.currentEnergy >= 0)
            {
                if (other.tag == "Player")
                {
                    if (IsOutOfTemperatureThreshold(scriptCharEnergy.currentEnergy))	
					{
						RaycastHit hit;
						direction = (other.transform.position - myTransform.position);
						if(Physics.Raycast(myTransform.position, direction, out hit, myCollider.radius, xWallMask))		// This layermask doesn't allow it to go through the Walls layer or the player
						{
							if(hit.transform.tag == playerTag)
		                    	playerInSight = true;
							else 
								playerInSight = false;
						}
						else 
						{
							playerInSight = false;
						}
                    }
                    else if (!playerIsTouching)
                    {
                        playerInSight = false;
                    }
                }
            }
    }

    void OnTriggerExit(Collider other)
    {
        HeatControl itsHeat;
        if (itsHeat = other.GetComponent<HeatControl>())
        {
            itsHeat.xInHeatSensorRange = false;
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
            direction = transCharacter.position - myTransform.position;
            angle = Vector3.Angle(direction, myTransform.forward);

            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;

                if (Physics.Raycast(myTransform.position, direction.normalized, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == goCharacter)
                    {
                        playerInSight = true;
                        personalLastSighting = transCharacter.position;   //Update this so that one script has the position for all to reference
                        scriptShared.sharedLastKnownLocation = transCharacter.position;
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
        direction = transCharacter.position - myTransform.position;
        angle = Vector3.Angle(direction, myTransform.forward);

        if (angle < fieldOfViewAngle * 0.5f)
            return true;
        else
            return false;
    }
}
