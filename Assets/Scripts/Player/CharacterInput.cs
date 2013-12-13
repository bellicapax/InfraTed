using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInput : MonoBehaviour {

    public float touchDistance = 5.0f;
    public float seeingBotDrain = 2.0f;
    public float offsetDeltaTime = 100.0f;
    public float energyIncrement = 10.0f;
    public float fadeOutRate = 1.0f;
    public float drainVolume = 0.75f;
    public static bool infraOn = false;
    public bool newScene = true;
    public AudioSource sourceDrain;
    public Material matLukewarm;
    public Transform characterPalm;
    public ParticleSystem suckPart;
    public ParticleSystem blowPart;
    public AudioClip clipFootstep;
    public AudioClip clipDrain;
    public Color xColdColor = new Color(0.2627450980392157f, 0.0f, 1.0f);
    public float xTransferEnergy = 0.0f;
    public static int idleState = Animator.StringToHash("Base Layer.Idle");
    public static int extendState = Animator.StringToHash("Base Layer.Extend");
    public static int drainState = Animator.StringToHash("Base Layer.Drain");
    public static int retractState = Animator.StringToHash("Base Layer.Retract");

    private bool playingDrain = false;
    private bool stoppingSteps = false;
    private string objectDrain = "ObjectDrain";
    private string strHorz = "Horizontal";
    private string strVert = "Vertical";
    private float originalSpeedByHeat = 3.374859f;
    private AudioSource sourceFoot;
    private Material[] aryOriginalMaterial;
    private GameObject[] aryLukewarmGO;
    private List<Material[]> lisOriginalMaterial = new List<Material[]>();
    private Transform transMainCam;
    private Animator myAnim;
    private AnimatorStateInfo currentState;
    private CharacterEnergy scriptCharEnergy;
    private RoomHeatVariables scriptThermometer;
    private CharacterMotor scriptCharMotor;
    private Light[] aryLights;


	// Use this for initialization
	void Start () 
    {
        sourceFoot = GetComponent<AudioSource>();
        myAnim = GetComponentInChildren<Animator>();
        currentState = myAnim.GetCurrentAnimatorStateInfo(0);           // Get the current state for the base layer
        transMainCam = Camera.main.transform;
        scriptCharEnergy = GetComponent<CharacterEnergy>();
        scriptCharMotor = GetComponent<CharacterMotor>();
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
        HandleSounds();
        GoInfrared();
        TouchDrain();
	}

    private void GoInfrared()
    {
        if (Input.GetButtonDown("Infrared"))
        {
            if (infraOn)
            {
                // If the infrared vision was on when we entered a new scene (for whatever reason), load the arrays and lists
                if (newScene)
                {
                    aryLukewarmGO =  GameObject.FindGameObjectsWithTag("Lukewarm");
                    aryOriginalMaterial = new Material[aryLukewarmGO.Length];
                    for (int i = 0; i < aryLukewarmGO.Length; i++)
                    {
                        lisOriginalMaterial[i] = new Material[aryLukewarmGO[i].transform.renderer.materials.Length];
                        lisOriginalMaterial[i] = (aryLukewarmGO[i].transform.renderer.materials);
                        //aryOriginalMaterial[i] = aryLukewarmGO[i].transform.renderer.material;
                    }

                    print("Number of objects in lukewarm GameObject array: " + aryLukewarmGO.Length + "  Number of objects in List of original Materials: " + lisOriginalMaterial.Count);
                    aryLights = FindObjectsOfType(typeof(Light)) as Light[];
                    newScene = false;
                }


                // Put the original materials back on the objects
                for (int i = 0; i < aryLukewarmGO.Length; i++)
                {
                    //aryLukewarmGO[i].renderer.material = aryOriginalMaterial[i];
                    aryLukewarmGO[i].renderer.materials = lisOriginalMaterial[i];
                }


                // Turn the lights back on
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
                        //lisOriginalMaterial[i] = new Material[aryLukewarmGO[i].transform.renderer.materials.Length];
                        lisOriginalMaterial.Add(aryLukewarmGO[i].transform.renderer.materials);
                        //aryOriginalMaterial[i] = aryLukewarmGO[i].transform.renderer.material;
                    }
                    aryLights = FindObjectsOfType(typeof(Light)) as Light[];
                    newScene = false;
                }


                // Switch each material in each object out with the lukewarm material
                foreach (GameObject aGO in aryLukewarmGO)
                {
                    Material[] mats = aGO.renderer.materials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = matLukewarm;
                    }

                    aGO.renderer.materials = mats;
                    //aGO.renderer.material = matLukewarm;
                }

                // Turn off all the lights in the scene
                foreach (Light aLight in aryLights)
                {
                    aLight.enabled = false;
                }
                infraOn = true;
            }
        }
    }

    private void HandleSounds()
    {
        // If we are moving AND pressing a move button
        if (((Input.GetAxis(strHorz) < -0.1f) || (Input.GetAxis(strHorz) > 0.1f) || (Input.GetAxis(strVert) < -0.1f) || (Input.GetAxis(strVert) > 0.1f)) && (Input.GetButton(strHorz) || Input.GetButton(strVert)))
        {
            //  If we're not already playing the foot loop, play it
            if (!sourceFoot.isPlaying)
                PlaySound(clipFootstep, sourceFoot, true, scriptCharMotor.movement.maxForwardSpeed / originalSpeedByHeat);
            else
                sourceFoot.pitch = scriptCharMotor.movement.maxForwardSpeed / originalSpeedByHeat;
        }

        // If we've released the horizontal and we're not holding vertical or vice versa
        if (((Input.GetButtonUp(strHorz) && !Input.GetButton(strVert)) || (Input.GetButtonUp(strVert) && !Input.GetButton(strHorz))) || (!stoppingSteps && sourceFoot.isPlaying && !Input.GetButton(strHorz) && !Input.GetButton(strVert)))
        {
            stoppingSteps = true;
            StartCoroutine(StopFootsteps());
        }

    }

    private void TouchDrain()
    {
        if (Input.GetButton(objectDrain))       // If we're trying to drain energy
        {
            myAnim.SetBool("Draining", true);
            if (currentState.nameHash == drainState)    // And the hand is already out
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
                    else if ((tempSeeingBotHeat = itsTransform.GetComponentInChildren<SeeingBotHeatControl>()) != null)	// If it's a guard
                    {
                        if (HSBColor.FromColor(tempSeeingBotHeat.heatColor).h < HSBColor.FromColor(xColdColor).h) 	// If they aren't already as cold as can be
                        {
                            // Reduce the Seeing Bot's heat
                            tempSeeingBotHeat.heatColor.H(HSBColor.FromColor(tempSeeingBotHeat.heatColor).h + (seeingBotDrain / tempSeeingBotHeat.xHeatEnergy) * Time.deltaTime, ref tempSeeingBotHeat.heatColor);

                            // If we aren't already as hot as can be, heat us up
                            if(scriptCharEnergy.currentEnergy < 100.0f)
                                xTransferEnergy = energyIncrement * Time.deltaTime;
                            
                            // Play the Suction particle effect
                            TurnOnSuctionParticle(hit, suckPart);
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
            if (Input.GetButtonUp(objectDrain))
            {
                StartCoroutine(StopDrain());
            }
        }

        if (playingDrain)
        {
            StartCoroutine(StopDrain());
            playingDrain = false;
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
        if (!playingDrain)
        {
            PlaySound(clipDrain, sourceDrain, true, 1.0f, drainVolume);
            playingDrain = true;
        }
        
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

    void PlaySound(AudioClip clip, AudioSource source, bool loop, float pitch = 1.0f, float volume = 1.0f)
    {
        source.clip = clip;

        source.pitch = pitch;

        source.volume = volume;

        if (loop)
            source.loop = true;

        source.Play();
    }

    IEnumerator StopDrain()
    {
        while (sourceDrain.volume > 0)
        {
            sourceDrain.volume -= Time.deltaTime * fadeOutRate;
            yield return null;
        }

            sourceDrain.Stop();
            sourceDrain.volume = 1.0f;
            playingDrain = false;
    }

    IEnumerator StopFootsteps()
    {
        // If it's between steps or at the end of the second step, stop
        while (!(((sourceFoot.time > 0.23f) && (sourceFoot.time < 0.255f)) || (sourceFoot.time > 0.485f)))
        {
            //print("Clip length: " + sourceFoot.clip.length +  " Time = " + sourceFoot.time + " Desired time between: " + (0.23f * 1 / sourceFoot.pitch) + " & " + (0.26f * 1 / sourceFoot.pitch) + "  Or Greater than " + (0.485f * 1 / sourceFoot.pitch));
            yield return null;
        }
        
        stoppingSteps = false;
        sourceFoot.Stop();
    }
}
