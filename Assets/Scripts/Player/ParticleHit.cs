using UnityEngine;
using System.Collections;

public class ParticleHit : MonoBehaviour 
{
    public float freezeDecrement = 10.0f;
	
	private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
	private CharacterInput scriptCharInput;
	
	void Start()
	{
		scriptCharInput = this.transform.parent.GetComponent<CharacterInput>();
	}
	
	void OnParticleCollision(GameObject other)
	{
		ParticleSystem prt;
		prt = other.GetComponent<ParticleSystem>();
		int safeLength = prt.safeCollisionEventSize;
        if (collisionEvents.Length < safeLength)
            collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
		
		int numCollisionEvents = prt.GetCollisionEvents(gameObject, collisionEvents);
		for(int i = 0; i < numCollisionEvents; i++)
		{
			scriptCharInput.xTransferEnergy -= freezeDecrement * Time.deltaTime;
            print("BOOM");
		}
	}
}
