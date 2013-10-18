using UnityEngine;
using System.Collections;

public class EnemyBump : MonoBehaviour {

    public bool isBumping = false;

    private int obstacles = 10;
    private EnemyMovement scriptMove;

	// Use this for initialization
	void Start () 
    {
        scriptMove = transform.parent.GetComponent<EnemyMovement>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Character")  // If the object is an obstacle and it's not another guard
        {
            isBumping = true;
            scriptMove.nameBump = other.name;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Character")
        {
            isBumping = false;
            scriptMove.nameBump = "";
        }
    }
}
