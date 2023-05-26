using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRays : MonoBehaviour
{
    private ObjectFader fader;

    private GameObject lastHit = null;

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("maid");

        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null)
                    return;

                if (hit.collider.gameObject == player || (lastHit != null && hit.collider.gameObject != lastHit))
                {
                    lastHit = null;
                    if (fader != null)
                    {
                        fader.doFade = false;
                    }
                }
                else
                {
                    lastHit = hit.collider.gameObject;
                    fader = lastHit.GetComponent<ObjectFader>();
                    if (fader != null)
                    {
                        fader.doFade = true;
                    }
                }
            }
        }
    }
}
