using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mirror_texture_change : MonoBehaviour
{
    public float texture_change_frequency = 2.0f;
    float time_since_change = 0.0f;
    public Material[] textures = new Material[4];
    int textureIndex = 0;
    public SkinnedMeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time_since_change += Time.deltaTime;
        if(time_since_change >= texture_change_frequency)
        {
            update_texture();
            time_since_change = 0.0f;
        }
    }

    void update_texture()
    {
        Debug.Log("Update texture");
        Material tex = textures[textureIndex];
        //meshRenderer.
        meshRenderer.materials[1] = tex;
        if(textureIndex < textures.Length - 1)
        {
            textureIndex++;
        }
    }
}
