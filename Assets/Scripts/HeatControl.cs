using UnityEngine;
using System.Collections;

[RequireComponent(typeof (MeshVolume))]

public class HeatControl : MonoBehaviour {

    public bool inHeatSensorRange = false;
    public Color heatColor;
    public float heatEnergy;
    public Material matNormal;
    public Material matInfra;

    private float heatMultiplier;
    private string lukewarm = "Lukewarm";
    private string hot = "Hot";
    private string cold = "Cold";
    private HSBColor coldHSB;
    private GameObject goCharacter;
    private GameObject goRoomThermo;
    private CharacterInput scriptCharInput;
    private MeshVolume scriptMesh;
    private RoomHeatVariables scriptThermo;
    private Transform myTransform;
    private bool infraOn = false;
    
    // Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        if (matInfra)
        {
            heatColor = matInfra.color;
        }
        else
        {
            Debug.LogError("Infrared material not assigned in the Inspector!");
        }
        if (!matNormal)
        {
            Debug.LogError("Normal material not assigned in the Inspector!");
        }

        goCharacter = GameObject.Find("Character");

        goRoomThermo = GameObject.FindGameObjectWithTag("Thermometer");

        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        
        scriptMesh = GetComponent<MeshVolume>();

        scriptThermo = goRoomThermo.GetComponent<RoomHeatVariables>();

        coldHSB = HSBColor.FromColor(scriptCharInput.coldColor);

        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
 
        heatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * scriptMesh.volume;
	}
	
	// Update is called once per frame
	void Update () 
    {
        MaterialSwap();
        EnergyAndColor();
        RefreshTag();
	}

    private void EnergyAndColor()
    {
        if (HSBColor.FromColor(myTransform.renderer.material.color).h <= coldHSB.h)
        {
            if (infraOn)
            {
                myTransform.renderer.material.color = heatColor;
            }
        }
        else
        {
            if (infraOn)
            {
                myTransform.renderer.material.color = scriptCharInput.coldColor;
            }
        }
    }

    private void MaterialSwap()
    {
        if(Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
            {
                myTransform.renderer.material = matNormal;
                infraOn = false;
            }
            else
            {
                myTransform.renderer.material = matInfra;
                infraOn = true;
            }
        }
    }

    private void RefreshTag()
    {
        HSBColor tempHSB = HSBColor.FromColor(heatColor);

        if (tempHSB.h < scriptThermo.minStealthHue)             // Since our hotter hues are smaller than our cooler ones, we must be below the min to be hot
        {
            this.tag = hot;
        }

        else if(tempHSB.h > scriptThermo.maxStealthHue)         // Since our colder hues are larger than our hotter ones, we must be above the max to be cold
        {
            this.tag = cold;
        }

        else if (tempHSB.h <= scriptThermo.maxStealthHue && tempHSB.h >= scriptThermo.minStealthHue)
        {
            this.tag = lukewarm;
        }
    }
}
