using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* Used to run the whole game */

public class GameControl : MonoBehaviour {

	public float StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
	public float EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
	public CameraControl CameraControl;       // Reference to the CameraControl script for control during different phases.
	public Text MessageText;                   // Reference to the overlay Text to display winning text, etc.

	public GameObject TankPrefab;			// Reference to the Tank prefab in the Assets
	public GameObject EnemyPrefab;			// Reference to the Enemy Tank prefab in the Assets

	public Color PlayerColor;				// Represents Player Tank colour 
	public Color EnemyColor;				// Represents Enemy Tank colour

	private GameObject Player;				// Holds reference to the player tank object created
	private GameObject Enemy;				// Holds reference to the enemy tank object created


	private TankShooting Shooting;			// Reference to Shooting script
	private TankMovement Movement;			// Reference to Movement script

	private ShootingAI AIShooting;			// Reference to Shooting AI Script
	private EnemyUnit MovementAI;			// Reference to Enemy Unit Script

	private WaitForSeconds StartWaitTime;   // Used to have a delay whilst the round starts.
	private WaitForSeconds EndWaitTime;    	// Used to have a delay whilst the round or game ends.

	public Transform spawnPoint1;			// Holds transform postions of spawn point 1
	public Transform spawnPoint2;			// Holds transform postions of spawn point 2

	private int no_tanks = 2; 				// Represents the number of tanks in the game
	private bool game_over = false;			// indicates if game is over

	//Initializes the Script 

	private void Awake(){

		// Create the delays

		StartWaitTime = new WaitForSeconds (StartDelay);
		EndWaitTime = new WaitForSeconds (EndDelay);

		//Player and Enemy are instantiated and given positions

		Player = Instantiate (TankPrefab, spawnPoint1.position, spawnPoint1.rotation);
		Enemy = Instantiate (EnemyPrefab, spawnPoint1.position, spawnPoint1.rotation);

		//Each of them is applied a colour

		ApplyColor (Player.GetComponentsInChildren<MeshRenderer> (), PlayerColor);
		ApplyColor (Enemy.GetComponentsInChildren<MeshRenderer> (), EnemyColor);

		//All the scripts are instantiated

		Movement = Player.GetComponent<TankMovement> ();
		Shooting = Player.GetComponent<TankShooting> ();
		MovementAI = Enemy.GetComponent<EnemyUnit>	();
		AIShooting = Enemy.GetComponent<ShootingAI> ();

	}

	// And here it begins..

	private void Start()
	{

		// Initialize the camera

		SetCameraTargets();

		// The game loop with state machines starts here

		StartCoroutine (GameLoop ());
	}

	/* Method is responsible for informating the camera control script about the number of tanks */

	private void SetCameraTargets()
	{
		// Create a two element array of targets representing the player and the enemy

		Transform[] targets = new Transform[no_tanks];

		// Set it to the appropriate tank transform.
		targets[0] = Player.transform;
		targets[1] = Enemy.transform;

		// These are the targets the camera should follow.
		CameraControl.Targets = targets;
	}

	/* This is the game loop and walks the game through different states */
	private IEnumerator GameLoop ()
	{
		// Starts off with initiating 'GameBegins' coroutine and does not return until phase has past.
		yield return StartCoroutine (GameBegins ());

		// Once 'GameBegins' coroutine has ended, run  'GamePlaying' coroutine and return only after it's finished.
		yield return StartCoroutine (GamePlaying());

		// Once execution has finished, run the 'GameOver' coroutine.
		yield return StartCoroutine (GameOver());

		// This code below is not executed until 'GameOver' has finished processing. 
		if (game_over)
		{
			// If there is a game winner, restart the level.
			Application.LoadLevel (Application.loadedLevel);
			game_over = false;
		}
		else
		{
			// If no there is not winner found, restart this coroutine so the loop continues.
			StartCoroutine (GameLoop ());
		}
	}

	/* Called at the start to sequence and prepare the game environment */

	private IEnumerator GameBegins ()
	{
		// When the game starts initialize the players and disable all movement

		initializePlayers ();
		disableAll ();

		// Reset the camera position

		CameraControl.SetStartPositionAndSize ();


		MessageText.text = "Attack!!! ";

		// Wait for 2 seconds before enabling gameplay

		yield return StartWaitTime;
	}

	/* Tanks can now move and fire and game moves to the next phase only after one or both blow up */

	private IEnumerator GamePlaying ()
	{
		// Enable the tanks
		enableAll ();

		MessageText.text = string.Empty;

		// till there is only one or none tanks left 

		while (!isOneTankLeft())
		{
			// return at the dawn next frame.
			yield return null;
		}
	}

	/* Displays the result after the game play on screen */

	private IEnumerator GameOver ()
	{
		// Disable all tanks
		disableAll ();

		game_over = true;

		// Get the message to display the winner on screen

		string message = FinalMessage ();
		MessageText.text = message;

		// Wait for 2 sec before giving back control to game loop 
		yield return EndWaitTime;
	}

	/* Method is responsible for generating the outcome for the final result */

	private string FinalMessage()
	{

		string msg; 

		// Draw if they both blow up

		if (!Player.activeSelf && !Enemy.activeSelf)
			msg = "DRAW!";

		//Player wins or else enemy wins

		else if (Player.activeSelf)
			msg = "Blue wins";
		else
			msg = "Red wins";

		return msg;
	}

	/* Used to determine if there is a winner or not in the game */

	private bool isOneTankLeft()
	{
		// Holds the number of tanks standing
		int tanksCount = 0;

		// loop through all the tanks. 

		if (Player.activeSelf)
			tanksCount++;
	
		if (Enemy.activeSelf)
			tanksCount++;

		// If there is less than one tank left then return true, else false
		return tanksCount <= 1;
	}

	// Disables all scripts and AIs 

	private void disableAll(){
	
		AIShooting.enabled = false;
		MovementAI.enabled = false;
		Movement.enabled = false;
		Shooting.enabled = false;
	
	}

	// Enables all scripts and AIs 

	private void enableAll(){

		AIShooting.enabled = true;
		MovementAI.enabled = true;
		Movement.enabled = true;
		Shooting.enabled = true;

	}
		
	/* Initialize or reinitialize essential components */

	public void initializePlayers(){

		// Set the player postion

		Player.transform.position = spawnPoint1.position;
		Player.transform.rotation = spawnPoint1.rotation;

		// Set the enemy position

		Enemy.transform.position = spawnPoint2.position;
		Enemy.transform.rotation = spawnPoint2.rotation;

		// Reinitiate the Player position

		Player.SetActive (false);
		Player.SetActive (true);

		// Reinitiate the Enemy position

		Enemy.SetActive (false);
		Enemy.SetActive (true);

	}

	// Used to change color of the tank

	public void ApplyColor(MeshRenderer[] renderers, Color color){

		foreach (MeshRenderer child in renderers) {

			child.material.color = color;
		}

	}
		

}
