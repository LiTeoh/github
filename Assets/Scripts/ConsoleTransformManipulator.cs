using UnityEngine;
using TMPro;

/*
 * If you wish to modify the target object with simple buttons etc:
   a. Attach this to a gameObject in your scene
   b. Assign a target
   c. Assign your speed and min/max for moving/scaling (these WON'T be divided by frame rate)
   d. Any UI button, simple interactor select event etc, can just call one of the public methods

NOTE/SUGGESTIONS: 
- You could add 'button' functionality, e.g. sounds of button clicks, animations of buttons moving etc
- You also could change the UI buttons to sliders...
*/

public class ConsoleTransformManipulator : MonoBehaviour
{
    // What gameobject is the handle manipulating?
    public GameObject targetObject = null;

    // The multiplier (i.e. sensitivity) of the handle, i.e. how much more/less does the target move compared to the handle
    public float moveMultiplier = 1f;
    public float rotateMultiplier = 10f;
    public float scaleMultiplier = 1f;

    // to avoid scale going below 0 and getting so big it clips the player add a min and max scale for the target
    public float minScale = .1f;
    public float maxScale = 2f;

    private void Start()
    {
        // if you forgot to assign a target object, create one automatically to avoid runtime errors!
        if (targetObject == null)
        {
            targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Store the target's starting position in case we wish to reset it
        targetStartPosition = targetObject.transform.position;
    }

    // The handle's starting transform (so we can reset it on release)
    private Vector3 targetStartPosition;

    public void MoveTargetLeft()
    {
        targetObject.transform.Translate(-moveMultiplier, 0, 0);
    }

    public void MoveTargetRight()
    {
        targetObject.transform.Translate(moveMultiplier, 0, 0);

    }

    public void MoveTargetForward()
    {
        targetObject.transform.Translate(0, 0, moveMultiplier);

    }

    public void MoveTargetBackward()
    {
        targetObject.transform.Translate(0, 0, -moveMultiplier);
    }

    public void RotateTargetLeft()
    {
        targetObject.transform.Rotate(0, rotateMultiplier, 0);
    }

    public void RotateTargetRight()
    {
        targetObject.transform.Rotate(0, -rotateMultiplier, 0);
    }

    public void RescaleTargetBigger()
    {
        // get the new scale 
        float newScale = targetObject.transform.localScale.x + scaleMultiplier;

        // make sure distance is clamped between min and max scale
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        // generalise scale to x, y and z from the distance value
        Vector3 newScaleVector = new Vector3(newScale, newScale, newScale);

        // set the cube's scale to the newScale
        targetObject.transform.localScale = newScaleVector;
    }

    public void RescaleTargetSmaller()
    {
        // get the new scale 
        float newScale = targetObject.transform.localScale.x - scaleMultiplier;

        // make sure distance is clamped between min and max scale
        newScale = Mathf.Clamp(newScale, minScale, maxScale);

        // generalise scale to x, y and z from the distance value
        Vector3 newScaleVector = new Vector3(newScale, newScale, newScale);

        // set the cube's scale to the newScale
        targetObject.transform.localScale = newScaleVector;
    }

    // If you wish to reset the target to its starting position, rotation and size, call this method
    public void ResetTargetObject()
    {
        // Moves the target to its initial position, with zero rotation, and default scale of 1
        targetObject.transform.position = targetStartPosition;
        targetObject.transform.rotation = new Quaternion();
        targetObject.transform.localScale = new Vector3(1f, 1f, 1f); //targetStartTransform.localScale;
    }
}
