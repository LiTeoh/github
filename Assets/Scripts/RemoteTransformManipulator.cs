using UnityEngine;
using TMPro;

/*
USAGE 1: If you want a 'grab' based handle to control the target object:
  a. Attach this to a gameobject with a mesh, mesh renderer and collider.
  b. Add an XRGrabInteractable and set 
     - Select event to ActivateHandle()
     - Deleect event to ReleaseHandle()
  c. Assign the right or left controller to targetController
  d. Assign a target object.
  e. Set the type of manipulator by setting the handle bools
  f. Set the speed and min/max of the moving/scaling (these WILL be divided by frame rate)
  g. Set the height offset for re-scaling (you may need to experiment relative to player/ground height)
  h. You may want to create a GameObject > XR > UI Canvas with child TextMeshPro text elements for the in-game debugging

USAGE 2: If you wish to modify the target object with simple buttons etc:
   a. Attach this to a gameObject in your scene
   b. Assign a target
   c. Assign your speed and min/max for moving/scaling (these WON'T be divided by frame rate)
   d. Any UI button, simple interactor select event etc, can just call one of the public methods

NOTE/SUGGESTIONS: 
- You could add 'button' functionality, e.g. sounds of button clicks, animations of buttons moving etc
- Good practice would be to separate this script into multiple scripts, 
    e.g. one for grab handles, one for buttons etc
- You also could change the UI buttons to sliders...
- You may wish to explore ways of checking which targetController is gripping 
     the manipulator rather than setting a single preferred one in the inspector...
*/

public class RemoteTransformManipulator : MonoBehaviour
{
    // Drag the controller (i.e. right or left hand) you wish to trigger/pick up the target object with here
    public GameObject targetController;

    // What gameobject is the handle manipulating?
    public GameObject targetObject = null;

    // the boolean (true or false) indication about what this handle is manipulating: movement, rotation and/or scale
    public bool movementHandle = true;
    public bool rotateHandle = false;
    public bool scaleHandle = false;

    // The multiplier (i.e. sensitivity) of the handle, i.e. how much more/less does the target move compared to the handle
    public float moveMultiplier = 1f;
    public float rotateMultiplier = 10f;
    public float scaleMultiplier = 1f;

    // to avoid scale going below 0 and getting so big it clips the player add a min and max scale for the target
    public float minScale = .1f;
    public float maxScale = 2f;
    // this value can be used to modify the default controller's height value (y) used to define the target's height
    public float scaleOffset = -.5f;

    public TextMeshProUGUI debugUI;
    public TextMeshProUGUI moveUI;
    public TextMeshProUGUI rotateUI;
    public TextMeshProUGUI scaleUI;
    public TextMeshProUGUI resetUI;

    // The manipulator grip will only work while it's operating (triggered by ActivateHandle() and ReleaseHandle()
    private bool isOperating = false;

    // The handle's starting transform (so we can reset it on release)
    private Vector3 targetStartPosition;

    // the handle's starting Transform information (for comparison with moved position, as well as to reset it)
    private Vector3 handleDefaultPosition;
    private Vector3 handleStartingPosition;

    private void Start()
    {
        // Set the UI info to default text
        ResetUIText();

        // Set up handle's values if you use one
        InitialiseHandle();
    }

    private void ResetUIText()
    {
        // the null checks prevent runtime errors if you forgot to create a UI canvas and assign text fields
        if (debugUI != null) { 
            debugUI.text = "Debug: nothing to report!"; }
        if (moveUI != null) { 
            moveUI.text = "Not using yellow cube"; }
        if (rotateUI != null) { 
            rotateUI.text = "Not using orange cube";}
        if (scaleUI != null) { 
            scaleUI.text = "Not using red cube";}
        if (debugUI != null) { 
            scaleUI.text = "Not using black cube";}
    }


    //***************************************************************************************
    // GRIP-BASED METHODS FOR TRANSFORM MANIPULATION

