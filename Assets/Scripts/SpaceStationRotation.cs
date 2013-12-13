using UnityEngine;
using System.Collections;

public class SpaceStationRotation : MonoBehaviour {

    public float rotateSpeed = 10.0f;

    private Transform myTransform;

    // Use this for initialization
    void Start()
    {
        myTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.RotateAround(myTransform.position, myTransform.up, rotateSpeed * Time.deltaTime);
    }
}
