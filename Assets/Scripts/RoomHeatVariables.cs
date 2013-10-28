using UnityEngine;
using System.Collections;

public class RoomHeatVariables : MonoBehaviour {

    public bool tooHotOrCold = false;
    public float allowedPlayerTempVariance;
    public float temperatureMultiplier;
    public float maxStealthTemp;
    public float minStealthTemp;
    public float maxStealthHue;
    public float minStealthHue;
    public Color roomInfraTemp;

    private float hueCold = 255.0f / 360.0f;
    private CharacterEnergy scriptCharEnergy;
    private GameObject goCharacter;

	// Use this for initialization
	void Awake () 
    {
        goCharacter = GameObject.Find("Character");
        scriptCharEnergy = goCharacter.GetComponent<CharacterEnergy>();

        if (roomInfraTemp.a != 0)
        {
            if (allowedPlayerTempVariance != 0)
            {
                if (temperatureMultiplier != 0)
                {
                    maxStealthTemp = ((hueCold - (HSBColor.FromColor(roomInfraTemp).h)) / hueCold) * temperatureMultiplier + allowedPlayerTempVariance;
                    minStealthTemp = ((hueCold - (HSBColor.FromColor(roomInfraTemp).h)) / hueCold) * temperatureMultiplier - allowedPlayerTempVariance;
                    minStealthHue = (hueCold * (temperatureMultiplier - maxStealthTemp)) / temperatureMultiplier;           // Since the hues go up as temperature goes down, we must subtract from the temp multiplier to be on the same scale and we must use the opposite min/max
                    maxStealthHue = (hueCold * (temperatureMultiplier - minStealthTemp)) / temperatureMultiplier;

                    print("Max: " + maxStealthTemp + " Min: " + minStealthTemp + " HueMax: " + maxStealthHue + " HueMin: " + minStealthHue);
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
