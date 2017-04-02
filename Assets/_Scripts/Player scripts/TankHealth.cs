using UnityEngine;
using UnityEngine.UI;

/* Script controls the health aspect of the tank unit */

public class TankHealth : MonoBehaviour
{
    public float StartingHealth = 100f;          		// Represents the starting health
    public Slider Slider;        						// Holds reference to health slider               
    public Image FillImage;                      		// Holds reference to fill in the health slider
    public Color FullHealthColor = Color.green;  		// Represents the green for full health
	public Color ZeroHealthColor = Color.red;    		// Represents the red for zero health
    public GameObject ExplosionPrefab;
    
    
    private AudioSource ExplosionAudio;          		// Holds reference to explosion audio
    private ParticleSystem ExplosionParticles;   		// Holds reference to the explosion particle system
    private float CurrentHealth;  						// Holds value of current health
    private bool Dead;            						// Represents whether dead or alive

	/* Instantiates the script elements such as sound and particle system */

    private void Awake()
    {
        ExplosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();
        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();

        ExplosionParticles.gameObject.SetActive(false);
    }

	/* When enabled the current health is set to starting health and the UI or the slider is updated */
    private void OnEnable()
    {
        CurrentHealth = StartingHealth;
        Dead = false;

        SetHealthUI();
    }
    
	/* This is a public function which will be called when tank takes damage from the shell, which applied to current health and the health slider */
    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
		CurrentHealth -= amount;

		SetHealthUI ();

		// if health is zero or not dead already call onDeath

		if (CurrentHealth <= 0f && !Dead) {

			OnDeath ();

		}
    }

	/* It is used to update the UI to reflect the health */

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

		Slider.value = CurrentHealth;

		FillImage.color = Color.Lerp (ZeroHealthColor, FullHealthColor, CurrentHealth/StartingHealth);

    }

	/* Invoked when health deplits to nothing */

    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
		Dead = true;
		ExplosionParticles.transform.position = transform.position;
		ExplosionParticles.gameObject.SetActive (true);

		ExplosionParticles.Play ();
		ExplosionAudio.Play ();

		gameObject.SetActive (false);
    }
}