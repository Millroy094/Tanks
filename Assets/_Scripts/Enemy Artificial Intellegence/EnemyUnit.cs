using UnityEngine;
using System.Collections;


/* Class is used to get the waypoints for movement from the pathfinding algorithm and act on it */

public class EnemyUnit : MonoBehaviour {


	[HideInInspector] public Transform target;	// Represents transform of the target tank to follow
	float speed = 4;							// Represents the speed of movement and rotation
	Vector3[] calculatedPath;					// Represents an array of waypoint positions
	int waypointIndex;							// Used to loop through the waypoints

	ShootingAI shooter;							// References the Shooting AI script

	/* Instantiates the shooter-script for AI */

	void Awake(){
		
		shooter = GetComponent<ShootingAI> ();
	}

	/* Starts the pathfinding manager */

	void Start() {

		//Gets reference to the player tank to be followed
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		//Starts by requesting path to the target
		PathfindingRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	/* All movements are then coordinated from here */

	void Update() {
		
		Quaternion rotationAngle;			// Represents the angle of rotation

		// Only if the distance between any enemy and the player is 15 or less stop moving, aim and shoot

		if (Vector3.Distance (target.position, transform.position) <= 15) {
		
			StopCoroutine ("FollowFoundPath");
			rotationAngle = Quaternion.LookRotation (target.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotationAngle, Time.deltaTime * speed );

			//Enable the Shooting AI

			shooter.FireNow = true;
			shooter.Force = Vector3.Distance (target.position, transform.position);

		} else {

			// Else disable shooting, recalculate the path, and follow the player

			PathfindingRequestManager.RequestPath(transform.position,target.position, OnPathFound);
			shooter.FireNow = false;
		}
	}


	/* This is callback method which is run after PathRequestManager responds */
	 
	public void OnPathFound(Vector3[] newFoundPath, bool pathFound) {

		//If path is found, start following 
		if (pathFound) {
			calculatedPath = newFoundPath;
			waypointIndex = 0;
			StopCoroutine("FollowFoundPath");
			StartCoroutine("FollowFoundPath");
		}
	}

	/* Causes all the movement to happen */

	IEnumerator FollowFoundPath() {
		
		Quaternion rotationAngle;		//Holds the rotation angle
		Vector3 curWaypoint;			//Holds the current waypoint to move to

		//breaks if no path is returned

		if (calculatedPath == null)
			yield break;

		//initializes the current waypoint to the initial waypoint to move

		curWaypoint = calculatedPath[0];

		//Loops through until all waypoints are reached

		while (true) {


			if(Vector3.Distance(curWaypoint, transform.position) == 0) {
				waypointIndex ++;
				if (waypointIndex >= calculatedPath.Length) {
					yield break;
				}
				curWaypoint = calculatedPath[waypointIndex];
			}

			// Rotates towards to waypoint points and moves towards it 

			rotationAngle = Quaternion.LookRotation (curWaypoint - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotationAngle, Time.deltaTime);
			transform.position = Vector3.MoveTowards(transform.position,curWaypoint,speed * Time.deltaTime);

			yield return null;

		}
	}
		


	/* Draws the gizmos of the waypoints */

	public void OnDrawGizmos() {

		if (calculatedPath != null) {

			//Loops through and draws the waypoints

			for (int count = waypointIndex; count < calculatedPath.Length; count ++) {

				// Set the colour of the path

				Gizmos.color = Color.cyan;

				// Draw the waypoint

				Gizmos.DrawCube(calculatedPath[count], Vector3.one);

				//Draws links to the waypoints

				if (count == waypointIndex) {
					Gizmos.DrawLine(transform.position, calculatedPath[count]);
				}
				else {
					Gizmos.DrawLine(calculatedPath[count-1],calculatedPath[count]);
				}
			}
		}
	}
}