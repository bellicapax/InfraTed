using UnityEngine;
using System.Collections;

public class RoomHeatVariables : MonoBehaviour {

    public bool tooHotOrCold = false;
    public float allowedPlayerTempVariance;
    public float temperatureMultiplier;
    public float maxStealthTemp;
    public float minStealthTemp;
    public Color roomInfraTemp;

    private float hueCold = 255.0f / 360.0f;
    private CharacterInput scriptCharInput;
    private CharacterEnergy scriptCharEnergy;
    private GameObject goCharacter;

	// Use this for initialization
	void Start () 
    {
        goCharacter = GameObject.Find("Character");
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();

        if (roomInfraTemp.a != 0)
        {
            scriptCharInput.ambientInfra = roomInfraTemp;
            if (allowedPlayerTempVariance != 0)
            {
                if (temperatureMultiplier != 0)
                {
                    maxStealthTemp = ((hueCold - (HSBColor.FromColor(roomInfraTemp).h)) / hueCold) * temperatureMultiplier + allowedPlayerTempVariance;
                    minStealthTemp = ((hueCold - (HSBColor.FromColor(roomInfraTemp).h)) / hueCold) * temperatureMultiplier - allowedPlayerTempVariance;
                }
                else
                    Debug.LogError("Temperature multiplier not assigned!");
            }
        }
        else
            Debug.LogError("Room temperature Color not assigned!  (or alpha is 0)");
	}

    void Update()
    {
        if (scriptCharEnergy.currentEnergy > maxStealthTemp || scriptCharEnergy.currentEnergy < minStealthTemp)
        {
            tooHotOrCold = true;
        }
        else
        {
            tooHotOrCold = false;
        }
    }
}
