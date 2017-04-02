using UnityEngine;

public class TankMovement : MonoBehaviour
{      
    public float Speed = 12f;      			// Represents the speed of the Tank      
    public float TurnSpeed = 180f;     		// Represents the roatation speed on the Tank
    public AudioSource MovementAudio;    	// Holds reference to the movement audio source
    public AudioClip EngineIdling;       	// Holds reference to engine idel clip
    public AudioClip EngineDriving;      	// Holds refeence to engine driving clip
    public float PitchRange = 0.2f;			// Represents the value of pitch range

  
    private string MovementAxisName; 		// Holds the value of the movement axis  
    private string TurnAxisName;         	// Represents the axis on which turning is to be done
    private Rigidbody Rigidbody;         	// Holds reference to the rigid body physics engine of the Tank
    private float MovementInputValue;    	// Represents the value obtained from the movement axis
	private float TurnInputValue;        	// Represents the value obtained from the movement axis
    private float OriginalPitch;         	// Represents the original pitch of the movement audio

	/* instantiates.. */

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

	/* When enabled default settings are applied */
    private void OnEnable ()
    {
        Rigidbody.isKinematic = false;
        MovementInputValue = 0f;
        TurnInputValue = 0f;
    }

	/* When disabled.. */
    private void OnDisable ()
    {
        Rigidbody.isKinematic = true;
    }

	/* This is where it begins, initilaizes the components enabling movement */
    private void Start()
    {
		MovementAxisName = "Vertical2";
		TurnAxisName = "Horizontal2";

        OriginalPitch = MovementAudio.pitch;
    }
  
	/* At each frame the input from the arrow keys up and down for movement and left and right for roatation is recorded */
    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
		MovementInputValue = Input.GetAxis(MovementAxisName);
		TurnInputValue = Input.GetAxis (TurnAxisName);

		EngineAudio ();
    }

	/* The values Movement input and tutn input is used to decide which clip to play and range of the pitch is calulcated and played */
    
	private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

		if (Mathf.Abs (MovementInputValue) < 0.1f && Mathf.Abs (TurnInputValue) < 0.1f) {
			if (MovementAudio.clip == EngineDriving) {
				MovementAudio.clip = EngineIdling;
				MovementAudio.pitch = Random.Range (OriginalPitch - PitchRange, OriginalPitch + PitchRange);
				MovementAudio.Play ();
			}	
		} 

		else {
			if (MovementAudio.clip == EngineIdling) {
				MovementAudio.clip = EngineDriving;
				MovementAudio.pitch = Random.Range (OriginalPitch - PitchRange, OriginalPitch + PitchRange);
				MovementAudio.Play ();
			}
		}

    }

	/* Used to perform the phyiscals i.e. movements for the tank */
    private void FixedUpdate()
    {
        // Move and turn the tank.
		Move();
		Turn ();
    }

	/* Movement is calculated and applied */

    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
		Vector3 movement = transform.forward * MovementInputValue * Speed * Time.deltaTime;
		Rigidbody.MovePosition (Rigidbody.position + movement);
    }

	/* Rotation is calculated and applied */

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
		float turn = TurnInputValue * TurnSpeed * Time.deltaTime;
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
		Rigidbody.MoveRotation (Rigidbody.rotation * turnRotation);
    }
}