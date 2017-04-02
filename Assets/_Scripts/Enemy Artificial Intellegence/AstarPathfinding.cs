using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* This is the algorithm which makes all the Astar pathfinding calculations */

public class AstarPathfinding : MonoBehaviour {

	PathfindingRequestManager requestHandler;				// Holds the reference to path finding request manager
	Grid grid;										// Holds the reference to grid drawn over the whole game level

	/* Instantiated.. gets all the components needed */

	void Awake() {
		requestHandler = GetComponent<PathfindingRequestManager>();
		grid = GetComponent<Grid>();
	}

	/* Public method called by pathfinding manager to search for the path */

	public void StartFindingPath(Vector3 startPosition, Vector3 targetPosition) {

		// calls a coroutine which will do the calculation for it..

		StopCoroutine("SearchPath");
		StartCoroutine(SearchPath(startPosition,targetPosition));
	}

	/* This IEnumnerator is what will find the best possible path to the target */

	IEnumerator SearchPath(Vector3 startPosition, Vector3 targetPosition) {

		Vector3[] points = new Vector3[0];					// Creates an array of vectors holding waypoint positions
		bool pathFound = false;								// Decides weather pathfinding was a success or not

		Node initialNode = grid.GetNodeFrom3DPosition(startPosition);		// Gets the start node position from the game world perspective on the grid
		Node endNode = grid.GetNodeFrom3DPosition(targetPosition);	// Gets the target node position from the game world perspective on the grid

		// First checks if start node and end node are walkable postions

		if (initialNode.isWalkable && endNode.isWalkable) {

			//Creates a Heap of size of the grid i.e GridX * GridY

			List<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();

			//Starts by adding the start node into the heap

			openSet.Add(initialNode);

			// Don't stop until open set is empty

			while (openSet.Count > 0) {

				// Removes the node with the lowest f cost from the Open set and adds it to the closed set

				Node curNode = openSet[0];
				for (int count = 1; count < openSet.Count; count ++) {
					if (openSet[count].f < curNode.f || openSet[count].f == curNode.f) {
						if (openSet[count].h < curNode.h)
							curNode = openSet[count];
					}
				}

				openSet.Remove(curNode);
				closedSet.Add(curNode);

				// If current node is equal to the target node we know that path is sucessful and we break from while loop

				if (curNode == endNode) {
					pathFound = true;
					break;
				}

				//Loop through all the neighbours around current node

				foreach (Node neighbour in grid.RetrieveNeighbours(curNode)) {

					// If it is marked as unmarkable or is already in the closed set ignore it, to the next..

					if (!neighbour.isWalkable || closedSet.Contains(neighbour)) {
						continue;
					}

					// Get the distance from current node to neighbour and calculate the G cost from current to neighbour..

					int MovementCostForNeighbour = curNode.g + RetrieveDistance(curNode, neighbour);

					// If g cost is less than the neighbour g cost or its not in the open set..  
					if (MovementCostForNeighbour < neighbour.g || !openSet.Contains(neighbour)) {

						// Assign newMovementCostToNeighbour to Neighbours g cost
						neighbour.g = MovementCostForNeighbour;

						// Also calculate the h cost from the distance between target node and neighbour
						neighbour.h = RetrieveDistance(neighbour, endNode);
						neighbour.parent = curNode;

						// If not in the open set add it to it.. if not update it..

						if (!openSet.Contains (neighbour))
							openSet.Add (neighbour);
					}
				}
			}
		}
		yield return null;

		// Path was successful retrace path..

		if (pathFound) {
			points = RetraceThePath(initialNode,endNode);
		}

		//Notify request manager with new path 

		requestHandler.PathIsProcessed(points,pathFound);

	}

	/* Used to retrace the path and return waypoints */

	Vector3[] RetraceThePath(Node initialNode, Node targetNode) {

		List<Node> path = new List<Node>();    		// Represents the nodes in a list
		Node curNode = targetNode;			   	 // Current node is set to end node passed as argument

		// Loop through all the nodes and their parents to draw the path until start node is equal to current node

		while (curNode != initialNode) {
			path.Add(curNode);
			curNode = curNode.parent;
		}
			
		Vector3[] points = SimplifyThePath(path);

		//Reverse the order and send it back.

		Array.Reverse(points);
		return points;

	}

	/* Function will simplfy the path */

	Vector3[] SimplifyThePath(List<Node> path) {
		
		List<Vector3> points = new List<Vector3>();
		Vector2 oldDirection = Vector2.zero;

		// loops through the paths and creates a new waypoint vector array which only input those nodes that have path changing direction 

		for (int count = 1; count < path.Count; count ++) {

			Vector2 newDirection = new Vector2(path[count-1].X_gridPosition - path[count].X_gridPosition,path[count-1].Y_gridPosition - path[count].Y_gridPosition);

			// if direction has changed add node to the waypoint list
			if (newDirection != oldDirection) {
				points.Add(path[count].positionIn3D);
			}
			oldDirection = newDirection;
		}

		// Convert to an array and return..

		return points.ToArray();
	}

	/* Returns the distance between the 2 nodes in the grid */

	int RetrieveDistance(Node nodeX, Node nodeY) {

		// Calculate difference between in grid X and grid y from Node A and Node B

		int distanceX = Mathf.Abs(nodeX.X_gridPosition - nodeY.X_gridPosition);
		int distanceY = Mathf.Abs(nodeX.Y_gridPosition - nodeY.Y_gridPosition);

		// X distance is higher do 14 * Y + 10 (X - Y)

		if (distanceX > distanceY)

			return 14*distanceY + 10* (distanceX-distanceY);

		// Or else do 14 * X + 10 (Y - X)

		return 14*distanceX + 10 * (distanceY-distanceX);
	}


}