using UnityEngine;
using System.Collections;

/* Used to represent each individual node created in the grid class */

public class Node {

	public bool isWalkable;					// Represents whether a node is walkable or not
	public Vector3 positionIn3D;			// Represents the postion of the node on the game level
	public int X_gridPosition;				// Represents the X position on the grid
	public int Y_gridPosition;				// Represents the Y position on the grid

	public int g;						// Represents the g cost of the node
	public int h;						// Represents the h cost of the node
	public Node parent;					// Holds reference to its parent node used when AStar mapping is done

	/* Constructor */

	public Node(bool _isWalkable, Vector3 _3DPos, int x, int y) {
		isWalkable = _isWalkable;
		positionIn3D = _3DPos;
		X_gridPosition = x;
		Y_gridPosition = y;
	}

	/* Property representing the f cost */

	public int f {
		get {
			return g + h;
		}
	}

}