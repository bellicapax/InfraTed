using UnityEngine;
using System.Collections;

public class SprayGun : MonoBehaviour {

    public float freezeDecrement = 10.0f;
	private string walls = "Walls";
	private string obstacles = "Obstacles";
    private ParticleSystem prtSys;
    private CharacterInput scriptCharInput;


    void Start()
    {
        prtSys = GetComponent<ParticleSystem>();
        scriptCharInput = GameObject.Find("Character").GetComponent<CharacterInput>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem.CollisionEvent[] colEvents = new ParticleSystem.CollisionEvent[prtSys.safeCollisionEventSize];

        int numCollisionEvents = prtSys.GetCollisionEvents(other, colEvents);

//        for (int i = 0; i < numCollisionEvents; i++)
//        {
//
//            //print(colEvents[i].collider.name);
//        }
    }
}
