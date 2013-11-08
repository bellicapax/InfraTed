using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {

    public float fieldOfViewAngle = 110.0f;
    public float angle;
    public bool xPlayerInSight;
    public bool xPlayerIsTouching = false;
    public bool xPlayerHasTouched = false;
    
    public Vector3 personalLastSighting;
    public GameObject goRoomThermostat;

    private string strPlayer = "Player";
    private Vector3 direction;
    private Vector3 previousSighting;
    private GameObject goCharacter;
    private GameObject goEnemySharedVars;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;
    private EnemyShared scriptShared;
    private Transform myTransform;
    private Transform transCharacter;
    private Transform transHead;


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
        transCharacter = goCharacter.transform;
        transHead = GameObject.FindWithTag("Head").transform;
        goEnemySharedVars = GameObject.Find("EnemySharedVariables");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();
        scriptShared = goEnemySharedVars.GetComponent<EnemyShared>();
        if (!scriptShared)
            Debug.Log("Unable to access EnemyShared script from EnemySight script");
	}
	
	// Update is called once per frame
	void Update () 
    {
        FieldOfView();

        if (xPlayerHasTouched)
        {
            if (FieldOfView())
            {
                xPlayerHasTouched = false;
            }
        }
	}

    public bool FieldOfView()
    {
        if (!xPlayerIsTouching)
        {        
            //if the other conditions are not met, xPlayerInSight should be false
            xPlayerInSight = false;
        }

        if (scriptCharEnergy.currentEnergy >= 0)
        {
            direction = transCharacter.position - transHead.position;        // We want to apply both vertical and horizontal constraints on the angle, so use the position of the head and not just the gameObject
            angle = Vector3.Angle(direction, transHead.forward);

            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                //print("Angle checks out"); // DBGR
                if (Physics.Raycast(transHead.position, direction.normalized, out hit, Mathf.Infinity))
                {
                    //print("Hit Something with my FOV Raycast"); // DBGR
                    if (hit.collider.tag == strPlayer)
                    {
                        xPlayerInSight = true;
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
