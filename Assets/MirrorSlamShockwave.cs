using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorSlamShockwave : MonoBehaviour
{
    float startingWidth = 0.75f;
    float endingWidth = 4f;
    float duration = 0.5f;
    float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(startingWidth, transform.localScale.y, startingWidth);
    }

    public void Initialize(float sw, float ew, float d)
    {
        startingWidth = sw;
        endingWidth = ew;
        duration = d;
    }

    //Starts with x and z scale = startingWidth and grows to endingWidth over the course of the duration
    void FixedUpdate()
    {
        float currentSize = Mathf.Lerp(startingWidth, endingWidth, t);

        transform.localScale = new Vector3(currentSize, transform.localScale.y, currentSize);

        t += Time.fixedDeltaTime / duration;
        if (t > 1)
        {
            Destroy(gameObject);
        }
    }

    
}
