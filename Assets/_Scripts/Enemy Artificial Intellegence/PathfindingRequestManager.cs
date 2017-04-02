using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* Script is incharge of initiating and managing pathfinding requests to the pathfinding script */

public class PathfindingRequestManager : MonoBehaviour {

	Queue<PathRequest> RequestQueue = new Queue<PathRequest>();  	// Creates a queue of type path requests 
	PathRequest currentRequest;									 	// Represents the reference of current path request

	static PathfindingRequestManager instance;								 	// Holds a static reference of itself
	AstarPathfinding pathfinding;										// Holds a reference to pathfinding instance

	bool DoneProcessingPath;										  		// Tells whether there is a request being processed

	/* Retrieves the instance of the AStar game object and takes the reference of pathfinding */

	void Awake() {
		instance = this;
		pathfinding = GetComponent<AstarPathfinding>();
	}

	/* This method is used to request the path and place the request in a queue*/

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		instance.RequestQueue.Enqueue(newRequest);
		instance.ProcessNext();
	}

	/* This is where the queue is actioned upon, where the pathfinding script is given the task of calculating the waypoints */

	void ProcessNext() {

		// If path is not in queue and count is not zero...

		if (!instance.DoneProcessingPath && instance.RequestQueue.Count > 0) {

			//Process path request and return path..

			currentRequest = instance.RequestQueue.Dequeue ();
			instance.DoneProcessingPath = true;
			pathfinding.StartFindingPath (currentRequest.start, currentRequest.end);
		}

	}

	/* Public method called by pathfinding to indicate request completed */

	public void PathIsProcessed(Vector3[] path, bool success) {

		/* The path received it sent back into the callback function along with the boolean claiming success */

		currentRequest.callback(path,success);
		instance.DoneProcessingPath = false;
		ProcessNext();
	}


	/* Structure is used to represent a single pathfinding request */

	struct PathRequest {
		
		public Vector3 start;					// Represents the transform to start from
		public Vector3 end;						// Represents the transform of the target
		public Action<Vector3[], bool> callback;	// This is callback function which will be called after the request is actioned

		// Constructor

		public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback) {
			this.start = start;
			this.end = end;
			this.callback = callback;
		}

	}
}