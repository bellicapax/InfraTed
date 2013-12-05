using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

    public float minTimeOff = 0.05f;
    public float maxTimeOff = 0.3f;

    private float offTime;
    private GameObject[] aryGOs;
    private Light[] aryLights;

	// Use this for initialization
	void Start () 
    {
        aryGOs = GameObject.FindGameObjectsWithTag("Flicker");
        aryLights = new Light[aryGOs.Length];
        for(int i = 0; i < aryGOs.Length; i++)
        {
            aryLights[i] = aryGOs[i].GetComponent<Light>();
        }

        StartCoroutine(Flicker());
	}
	
	    // Random chance to assign a length of time that the light will be off.
        // Assign the somewhat random length of time off
        // 

    IEnumerator Flicker()
    {
        while (true)
        {
            if(offTime == 0)
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

                // Wait for the specified time
                yield return new WaitForSeconds(offTime);

                // Set the offTime back to 0
                offTime = 0;

                // Turn all the lights back on
                foreach (Light l in aryLights)
                {
                    l.enabled = true;
                }
            }

            yield return null;
        }
    }
}
