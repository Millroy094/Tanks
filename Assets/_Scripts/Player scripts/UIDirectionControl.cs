using UnityEngine;


/* Controls the rotation of the health slider UI to match with the tank unit*/

public class UIDirectionControl : MonoBehaviour
{
    public bool UseRelativeRotation = true;  


    private Quaternion RelativeRotation;     

	/* Local rotation is retrieved */

    private void Start()
    {
        RelativeRotation = transform.parent.localRotation;
    }

	/* And set to the UI transform rotation */

    private void Update()
    {
        if (UseRelativeRotation)
            transform.rotation = RelativeRotation;
    }
}
