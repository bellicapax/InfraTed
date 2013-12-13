using UnityEngine;
using System.Collections;

public class EscapeTheCredits : MonoBehaviour {

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            Application.LoadLevel("Start");
        if (Time.timeSinceLevelLoad > 0.75f)
        {
            if(Input.GetButtonDown("ObjectDrain"))
                Application.LoadLevel("Start");
        }
	}


}
