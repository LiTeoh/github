using UnityEngine;

// attach this to an object with a trigger, when it hits the player, it will rescale
public class TouchGrowandShrink : MonoBehaviour
{
    // minumum and maximum scale for the object
    public float minScale = 0.5f;
    public float maxScale = 2f;

    // if negative values, it will shrink first; if positive, it will grow first
    public Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Hand")
        {
            transform.localScale += scaleChange;

            if (transform.localScale.y < minScale || transform.localScale.y > maxScale)
            {
                scaleChange = -scaleChange;
            }
        }
    }
}
