using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask TankMask;						// Holds reference to the layer mask where you can impose damage
    public ParticleSystem ExplosionParticles;  		// Holds reference to the explosion particle system for the shell     
    public AudioSource ExplosionAudio;              // Holds reference to explosion audio
    public float MaxDamage = 100f;                  // Represents the maximum damage that can be done by a shell
    public float ExplosionForce = 1000f; 			// Represents an explosion force from the point of explosion           
    public float MaxLifeTime = 2f;                  // Represents how long the shell will live once exploded
    public float ExplosionRadius = 5f;              // Represents the radius of impact that the shell can have on the tank unit

	/* After instantiation destroy the shell after max life time */

    private void Start()
    {
        Destroy(gameObject, MaxLifeTime);
    }

	/* Called when the shell collides with something */

    private void OnTriggerEnter(Collider other)
    {
		// Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
		Collider[] colliders = Physics.OverlapSphere (transform.position, ExplosionRadius, TankMask);

		// Go through all the colliders...
		for (int i = 0; i < colliders.Length; i++)
		{
			// ... and find their rigidbody.
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

			// If they don't have a rigidbody, go on to the next collider.
			if (!targetRigidbody)
				continue;

			// Add an explosion force.
			targetRigidbody.AddExplosionForce (ExplosionForce, transform.position, ExplosionRadius);

			// Find the TankHealth script associated with the rigidbody.
			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

			// If there is no TankHealth script attached to the gameobject, go on to the next collider.
			if (!targetHealth)
				continue;

			// Calculate the amount of damage the target should take based on it's distance from the shell.
			float damage = CalculateDamage (targetRigidbody.position);

			// Deal this damage to the tank.
			targetHealth.TakeDamage (damage);
    	}

		// Unparent the particles from the shell.
		ExplosionParticles.transform.parent = null;

		// Play the particle system.
		ExplosionParticles.Play();

		// Play the explosion sound effect.
		ExplosionAudio.Play();

		// Once the particles have finished, destroy the gameobject they are on.
		Destroy (ExplosionParticles.gameObject, ExplosionParticles.main.duration);

		// Destroy the shell.
		Destroy (gameObject);
	}

	/* The damage done by the shell to the target is calculated by this function */

    private float CalculateDamage(Vector3 targetPosition)
    {
		// Create a vector from the shell to the target.
		Vector3 explosionToTarget = targetPosition - transform.position;

		// Calculate the distance from the shell to the target.
		float explosionDistance = explosionToTarget.magnitude;

		// Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
		float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

		// Calculate damage as this proportion of the maximum possible damage.
		float damage = relativeDistance * MaxDamage;

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max (0f, damage);

		return damage;
	
    }
}