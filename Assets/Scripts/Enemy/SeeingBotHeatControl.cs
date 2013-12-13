using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class SeeingBotHeatControl : MonoBehaviour {

    public Color heatColor;
    public Material matInfra;
    public float secondsTillThaw = 5.0f;
    public float heatHomeostasisRate = 4;
    public bool xInHeatSensorRange = false;
    public bool xBeingTouched = false;
    public float xHeatAmountMultiplier = 5.0f;
    public float xHeatEnergy;
    public Color xOriginalColor;

    private float heatMultiplier;
    private float thawCounter;
	private string objectDrain = "ObjectDrain";
    private Color visibleLightColor;
    private HSBColor coldHSB;
    private Material eyeMat;
    private Renderer eyeRenderer;
    private Renderer[] myRenderers;
    private Material metalMaterial;
    private List<Renderer> metalRenderers = new List<Renderer>();
    private GameObject goCharacter;
    public GameObject goRoomThermo;
    private CharacterInput scriptCharInput;
    private RoomHeatVariables scriptThermo;
    private Transform myTransform;
    private bool infraOn = false;
    
    // Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        myRenderers = new Renderer[myTransform.GetComponentsInChildren<Renderer>().Length];
        myRenderers = myTransform.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in myRenderers)
        {
            if (r.tag != "Eye" && r.tag != "Extinguisher")
            {
                metalRenderers.Add(r);
            }
            else if (r.tag == "Eye")
            {
                eyeRenderer = r;
                eyeMat = r.material;
            }
        }

        metalMaterial = metalRenderers[0].material;
        visibleLightColor = myRenderers[0].material.color;
		xOriginalColor = heatColor;

        goCharacter = GameObject.Find("Character");
        goRoomThermo = GameObject.FindGameObjectWithTag("Thermometer");
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptThermo = goRoomThermo.GetComponent<RoomHeatVariables>();
        coldHSB = HSBColor.FromColor(scriptCharInput.xColdColor);
        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
        xHeatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * xHeatAmountMultiplier;
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
            {
                foreach (Renderer r in metalRenderers)
                {
                    r.material = metalMaterial;
                }
                eyeRenderer.material = eyeMat;
                infraOn = false;
            }
            else
            {
                foreach (Renderer r in metalRenderers)
                {
                    r.material = matInfra;
                }
                eyeRenderer.material = matInfra;
                infraOn = true;
            }
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
                //foreach(Renderer r in myRenderers)
                //{
                //    r.material.color = heatColor;
                //}
                matInfra.color = heatColor;
			}
            else
			{
                //foreach(Renderer r in myRenderers)
                //{
                //    r.material.color = scriptCharInput.xColdColor;
                //}
                matInfra.color = scriptCharInput.xColdColor;
            }
        }
    }

    private void RegainHeat()
    {
        if (!xBeingTouched)  // If it's not currently being drained
        {
            if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(xOriginalColor).h) // If the guard's heat is colder than the original color
            {
                if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(EnemyShared.xFrozenColor).h)  // If the guard is actually frozen and not just colder
                {
                    if (thawCounter >= secondsTillThaw)
                    {
                        heatColor.H(HSBColor.FromColor(heatColor).h - (heatHomeostasisRate / xHeatEnergy) * Time.deltaTime, ref heatColor);
                        print("Heating up but still frozen");
                    }
                    else
                    {
                        thawCounter += Time.deltaTime;
                        print("Frozen and waiting");
                    }
                }
                else // Else we are colder than our original temperature, but not frozen, so heat back up and reset the thaw counter
                {
                    thawCounter = 0.0f;
                    heatColor.H(HSBColor.FromColor(heatColor).h - (heatHomeostasisRate / xHeatEnergy) * Time.deltaTime, ref heatColor);
                    //print("Heating up NOT frozen. Heat hue: " + HSBColor.FromColor(heatColor).h + " Original hue: " + HSBColor.FromColor(xOriginalColor).h);
                }
            }
        }
    }


    private IEnumerator AssignColor()
    {
        while (!infraOn)
            yield return null;
		
        //foreach(Renderer r in myRenderers)
        //{
        //    r.material.color = heatColor;
        //}
        matInfra.color = heatColor;
    }
}
