using UnityEngine;
using System.Collections;
[RequireComponent(typeof (MeshVolume))]

public class SensorBotHeatControl : MonoBehaviour {

    public Color heatColor;
    public float secondsTillThaw = 5.0f;
    public bool xBeingTouched = false;
    public float xHeatEnergy;
    public Color xfrozenColor;

    private float heatMultiplier;
    private float heatHomeostasisRate = 4;
    private float thawCounter;
    private Color originalColor;
    private HSBColor coldHSB;
    private Renderer[] myRenderers = new Renderer[2];
    private GameObject goCharacter;
    public GameObject goRoomThermo;
    private CharacterInput scriptCharInput;
    private MeshVolume scriptMesh;
    private RoomHeatVariables scriptThermo;
    private Transform myTransform;
    public bool infraOn = false;
    
    // Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        myRenderers = myTransform.GetComponentsInChildren<Renderer>();
        originalColor = myRenderers[0].material.color;

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
        CheckForInfra();
        EnergyAndColor();
        RegainHeat();
	}

    private void CheckForInfra()
    {
        if (Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
                infraOn = false;
            else
                infraOn = true;
        }
    }

    private void EnergyAndColor()
    {
        if (infraOn)
        {
            foreach (Renderer r in myRenderers)
            {
                r.material.color = heatColor;
            }
        }
        else
        {
            foreach (Renderer r in myRenderers)
            {
                r.material.color = originalColor;
            }
        }
    }

    private void RegainHeat()
    {
        if (infraOn && !xBeingTouched)  // If the infrared vision is on and it's not currently being drained or deposited
        {
            if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(originalColor).h && !xBeingTouched)
            {
                if (HSBColor.FromColor(heatColor).h > HSBColor.FromColor(xfrozenColor).h)  // If the guard is actually frozen and not just colder
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

            foreach (Renderer r in myRenderers)
            {
                r.material.color = heatColor;
            }
        }
    }

    private IEnumerator AssignColor()
    {
        while (!infraOn)
            yield return null;

        foreach (Renderer r in myRenderers)
        {
            r.material.color = heatColor;
        }
    }
}
