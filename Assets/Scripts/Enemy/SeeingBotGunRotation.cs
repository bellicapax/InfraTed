using UnityEngine;
using System.Collections;

public class SeeingBotGunRotation : MonoBehaviour {

    public static Quaternion gunOffset;

	// Use this for initialization
	void Start () 
    {
        gunOffset = Quaternion.Inverse(transform.localRotation);
	}
}
