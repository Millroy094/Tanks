using UnityEngine;


/* Class is responsible for having the camera to give a full view of the game player with zooming in whener needed*/

public class CameraControl : MonoBehaviour
{
    public float DampTime = 0.2f;                 
    public float ScreenEdgeBuffer = 4f;           
    public float MinSize = 6.5f;                  
    [HideInInspector] public Transform[] Targets; 	// Represents the transforms tanks that will be followed and forcused on


    private Camera Camera;             				// Holds reference to the main camera           
    private float ZoomSpeed;						// Controls the zooming in and out speed                      
    private Vector3 MoveVelocity;                 	// Controls the velocity of the camera when moving
    private Vector3 DesiredPosition;  				// Tranform postion where to move to		            


    private void Awake()
    {
        Camera = GetComponentInChildren<Camera>(); 	//instantiates with the reference
    }

	/* Gets the camera to adjust itself with the action on game level */

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

	/* Controls the movement of the Camera */

    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref MoveVelocity, DampTime);
    }

	/* Calculates the average position the camera is meant to point to from the number of tanks out there */

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

		//Loops through all the transform positions and adds them up

        for (int i = 0; i < Targets.Length; i++)
        {
            if (!Targets[i].gameObject.activeSelf)
                continue;

            averagePos += Targets[i].position;
            numTargets++;
        }

		//Then divides it by the number of targets

        if (numTargets > 0)
            averagePos /= numTargets;

		//Y position is left untouched
        averagePos.y = transform.position.y;

		//Move position is updated
        DesiredPosition = averagePos;
    }

	/* Gets the camera to zoom in and out depending on the game level activity */

    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        Camera.orthographicSize = Mathf.SmoothDamp(Camera.orthographicSize, requiredSize, ref ZoomSpeed, DampTime);
    }

	/* Calcualtes the zoom for the camera */

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(DesiredPosition);

        float size = 0f;

        for (int i = 0; i < Targets.Length; i++)
        {
            if (!Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / Camera.aspect);
        }
        
        size += ScreenEdgeBuffer;

        size = Mathf.Max(size, MinSize);

        return size;
    }

	/* Used to initialize camera postion in the game */

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = DesiredPosition;

        Camera.orthographicSize = FindRequiredSize();
    }
}