using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // for (int wall_id = 0; wall_id < 4; wall_id++) {
        //     for (int row = 0; row < NrTilesInHeight; row++) {
        //         for (int col = 0; col < NrTilesInWidth; col++) {
        //             GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //         }
        //     }
        // }

        // var tiles = GameObject.FindGameObjectsWithTag("img_tile");
        // foreach (var t in tiles) {
        //     var textures = Resources.LoadAll(@"TestImages", typeof(Texture2D));
        //     Debug.Log("#Textures: " + textures.Length);
        //     if (textures.Length == 0) return;
            
        //     Texture2D texture  = (Texture2D)(textures[Random.Range(0, textures.Length)]);
        //     Material material = new Material(Shader.Find("Diffuse"));
        //     material.mainTexture = texture;
        //     t.GetComponent<Renderer>().material = material;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
