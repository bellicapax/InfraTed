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
        if (other.gameObject.layer == obstacles)
        {
            isBumping = true;
            scriptMove.nameBump = other.name;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == obstacles)
        {
            isBumping = false;
            scriptMove.nameBump = "";
        }
    }
}
