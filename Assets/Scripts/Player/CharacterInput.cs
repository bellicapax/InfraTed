﻿using UnityEngine;
using System.Collections;

public class CharacterInput : MonoBehaviour {

    public float touchDistance = 5.0f;
    public float offsetDeltaTime = 100.0f;
    public float transferEnergy = 0.0f;
    public float energyIncrement = 10.0f;
    public bool infraOn = false;
    public bool newScene = true;
    public Color ambientVisible;
    public Color ambientInfra;
    public Color coldColor = new Color(0.2627450980392157f, 0.0f, 1.0f);
    public Material defaultDiffuse;

    private string objectDrain = "ObjectDrain";
    private Material[] aryOriginalMaterial;
    private GameObject[] aryLukewarmGO;
    private Transform transMainCam;
    private CharacterEnergy scriptCharEnergy;
    private Light[] aryLights;


	// Use this for initialization
	void Start () 
    {
        transMainCam = Camera.main.transform;
        ambientVisible = RenderSettings.ambientLight;
        scriptCharEnergy = GetComponent<CharacterEnergy>();
        if (!defaultDiffuse)
        {
            Debug.LogError("Assign default material in Inspector, please!");
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
                RenderSettings.ambientLight = ambientVisible;
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
                RenderSettings.ambientLight = ambientInfra;
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

                    aGO.renderer.material = defaultDiffuse;
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
        transferEnergy = 0.0f;                  // Reset transferEnergy
        if (Input.GetButton(objectDrain))       // If we're trying to drain energy
        {
            Ray ray = new Ray(transMainCam.position, transMainCam.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, touchDistance)) // and If we hit something with our raycast
            {
                Transform itsTransform = hit.transform;
                HeatControl tempHeatControl;
                if ((tempHeatControl = itsTransform.GetComponent<HeatControl>()) != null) // and if it has a HeatControl script
                {

                    if (itsTransform.tag == "Hot" && scriptCharEnergy.currentEnergy < 100.0f)  // and if it is a hot object and we are not already at max temperature
                    {
                        HSBColor tempHSB = HSBColor.FromColor(tempHeatControl.heatColor);
                        tempHSB.h += (1 / tempHeatControl.heatEnergy) * Time.deltaTime;
                        transferEnergy = energyIncrement * Time.deltaTime;
                        tempHeatControl.heatColor = HSBColor.ToColor(tempHSB);
                    }
                    else if(itsTransform.tag == "Cold" && scriptCharEnergy.currentEnergy > 0.0f) // else if it is a cold object and we are not already at min temperature
                    {
                        HSBColor tempHSB = HSBColor.FromColor(tempHeatControl.heatColor);
                        tempHSB.h -= (1 / tempHeatControl.heatEnergy) * Time.deltaTime;
                        transferEnergy = -energyIncrement * Time.deltaTime;
                        tempHeatControl.heatColor = HSBColor.ToColor(tempHSB);
                    }
                }
            }
        }
    }
}
