using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRays : MonoBehaviour
{
    private ObjectFader fader;

    private GameObject lastHit = null;
    private Renderer lastHitRenderer = null;
    private GameObject player = null;
    private CharacterController charController = null;

    void Start()
    {
        player = GameObject.Find("maid");
        charController =  GameObject.Find("Player").GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player != null)
        {
            // Ray cast to player
            Vector3 offset = charController.center * player.transform.localScale.y;
            Vector3 dir = player.transform.position + offset - transform.position;
            Ray ray = new Ray(transform.position , dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null)
                    return;
                
                GameObject hitGameObject = hit.collider.gameObject;

                if (hitGameObject == player || (lastHit != null && hitGameObject != lastHit))
                {
                    lastHit = null;
                    if (fader != null)
                    {
                        fader.doFade = false;
                    }

                    if (lastHitRenderer)
                    {
                        lastHitRenderer.enabled = true;
                        lastHitRenderer = null;
                    }
                }
                else if (hitGameObject.layer == 0 && hitGameObject.tag != "Player") // Default Layer
                {
                    Renderer currRenderer = hitGameObject.GetComponentInChildren<Renderer>();
                    if (lastHitRenderer && lastHitRenderer != currRenderer)
                        lastHitRenderer.enabled = true;
                    lastHitRenderer = currRenderer;
                    lastHitRenderer.enabled = false;
                }
                else
                {
                    if (lastHitRenderer)
                        lastHitRenderer.enabled = true;
                    lastHit = hitGameObject;
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
