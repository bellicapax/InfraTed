using UnityEngine;
using System.Collections;

public class OrganismDelayAndSpeed : MonoBehaviour {

    public float delay;
    
    private Animator myAnim;


	// Use this for initialization
	void Start () 
    {
        myAnim = GetComponent<Animator>();
        Invoke("Animate", delay);
	}
	
	// Update is called once per frame
	void Animate () 
    {
        myAnim.SetBool("Go", true);
        myAnim.speed = Random.Range(0.5f, 1.5f);
	}
}
