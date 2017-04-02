using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{       	
    public Rigidbody Shell;     				// References the rigid body component of the shell       
    public Transform FireTransform;    			// References the transform FireTransform game object
    public Slider AimSlider;           			// References the Aim Slider for the tank
    public AudioSource ShootingAudio;  			// References the audio source for shooting 
    public AudioClip ChargingClip;     			// References the audio clip for charging
    public AudioClip FireClip;         			// References the audio clip for firing
    public float MinLaunchForce = 15f; 			// Represents the minimum launch force that can be applied
	public float MaxLaunchForce = 30f; 			// Represents the maximum launch force that can be applied
    public float MaxChargeTime = 0.75f;			// Represents the maximum charge time that player to control the intensity of shelling firing

    
    private string FireButton;         			// Represents a string value to the fire button which will be held, pressed, and left to fire
    private float CurrentLaunchForce;  			// Represents the current launch force
    private float ChargeSpeed;         			// Represents the charge speed from when the button is held
    private bool Fired;                			// Represents whether shell was fired or not

	/* Is used to enable and reinitialize the shell for that tank in question */
    
	private void OnEnable()
    {
        CurrentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }

	/* Fire button string is set and the charge speed is calculated so that we are ready use the firing mechanism */
    
	private void Start()
    {
		FireButton = "Fire1";

        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
    }
    
	/* This where all the action is initialed and calculated */
    private void Update()
    {
		// The slider is set to the default value
		AimSlider.value = MinLaunchForce;

		// If the max force has been exceeded and the shell hasn't yet been launched...
		if (CurrentLaunchForce >= MaxLaunchForce && !Fired)
		{
			// ... use the max force and launch the shell.
			CurrentLaunchForce = MaxLaunchForce;
			Fire ();
		}
		// Otherwise, if the fire button has just started being pressed...
		else if (Input.GetButtonDown (FireButton))
		{
			// ... reset the fired flag and reset the launch force.
			Fired = false;
			CurrentLaunchForce = MinLaunchForce;

			// Change the clip to the charging clip and start it playing.
			ShootingAudio.clip = ChargingClip;
			ShootingAudio.Play ();
		}
		// Otherwise, if the fire button is being held and the shell hasn't been launched yet...
		else if (Input.GetButton (FireButton) && !Fired)
		{
			// Increment the launch force and update the slider.
			CurrentLaunchForce += ChargeSpeed * Time.deltaTime;

			AimSlider.value = CurrentLaunchForce;
		}
		// Otherwise, if the fire button is released and the shell hasn't been launched yet...
		else if (Input.GetButtonUp (FireButton) && !Fired)
		{
			// ... launch the shell.
			Fire ();
		}
    }

	/* This method does the actual firing of the shells from the tank */
    
	private void Fire()
    {
		// Set the fired flag so only Fire is only called once.
		Fired = true;

		// Create an instance of the shell and store a reference to it's rigidbody.
		Rigidbody shellInstance =
			Instantiate (Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellInstance.velocity = CurrentLaunchForce * FireTransform.forward; ;

		// Change the clip to the firing clip and play it.
		ShootingAudio.clip = FireClip;
		ShootingAudio.Play ();

		// Reset the launch force.  This is a precaution in case of missing button events.
		CurrentLaunchForce = MinLaunchForce;
    }
}