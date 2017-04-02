using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Used to draw the grid of nodes for path finding */

public class Grid : MonoBehaviour {

	public bool showGizmos;						// Controls whether to disply the pathfinding gizmo or not
	public LayerMask unwalkableLayerMask;		// Represents the layer mask for all the unwalkable nodes
	public Vector2 gridSize;					// Vector2 which will hold the grid 2d size 
	public float pointRadius;					// Represents the radius of the node
	Node[,] grid;								// Holds the whole list of array of nodes in the grid

	float pointDiameter;						// Represents the diameter of the node
	int SizeOnX, SizeOnY;					    // Represents the X and Y size of the grid that is the dimensions 

	/* Instantition is done here, the grid is split into nodes depending on the radius and grid size */

	void Awake() {
		pointDiameter = pointRadius*2;
		SizeOnX = Mathf.RoundToInt(gridSize.x/pointDiameter);
		SizeOnY = Mathf.RoundToInt(gridSize.y/pointDiameter);
		GenerateGrid();
	}

	/* Method is responsible for creating the grid */

	void GenerateGrid() {

		// Grid is created here

		grid = new Node[SizeOnX,SizeOnY];

		// Calculates the bottom left position in 3D space of the game

		Vector3 gameBottomLeft = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.y/2;

		// Loops through the grid using X and Y

		for (int x = 0; x < SizeOnX; x ++) {

			for (int y = 0; y < SizeOnY; y ++) {

				// Calculates worldpostion of each node
				Vector3 Pos3D = gameBottomLeft + Vector3.right * (x * pointDiameter + pointRadius) + Vector3.forward * (y * pointDiameter + pointRadius);

				// Checks collision and returns a value to walkable
				bool isWalkable = !(Physics.CheckSphere(Pos3D,pointRadius,unwalkableLayerMask));

				// Creates a node in the grid and passes all the attained arguments
				grid[x,y] = new Node(isWalkable,Pos3D, x,y);
			}
		}
	}


	/* Gets the list of neighbours around the node passed as argument */

	public List<Node> RetrieveNeighbours(Node node) {
		
		List<Node> neighbours = new List<Node>();	// Holds the list neighbours

		// Walks into nested for loops to get the neighbours around that node

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {

				// Ignores if its the position of the current node in question

				if (x == 0 && y == 0)
					continue;

				int validateX = node.X_gridPosition + x;
				int validateY = node.Y_gridPosition + y;

				// Ignores if neighbour is outside the grid

				if (validateX >= 0 && validateX < SizeOnX && validateY >= 0 && validateY < SizeOnY) {
					neighbours.Add(grid[validateX,validateY]);
				}
			}
		}

		// Returns the found set of neighbours

		return neighbours;
	}

	/* Calculcates which node it from the world position passed as arguments */

	public Node GetNodeFrom3DPosition(Vector3 Pos3D) {

		float percentAtX = (Pos3D.x + gridSize.x/2) / gridSize.x;
		float percentAtY = (Pos3D.z + gridSize.y/2) / gridSize.y;
		percentAtX = Mathf.Clamp01(percentAtX);
		percentAtY = Mathf.Clamp01(percentAtY);

		int xOnGrid = Mathf.RoundToInt((SizeOnX-1) * percentAtX);
		int yOnGrid = Mathf.RoundToInt((SizeOnY-1) * percentAtY);
		return grid[xOnGrid,yOnGrid];
	}

	/* This method here is just to show the visuals on how the grid has been drawn on the game level */

	void OnDrawGizmos() {

		// Draws the grid..
		Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,1,gridSize.y));

		// Colors the nodes depending upon collision..

		if (showGizmos && grid != null) {
			foreach (Node node in grid) {
				Gizmos.color = (node.isWalkable)?Color.green:Color.red;
				Gizmos.DrawCube(node.positionIn3D, Vector3.one * (pointDiameter-.1f));
			}
		}
	}
}