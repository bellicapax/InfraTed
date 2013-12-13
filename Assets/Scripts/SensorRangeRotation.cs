using UnityEngine;
using System.Collections;

public class SensorRangeRotation : MonoBehaviour {

    public float rotateSpeed = 10.0f;

    private float yRot = 0.0f;
    private Transform myTransform;

    // Use this for initialization
    void Start()
    {
        myTransform = this.transform;
        yRot = myTransform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        yRot += Time.deltaTime * rotateSpeed;
        myTransform.YRotation(yRot); 
    }
}
