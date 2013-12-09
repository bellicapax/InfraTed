using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightFlicker : MonoBehaviour {

    public float minTimeOff = 0.05f;
    public float maxTimeOff = 0.3f;
    public Shader diffuse;

    private float offTime;
    private GameObject[] aryGOLights;
    private Light[] aryLights;
    private List<Shader> lisShader = new List<Shader>();
    private List<Renderer> lisRenderers = new List<Renderer>();
    
	// Use this for initialization
	void Start () 
    {
        aryGOLights = GameObject.FindGameObjectsWithTag("Flicker");
        aryLights = new Light[aryGOLights.Length];
        
        for(int i = 0; i < aryGOLights.Length; i++)
        {
            aryLights[i] = aryGOLights[i].GetComponent<Light>();

            // If the list doesn't already contain the renderer, add it and add the material to the mat list
            
            if (!lisRenderers.Contains(aryGOLights[i].transform.parent.GetComponent<Renderer>()))
            {
                lisRenderers.Add(aryGOLights[i].transform.parent.GetComponent<Renderer>());
                lisShader.Add(lisRenderers[lisRenderers.Count - 1].material.shader);
            }
        }

        StartCoroutine(Flicker());
	}
	
	    // Random chance to assign a length of time that the light will be off.
        // Assign the somewhat random length of time off

    IEnumerator Flicker()
    {
        while (true)
        {
            if (!CharacterInput.infraOn)
            {
                if (offTime == 0)
                {

                    if (Random.Range(0.0f, 1.0f) >= 0.99f)
                    {
                        offTime = Random.Range(minTimeOff, maxTimeOff);
                    }
                }
                else
                {
                    // Turn all the lights off
                    foreach (Light l in aryLights)
                    {
                        l.enabled = false;
                    }

                    foreach (Renderer r in lisRenderers)
                    {
                        r.material.shader = diffuse;
                    }

                    // Wait for the specified time
                    yield return new WaitForSeconds(offTime);
                    
                    if (!CharacterInput.infraOn)
                    {
                        // Set the offTime back to 0
                        offTime = 0;

                        // Turn all the lights back on
                        foreach (Light l in aryLights)
                        {
                            l.enabled = true;
                        }

                        for (int i = 0; i < lisRenderers.Count; i++)
                        {
                            lisRenderers[i].material.shader = lisShader[i];
                        }
                    }
                }
            }

            yield return null;
        }
    }
}
