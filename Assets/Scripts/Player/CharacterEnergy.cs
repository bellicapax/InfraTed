using UnityEngine;
using System.Collections;

public class CharacterEnergy : MonoBehaviour {

    public float currentEnergy = 21.0f;
    public float energyDecrement = 1.0f;

    private CharacterInput scriptCharInput;
    private CharacterMotor scriptCharMotor;

	// Use this for initialization
	void Start () 
    {
        scriptCharInput = GetComponent<CharacterInput>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        LoseHeat();
        GainHeat();
        AssessHealth();
	}

    void LateUpdate()
    {
        Mathf.Clamp(currentEnergy, 0.0f, 100.0f);
    }

    private void LoseHeat()
    {
        currentEnergy -= energyDecrement * Time.deltaTime;
    }

    private void GainHeat()
    {
        currentEnergy += scriptCharInput.transferEnergy;        //Add the energy to ours
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

    }


}