    private void InitialiseHandle()
    {
        // Store the handle's starting position 
        handleDefaultPosition = transform.position;
        handleStartingPosition = transform.position;

        // if you forgot to assign a target object, create one automatically to avoid runtime errors!
        if (targetObject == null)
        {
            targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Store the target's starting position in case we wish to reset it
        targetStartPosition = targetObject.transform.position;
    }

    private void Update()
    {
        // skip this update if this handle is not in operation 
        if (!isOperating) return;

        // ...otherwise check for manipulation
        if (movementHandle)
        {
            if (debugUI != null) { 
                debugUI.text = "Moving target!"; }
            MoveTarget();
        }
        if (rotateHandle)
        {
            if (debugUI != null) { 
                debugUI.text = "Rotating target!"; }
            RotateTarget();
        }
        if (scaleHandle)
        {
            if (debugUI != null) { 
                debugUI.text = "Rescaling target!"; }
            ScaleTarget();
        }
    }

    // Calculates the target's new position as offset from the handle's starting position (x,y,z axes)
    private void MoveTarget()
    {
        // Reset target's rotation to avoid weird movement
        targetObject.transform.rotation = new Quaternion();

        // if the cube is left or right of start position, move target at move speed
        if (transform.position.x < handleStartingPosition.x)
        {
            targetObject.transform.Translate(-moveMultiplier * Time.deltaTime, 0, 0);
        }
        else if (transform.position.x > handleStartingPosition.x)
        {
            targetObject.transform.Translate(moveMultiplier * Time.deltaTime, 0, 0);
        }

        // if the cube is forward or backward of start position, move target at move speed
        if (transform.position.z < handleStartingPosition.z)
        {
            targetObject.transform.Translate(0, 0, -moveMultiplier * Time.deltaTime);
        }
        else if (transform.position.z > handleStartingPosition.z)
        {
            targetObject.transform.Translate(0, 0, moveMultiplier * Time.deltaTime);
        }

        if (moveUI != null) { 
            moveUI.text = "Yellow cube start pos: " + handleStartingPosition + " curr pos: " + transform.position; }
    }

    // Calculates the target's new position by this object's rotation
    private void RotateTarget()
    {
        // change the target's rotation to the handle's rotation
        targetObject.transform.localRotation = transform.localRotation;

        if (rotateUI != null) {
            rotateUI.text = "Brown rotate by cube angle: " + transform.localRotation; }
    }

    // Calculates the target's new scale dynamically based on the handle's movement)
    private void ScaleTarget()
    {
        float distance;

        // scale to object height
        distance = (transform.position.y + scaleOffset) * scaleMultiplier;

        // make sure distance is clamped between min and max scale
        distance = Mathf.Clamp(distance, minScale, maxScale);

        // generalise scale to x, y and z from the distance value
        Vector3 newScale = new Vector3(distance, distance, distance);

        // set the cube's scale to the newScale
        targetObject.transform.localScale = newScale;

        if (scaleUI != null) {
            scaleUI.text = "Red scale by cube y offset: " + distance; }
    }

    // If you wish to reset the target to its starting position, rotation and size, call this method
    public void ResetTargetObject()
    {
        if (resetUI != null) {
            resetUI.text = "Reset target with black cube!";  }

        // Moves the target to its initial position, with zero rotation, and default scale of 1
        targetObject.transform.position = targetStartPosition;
        targetObject.transform.rotation = new Quaternion();
        targetObject.transform.localScale = new Vector3(1f, 1f, 1f); //targetStartTransform.localScale;
    }

    // Call this method to start moving the cube with the handle
    public void ActivateHandle()
    {
        // To avoid runtime errors, if you have no controller assigned, the functions quits without activating 
        if (targetController == null)
        {
            if (debugUI != null)  {
                debugUI.text = "Debug: You don't have a controller assigned!";  }
            return;
        }

        // getting handle 'starting point' as where it is grabbed by controller
        handleStartingPosition = targetController.transform.position;

        if (debugUI != null) {
            debugUI.text = "Debug: Selected " + gameObject.name; }

        isOperating = true;
    }

    // Call this method when a handle is released to reset the handle and its manipulation booleans
    public void ReleaseHandle()
    {
        if (debugUI != null)   {
            debugUI.text = "Debug: Deselected " + gameObject.name; } 

        // reset the handle to its starting position and default rotation
        transform.position = handleDefaultPosition;
        transform.rotation = new Quaternion();

        isOperating = false;

        ResetUIText();
    }


    //*********************************************************************************
    // BUTTON-BASED METHODS FOR TRANSFORM MANIPULATION (add or extend as needed)

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
}
