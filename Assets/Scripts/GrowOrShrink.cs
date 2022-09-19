using UnityEngine;

// Attach this to a gameObject
// Define you minScale and maxScale values (you will also need to check if the object is already rescaled in the scene!)
// An external script/event can access the StartShrinking() or StartGrowing() methods to start shrinking or growing
public class GrowOrShrink : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 2f;

    public Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    bool isShrinking = false;
    bool isGrowing = false;

    private void Start()
    {
        StartGrowing();
    }

    // if the object is in shrink or grow mode, it will call the methods to incrementally increase/decrease the object's size
    private void Update()
    {
        if (isShrinking)
        {
            ShrinkObject();
        }
        else if (isGrowing)
        {
            GrowObject();
        }
    }

    // this public method simply sets the bools to shrinking mode
    public void StartShrinking()
    {
        isShrinking = true;
        isGrowing = false;
    }

    // this public method simply sets the bools to growing mode
    public void StartGrowing()
    {
        isGrowing = true;
        isShrinking = false;
    }

    // this script will incrementally reduce the object's size
    private void ShrinkObject()
    {
        transform.localScale -= scaleChange;

        // if the object is at it's min size, stop shrinking!
        if (transform.localScale.y < minScale)
        {
            isShrinking = false;
        }
    }

    public void ShrinkObjectAnIncrement()
    {
        // if the object is at it's min size, stop shrinking!
        if (transform.localScale.y > minScale)
        {
            transform.localScale -= scaleChange;
        }
    }

    // this script will incrementally increase the object's size 
    private void GrowObject()
    {
        transform.localScale += scaleChange;

        // if the object is at it's max size, stop growing!
        if (transform.localScale.y > maxScale)
        {
            isGrowing = false;
        }
    }

    public void GrowObjectAnIncrement()
    {
        // if the object is at it's min size, stop shrinking!
        if (transform.localScale.y < maxScale)
        {
            transform.localScale += scaleChange;
        }
    }

}
