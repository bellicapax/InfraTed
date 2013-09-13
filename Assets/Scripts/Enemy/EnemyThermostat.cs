using UnityEngine;
using System.Collections;

public class EnemyThermostat : MonoBehaviour {

    public GameObject goRoomThermostat;
    public bool tooHotOrCold = false;

    private GameObject goCharacter;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptRoomHeat;

	// Use this for initialization
	void Start () 
    {
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
	void Update () {
        if (scriptCharEnergy.currentEnergy > scriptRoomHeat.maxStealthTemp || scriptCharEnergy.currentEnergy < scriptRoomHeat.minStealthTemp)
        {
            tooHotOrCold = true;
        }
        else
        {
            tooHotOrCold = false;
        }
	}
}
