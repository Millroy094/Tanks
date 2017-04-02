using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script is a shooting AI designed for the enemy tank unit */

public class ShootingAI : MonoBehaviour {

	       
	public Rigidbody Shell;   							// Holds a reference to the rigid body of the shell for the tank         
	public Transform FireTransform;   					// Holds a reference to the FireTransform           
	public AudioSource ShootingAudio;  					// Holds a reference to Shooting audio source
	public AudioClip ChargingClip;     					// Holds a reference to shell charging clip
	public AudioClip FireClip;         					// Holds a reference to shell fire clip
	[HideInInspector] public float Force;  				// Represents the launch force for the shell
	[HideInInspector]public bool FireNow = false;		// Indicates whether AI can start firing 
	        
	private bool Fired;  								// Indicates whether shell has already been fired
	private float delay = 2f;							// represents a 2 seconds delay between shells fired

	
	// Update is called once per frame

	void Update () {

		// if both enabled and fired is false start co routine to fire..

		if (FireNow && !Fired) {
			
			StartToFire();
			StartCoroutine ("Controller");
		} 
	}


	/* This method here is incharge of the firing */

	private void StartToFire()
	{

		// Set everytime method is called
		Fired = true;

		// Create an instance of the shell to be fired
		Rigidbody shellObject =
			Instantiate (Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;

		// Velocity is set
		shellObject.velocity = Force * FireTransform.forward; ;

		// Change the clip to the firing clip and play it.
		ShootingAudio.clip = FireClip;
		ShootingAudio.Play ();



	}

	/* This coroutine here manages the 2 second deplay between shells launches */

	private IEnumerator Controller(){
	
		yield return new WaitForSeconds(delay);
		Fired = false;
	
	}

}
