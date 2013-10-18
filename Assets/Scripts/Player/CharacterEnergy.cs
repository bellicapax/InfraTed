using UnityEngine;
using System.Collections;

public class CharacterEnergy : MonoBehaviour {

    public float currentEnergy = 21.0f;
    public float energyDecrement = 0.1f;
    public float normalDecrement = 0.1f;
    public float sprintDecrement = 2.0f;
    public float absoluteMaxSpeed = 6.0f;

    private bool sprinting = false;
    private CharacterInput scriptCharInput;
    private CharacterMotor scriptCharMotor;

	// Use this for initialization
	void Start () 
    {
        scriptCharInput = GetComponent<CharacterInput>();
        scriptCharMotor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        LoseHeat();
        GainHeat();
        Mathf.Clamp(currentEnergy, 0.0f, 100.0f);
        HeatAndSpeed();
        AssessHealth();
        
	}

    private void LoseHeat()
    {
        if(Input.GetButton("Sprint"))
        {
            energyDecrement = sprintDecrement;
            sprinting = true;
        }
        else
        {
            energyDecrement = normalDecrement;
            sprinting = false;
        }
        currentEnergy -= energyDecrement * Time.deltaTime;
    }

    private void GainHeat()
    {
        currentEnergy += scriptCharInput.xTransferEnergy;        //Add the energy to ours
    }

    private void AssessHealth()
    {
        if (currentEnergy < 0.0f)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void HeatAndSpeed()
    {
        if (sprinting)
            scriptCharMotor.movement.maxForwardSpeed = absoluteMaxSpeed;
        else
            scriptCharMotor.movement.maxForwardSpeed = ((absoluteMaxSpeed * 3 / 4 * currentEnergy) / (0.5f * currentEnergy + 25.0f));
    }


}
