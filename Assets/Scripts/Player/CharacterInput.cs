using UnityEngine;
using System.Collections;

public class CharacterInput : MonoBehaviour {

    public float touchDistance = 5.0f;
    public float offsetDeltaTime = 100.0f;
    public float energyIncrement = 10.0f;
    public bool infraOn = false;
    public bool newScene = true;
    public Color coldColor = new Color(0.2627450980392157f, 0.0f, 1.0f);
    public Material matLukewarm;
    public float xTransferEnergy = 0.0f;


    private string objectDrain = "ObjectDrain";
    private Material[] aryOriginalMaterial;
    private GameObject[] aryLukewarmGO;
    private Transform transMainCam;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptThermometer;
    private Light[] aryLights;


	// Use this for initialization
	void Start () 
    {
        transMainCam = Camera.main.transform;
        scriptCharEnergy = GetComponent<CharacterEnergy>();
        scriptThermometer = GameObject.FindGameObjectWithTag("Thermometer").GetComponent<RoomHeatVariables>();
        if (!matLukewarm)
        {
            Debug.LogError("Assign default material in Inspector, please!");
        }
        else
        {
            matLukewarm.color = scriptThermometer.roomInfraTemp;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        GoInfrared();
        TouchDrain();
	}




    private void GoInfrared()
    {
        if (Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
            {
                if (newScene)
                {
                    aryLukewarmGO =  GameObject.FindGameObjectsWithTag("Lukewarm");
                    aryOriginalMaterial = new Material[aryLukewarmGO.Length];
                    for (int i = 0; i < aryLukewarmGO.Length; i++)
                    {
                        aryOriginalMaterial[i] = aryLukewarmGO[i].transform.renderer.material;
                    }
                    aryLights = FindObjectsOfType(typeof(Light)) as Light[];
                    newScene = false;
                }
                for (int i = 0; i < aryLukewarmGO.Length; i++)
                {
                    aryLukewarmGO[i].renderer.material = aryOriginalMaterial[i];
                }
                foreach (Light aLight in aryLights)
                {
                    aLight.enabled = true;
                }
                infraOn = false;
            }
            else
            {
                if (newScene)
                {
                    aryLukewarmGO = GameObject.FindGameObjectsWithTag("Lukewarm");
                    aryOriginalMaterial = new Material[aryLukewarmGO.Length];
                    for (int i = 0; i < aryLukewarmGO.Length; i++)
                    {
                        aryOriginalMaterial[i] = aryLukewarmGO[i].transform.renderer.material;
                    }
                    aryLights = FindObjectsOfType(typeof(Light)) as Light[];
                    newScene = false;
                }
                foreach (GameObject aGO in aryLukewarmGO)
                {

                    aGO.renderer.material = matLukewarm;
                }
                foreach (Light aLight in aryLights)
                {
                    aLight.enabled = false;
                }
                infraOn = true;
            }
        }
    }

    private void TouchDrain()
    {
        xTransferEnergy = 0.0f;                  // Reset xTransferEnergy
        if (Input.GetButton(objectDrain))       // If we're trying to drain energy
        {
            Ray ray = new Ray(transMainCam.position, transMainCam.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, touchDistance)) // and If we hit something with our raycast
            {
                Transform itsTransform = hit.transform;
                HeatControl tempHeatControl;
				SeeingBotHeatControl tempSeeingBotHeat;
                if ((tempHeatControl = itsTransform.GetComponent<HeatControl>()) != null) // and if it has a HeatControl script
				{
                    if ((itsTransform.tag == "Hot") && scriptCharEnergy.currentEnergy < 100.0f)  // and if it is a hot object and we are not already at max temperature
                    {
                        tempHeatControl.heatColor.H(HSBColor.FromColor(tempHeatControl.heatColor).h + (1 / tempHeatControl.xHeatEnergy) * Time.deltaTime, ref tempHeatControl.heatColor);
						tempHeatControl.xBeingTouched = true;
                        xTransferEnergy = energyIncrement * Time.deltaTime;
                    }
                    else if(itsTransform.tag == "Cold" && scriptCharEnergy.currentEnergy > 0.0f) // else if it is a cold object and we are not already at min temperature
                    {
                        tempHeatControl.heatColor.H(HSBColor.FromColor(tempHeatControl.heatColor).h - (1 / tempHeatControl.xHeatEnergy) * Time.deltaTime, ref tempHeatControl.heatColor);
						tempHeatControl.xBeingTouched = true;
                        xTransferEnergy = -energyIncrement * Time.deltaTime;
                    }
				}
                else if((tempSeeingBotHeat = itsTransform.GetComponent<SeeingBotHeatControl>()) != null)	// If it's a guard
				{
					if (HSBColor.FromColor(tempHeatControl.heatColor).h < HSBColor.FromColor(coldColor).h) 	// If they aren't already as cold as can be
	                {
	                    tempSeeingBotHeat.heatColor.H(HSBColor.FromColor(tempSeeingBotHeat.heatColor).h + (1 / tempSeeingBotHeat.xHeatEnergy) * Time.deltaTime, ref tempSeeingBotHeat.heatColor);
	                    xTransferEnergy = energyIncrement * Time.deltaTime;
	                }
				}
            }
        }
    }
}
