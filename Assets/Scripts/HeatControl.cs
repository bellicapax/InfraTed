using UnityEngine;
using System.Collections;

[RequireComponent(typeof (MeshVolume))]

public class HeatControl : MonoBehaviour {


    public bool canRegainTemp = false;
    public Color heatColor;
    public Material matNormal;
    public Material matInfra;
    public float secondsTillThaw = 5.0f;
    public bool xInHeatSensorRange = false;
    public bool xBeingTouched = false;
    public bool xGuard;
    public float xHeatEnergy;
    public Color xfrozenColor;

    private bool hotObject;
    private float heatMultiplier;
    private float heatHomeostasisRate = 4;
    private float thawCounter;
    private string lukewarm = "Lukewarm";
    private string hot = "Hot";
    private string cold = "Cold";
    private Color originalColor;
    private HSBColor coldHSB;
    private Renderer myRenderer;
    private Renderer[] sensorRenderers = new Renderer[2];
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
        originalColor = heatColor;
        myTransform = this.transform;

        if (myTransform.tag == hot)
            hotObject = true;
        else
            hotObject = false;

        if (myTransform.parent == null || myTransform.parent.GetComponent<EnemyMovement>() == null)
        {
            xGuard = false;
            myRenderer = myTransform.renderer;
        }
        else
        {
            xGuard = true;
            canRegainTemp = true;
            sensorRenderers = myTransform.GetComponentsInChildren<Renderer>();
        }

        if (!matInfra && !xGuard)
            Debug.LogError("Infrared material not assigned in the Inspector!");
        if (!matNormal && !xGuard)
            Debug.LogError("Normal material not assigned in the Inspector!");

        goCharacter = GameObject.Find("Character");
        goRoomThermo = GameObject.FindGameObjectWithTag("Thermometer");
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptMesh = GetComponent<MeshVolume>();
        scriptThermo = goRoomThermo.GetComponent<RoomHeatVariables>();
        coldHSB = HSBColor.FromColor(scriptCharInput.coldColor);
        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
        xHeatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * scriptMesh.volume;
        StartCoroutine(AssignColor());
	}
	
	// Update is called once per frame
	void Update () 
    {
        MaterialSwap();
        EnergyAndColor();
        RegainHeat();
        RefreshTag();
	}

    private void MaterialSwap()
    {
        if (Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
            {
                myRenderer.material = matNormal;
                infraOn = false;
            }
            else
            {
                myRenderer.material = matInfra;
                infraOn = true;
            }
        }
    }

    private void EnergyAndColor()
    {
        if (infraOn)
        {
            if (myTransform.tag == lukewarm || xGuard)  // If the tag changed to lukewarm or it is a guard, we don't need to check if it's in range of our heat spectrum
            {
                myRenderer.material.color = heatColor;
            }
            else if (myTransform.tag == hot)
            {
                if (HSBColor.FromColor(heatColor).h <= coldHSB.h)
                {
                    myRenderer.material.color = heatColor;
                }
                else
                {
                    myRenderer.material.color = scriptCharInput.coldColor;
                }
            }
            else if (myTransform.tag == cold)
            {
                if (HSBColor.FromColor(heatColor).h > 0.0f)
                {
                    myRenderer.material.color = heatColor;
                }
                else
                {
                    myRenderer.material.color = Color.red;
                }
            }
        }
    }

    private void RegainHeat()
    {
        if (infraOn && canRegainTemp && !xBeingTouched)  // If the infrared vision is on and the object is one that regains or loses heat naturally and it's not currently being drained or deposited
        {
            if (xGuard)
            {
                if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(originalColor).h && !xBeingTouched)
                {
                    if (HSBColor.FromColor(heatColor).h < HSBColor.FromColor(xfrozenColor).h)  // If the guard is actually frozen and not just colder
                    {
                        if (thawCounter >= secondsTillThaw)
                            heatColor.H(HSBColor.FromColor(heatColor).h - (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
                        else
                            thawCounter += Time.deltaTime;
                    }
                    else
                        heatColor.H(HSBColor.FromColor(heatColor).h - (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
                }
                else
                    thawCounter = 0.0f;
            }
            else if (hotObject && HSBColor.FromColor(heatColor).h > HSBColor.FromColor(originalColor).h)
            {
                heatColor.H(HSBColor.FromColor(heatColor).h - (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
            }
            else if (HSBColor.FromColor(heatColor).h < HSBColor.FromColor(originalColor).h)
            {
                heatColor.H(HSBColor.FromColor(heatColor).h + (1 / (xHeatEnergy * heatHomeostasisRate)) * Time.deltaTime, ref heatColor);
            }
            myRenderer.material.color = heatColor;
        }
    }

    private void RefreshTag()
    {
        if (!xGuard)
        {
            HSBColor tempHSB = HSBColor.FromColor(heatColor);

            if (tempHSB.h < scriptThermo.minStealthHue)             // Since our hotter hues are smaller than our cooler ones, we must be below the min to be hot
            {
                this.tag = hot;
            }

            else if (tempHSB.h > scriptThermo.maxStealthHue)         // Since our colder hues are larger than our hotter ones, we must be above the max to be cold
            {
                this.tag = cold;
            }

            else if (tempHSB.h <= scriptThermo.maxStealthHue && tempHSB.h >= scriptThermo.minStealthHue)
            {
                this.tag = lukewarm;
            }
        }
    }

    private IEnumerator AssignColor()
    {
        while (!infraOn)
            yield return null;

        myRenderer.material.color = heatColor;
    }
}
