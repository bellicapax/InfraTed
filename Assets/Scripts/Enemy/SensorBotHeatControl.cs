using UnityEngine;
using System.Collections;
[RequireComponent(typeof (MeshVolume))]

public class SensorBotHeatControl : MonoBehaviour {

    public Color heatColor;
    public float secondsTillThaw = 5.0f;
    public bool xBeingTouched = false;
    public float xHeatEnergy;
    public Color xFrozenColor;

    private float heatMultiplier;
    private float heatHomeostasisRate = 4;
    private float thawCounter;
	private string objectDrain = "ObjectDrain";
    private Color originalColor;
    private HSBColor coldHSB;
    private Renderer[] myRenderers = new Renderer[2];
    private GameObject goCharacter;
    private CharacterInput scriptCharInput;
    private MeshVolume scriptMesh;
    private RoomHeatVariables scriptThermo;
    private Transform myTransform;
    private bool infraOn = false;
    
    // Use this for initialization
	void Start () 
    {
        myTransform = this.transform;
        myRenderers = myTransform.GetComponentsInChildren<Renderer>();
        originalColor = myRenderers[0].material.color;

        goCharacter = GameObject.Find("Character");
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptMesh = GetComponent<MeshVolume>();
        scriptThermo = GameObject.FindGameObjectWithTag("Thermometer").GetComponent<RoomHeatVariables>();
		heatColor = scriptThermo.roomInfraTemp;
		
        coldHSB = HSBColor.FromColor(scriptCharInput.xColdColor);
        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
        xHeatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * scriptMesh.volume;
        StartCoroutine(AssignColor());
	}
	
	// Update is called once per frame
	void Update () 
    {
        CheckForInput();
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
