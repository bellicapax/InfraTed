using UnityEngine;
using System.Collections;

public class TestPitchLength : MonoBehaviour {

    public float slowness = 0.1f;
    private AudioSource mySource;

	// Use this for initialization
	void Start () 
    {
        mySource = GetComponent<AudioSource>();
        print("Pitch: " + mySource.pitch + "  Current Time: " + mySource.time);

        InvokeRepeating("ModifyPitch", 0.0f, 1.2f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(mySource.time > 0.5f)
            print("Pitch: " + mySource.pitch + "  Current Time: " + mySource.time + "  Clip Length: " + mySource.clip.length + "  Length x Pitch: " + mySource.clip.length/mySource.pitch);
	}

    void ModifyPitch()
    {
        mySource.pitch += Time.deltaTime * slowness;
    }
}
