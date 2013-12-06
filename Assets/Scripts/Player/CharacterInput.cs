using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInput : MonoBehaviour {

    public float touchDistance = 5.0f;
    public float offsetDeltaTime = 100.0f;
    public float energyIncrement = 10.0f;
    public static bool infraOn = false;
    public bool newScene = true;
    public Material matLukewarm;
    public Transform characterPalm;
    public ParticleSystem suckPart;
    public ParticleSystem blowPart;
    public AudioClip[] characterSounds;
    public Color xColdColor = new Color(0.2627450980392157f, 0.0f, 1.0f);
    public float xTransferEnergy = 0.0f;
    public static int idleState = Animator.StringToHash("Base Layer.Idle");
    public static int extendState = Animator.StringToHash("Base Layer.Extend");
    public static int drainState = Animator.StringToHash("Base Layer.Drain");
    public static int retractState = Animator.StringToHash("Base Layer.Retract");


    private string objectDrain = "ObjectDrain";
    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();
    private Material[] aryOriginalMaterial;
    private GameObject[] aryLukewarmGO;
    private Transform transMainCam;
    private Animator myAnim;
    private AnimatorStateInfo currentState;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptThermometer;
    private Light[] aryLights;


	// Use this for initialization
	void Start () 
    {
        myAnim = GetComponentInChildren<Animator>();
        currentState = myAnim.GetCurrentAnimatorStateInfo(0);           // Get the current state for the base layer
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
        currentState = myAnim.GetCurrentAnimatorStateInfo(0);           // Get the current state for the base layer
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
        if(Input.GetButtonDown(objectDrain))
        {

        }

        if (Input.GetButton(objectDrain))       // If we're trying to drain energy
        {
            myAnim.SetBool("Draining", true);
            if (currentState.nameHash == drainState)
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
                            LoseGainHeat(tempHeatControl, hit, true);
                            return;
                        }
                        else if (itsTransform.tag == "Cold" && scriptCharEnergy.currentEnergy > 0.0f) // else if it is a cold object and we are not already at min temperature
                        {
                            LoseGainHeat(tempHeatControl, hit, false);
                            return;
                        }
                    }
                    else if ((tempSeeingBotHeat = itsTransform.GetComponent<SeeingBotHeatControl>()) != null)	// If it's a guard
                    {
                        if (HSBColor.FromColor(tempSeeingBotHeat.heatColor).h < HSBColor.FromColor(xColdColor).h) 	// If they aren't already as cold as can be
                        {
                            tempSeeingBotHeat.heatColor.H(HSBColor.FromColor(tempSeeingBotHeat.heatColor).h + (1 / tempSeeingBotHeat.xHeatEnergy) * Time.deltaTime, ref tempSeeingBotHeat.heatColor);
                            xTransferEnergy = energyIncrement * Time.deltaTime;
                            return;
                        }
                    }
                }
            }
        }
        // If we are not holding down the Input button
        else
        {
            myAnim.SetBool("Draining", false);
        }
        suckPart.Stop();
        blowPart.Stop();
    }

    void LoseGainHeat(HeatControl loseGainHeatControl, RaycastHit losegainHit, bool isHot)
    {

        if (isHot)
        {
            loseGainHeatControl.heatColor.H(HSBColor.FromColor(loseGainHeatControl.heatColor).h + (1 / loseGainHeatControl.xHeatEnergy) * Time.deltaTime, ref loseGainHeatControl.heatColor);
            xTransferEnergy = energyIncrement * Time.deltaTime;
            TurnOnSuctionParticle(losegainHit, suckPart);
        }
        else
        {
            loseGainHeatControl.heatColor.H(HSBColor.FromColor(loseGainHeatControl.heatColor).h - (1 / loseGainHeatControl.xHeatEnergy) * Time.deltaTime, ref loseGainHeatControl.heatColor);
            xTransferEnergy = -energyIncrement * Time.deltaTime;
            TurnOnSuctionParticle(losegainHit, blowPart);
        }
        loseGainHeatControl.xBeingTouched = true;
        
    }

    void TurnOnSuctionParticle(RaycastHit prtHit, ParticleSystem prt)
    {
        if (prt == suckPart)
        {
            prt.transform.position = prtHit.point;                                // Move the position of the particle system to the point the ray hit
            //print(prtHit.point);                                                        
            prt.transform.LookAt(characterPalm);                                  // Orient the system towards the palm of the character
            float distance = Vector3.Distance(prtHit.point, characterPalm.position);    // Find the distance between the point and the character palm
            float lifetime = distance / prt.startSpeed;                           // How long it should live is the distance divided by the speed it's moving at
            prt.startLifetime = lifetime * 0.95f;                                 // Set the lifetime to 95% of what we calculated so that it doesn't actually go through the hand
            if (prt.isStopped)
                prt.Play();
        }
        else
        {
            prt.transform.position = characterPalm.position;
            prt.transform.LookAt(prtHit.point);
            float distance = Vector3.Distance(prtHit.point, characterPalm.position);    // Find the distance between the point and the character palm
            float lifetime = distance / prt.startSpeed;                                 // How long it should live is the distance divided by the speed it's moving at
            prt.startLifetime = lifetime;                                               // Set the lifetime to what we calculated so that it goes to the object and stops         
            if (prt.isStopped)
                prt.Play();
        }
    }
}
