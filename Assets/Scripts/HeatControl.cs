using UnityEngine;
using System.Collections;

public class HeatControl : MonoBehaviour {


    public Color heatColor;
    public float heatEnergy;
    public Material matNormal;
    public Material matInfra;

    private float heatMultiplier;
    private HSBColor coldHSB;
    private GameObject goCharacter;
    private CharacterInput scriptCharInput;
    private MeshVolume scriptMesh;
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
        scriptCharInput = goCharacter.GetComponent<CharacterInput>();
        scriptMesh = GetComponent<MeshVolume>();
        coldHSB = HSBColor.FromColor(scriptCharInput.coldColor);
        heatMultiplier = (10.0f / (coldHSB.h * coldHSB.h));         //This makes it so that an object with the highest temperature (100.0 degrees) and a volume of one cubed unit will take 10 seconds to be fully drained
        heatEnergy = heatMultiplier * Mathf.Abs(HSBColor.FromColor(heatColor).h - coldHSB.h) * scriptMesh.volume;
	}
	
	// Update is called once per frame
	void Update () 
    {
        MaterialSwap();
        EnergyAndColor();
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
}
