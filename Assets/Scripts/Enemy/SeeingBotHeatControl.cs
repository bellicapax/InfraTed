using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof (MeshVolume))]

public class SeeingBotHeatControl : MonoBehaviour {

    public Color heatColor;
    public float secondsTillThaw = 5.0f;
    public bool xInHeatSensorRange = false;
    public bool xBeingTouched = false;
    public float xHeatEnergy;
    public Color xFrozenColor;
    public Color xOriginalColor;

    private float heatMultiplier;
    private float heatHomeostasisRate = 4;
    private float thawCounter;
	private string objectDrain = "ObjectDrain";
    private Color visibleLightColor;
    private HSBColor coldHSB;
    private Renderer[] myRenderers;
    private GameObject goCharacter;
    public GameObject goRoomThermo;
    private CharacterInput scriptCharInput;
    private MeshVolume scriptMesh;
    private RoomHeatVariables scriptThermo;
    private Transform myTransform;
    private bool infraOn = false;
    
    // Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        myRenderers = new Renderer[myTransform.GetComponentsInChildren<Renderer>().Length];
        myRenderers = myTransform.GetComponentsInChildren<Renderer>();
        visibleLightColor = myRenderers[0].material.color;
		xOriginalColor = heatColor;

        goCharacter = GameObject.Find("Character");
        goRoomThermo = GameObject.FindGameObjectWithTag("Thermometer");
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptMesh = GetComponent<MeshVolume>();
        scriptThermo = goRoomThermo.GetComponent<RoomHeatVariables>();
        coldHSB = HSBColor.FromColor(scriptCharInput.xColdColor);
        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
        xHeatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * scriptMesh.volume;
        StartCoroutine(AssignColor());
	}
	
	// Update is called once per frame
	void Update () 
    {
        CheckForInput();
        RegainHeat();
        EnergyAndColor();
	}

    private void CheckForInput()
    {
        if (Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
                infraOn = false;
            else
                infraOn = true;
        }
		if(Input.GetButtonUp(objectDrain))
			xBeingTouched = false;
    }

    private void EnergyAndColor()
    {
        if (infraOn)
        {
            if (HSBColor.FromColor(heatColor).h <= coldHSB.h)
            {
				foreach(Renderer r in myRenderers)
				{
					r.material.color = heatColor;
				}
			}
            else
			{
				foreach(Renderer r in myRenderers)
				{
                    r.material.color = scriptCharInput.xColdColor;
                }
            }
        }
		else
		{
			foreach(Renderer r in myRenderers)
			{
				r.material.color = visibleLightColor;
			}
		}
    }

    private void RegainHeat()
    {
        if (!xBeingTouched)  // If it's not currently being drained
        {
            if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(xOriginalColor).h) // If the guard's heat is colder than the original color
            {
                if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(xFrozenColor).h)  // If the guard is actually frozen and not just colder
                {
                    if (thawCounter >= secondsTillThaw)
                    {
                        heatColor.H(HSBColor.FromColor(heatColor).h - (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
                        print("Heating up but still frozen");
                    }
                    else
                        thawCounter += Time.deltaTime;
                }
                else // Else we are colder than our original temperature, but not frozen, so heat back up and reset the thaw counter
                {
                    thawCounter = 0.0f;
                    heatColor.H(HSBColor.FromColor(heatColor).h - (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
                    //print("Heating up NOT frozen. Heat hue: " + HSBColor.FromColor(heatColor).h + " Original hue: " + HSBColor.FromColor(xOriginalColor).h);
                }
            }
        }
    }


    private IEnumerator AssignColor()
    {
        while (!infraOn)
            yield return null;
		
		foreach(Renderer r in myRenderers)
		{
        	r.material.color = heatColor;
		}
    }
}
