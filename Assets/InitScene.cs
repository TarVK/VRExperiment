using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class InitScene : MonoBehaviour
{
    public int NrTilesHeight;
    public int NrTilesWidth;
    public float TileSize;
    public bool randomTarget;
    public bool downwardsWalls;
    // Start is called before the first frame update
    void Start()
    {
        var textures = Resources.LoadAll(@"TestImages", typeof(Texture2D));
        if (textures.Length == 0) { 
            Debug.LogWarning("No tile textures found. Abort scene initialization"); 
            return; 
        }
        if (textures.Length == 1) { 
            Debug.LogWarning("At least two images needes, one image found. Abort scene initialization"); 
            return; 
        }

        // select target image
        int target_id = 0;
        if (randomTarget) target_id = Random.Range(0, textures.Length);

        Texture2D target_texture = textures[target_id] as Texture2D;
        Debug.Log(target_texture.name);

        List<Texture2D> other_textures = new List<Texture2D>();
        for (int i = 0; i < textures.Length; i++) {
            if (i != target_id) other_textures.Add(textures[i] as Texture2D);
        }

        int target_wall = 0;
        // int target_wall = Random.Range(0, 4);
        int target_row = Random.Range(0, NrTilesHeight);
        int target_col = Random.Range(0, NrTilesWidth);

        for (int wall_id = 0; wall_id < 4; wall_id++) {
            for (int row = 0; row < NrTilesHeight; row++) {
                for (int col = 0; col < NrTilesWidth; col++) {

                    // Place tile at script object
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    tile.transform.parent = gameObject.transform;
                    tile.transform.localPosition = new Vector3(0,0,0);

                    // Move tile to correct position on wall
                    if (downwardsWalls) {
                        tile.transform.Translate((col + 0.5f - NrTilesWidth / 2) * TileSize, 
                                                (row + 0.5f - NrTilesHeight / 2) * TileSize, 
                                                (NrTilesWidth / 2) * TileSize);
                    }
                    else {
                        tile.transform.Translate((col + 0.5f - NrTilesWidth / 2) * TileSize, 
                                                (row + 0.5f) * TileSize, 
                                                (NrTilesWidth / 2) * TileSize);
                    }
                                        
                    // Scale tile
                    tile.transform.localScale = new Vector3(TileSize * 0.1f, TileSize * 0.1f, TileSize * 0.1f);

                    // Orient tile 
                    tile.transform.localRotation = Quaternion.Euler(90, 0, 180);

                    // Rotate around script position for each wall
                    tile.transform.RotateAround(gameObject.transform.position, Vector3.up, 90 * wall_id);
                    
                    // Apply random texture
                    Texture2D texture;
                    if (target_wall == wall_id && target_row == row && target_col == col) {
                        texture  = target_texture;
                        tile.tag = "target";
                    }
                    else {
                        texture  = (Texture2D)(other_textures[Random.Range(0, other_textures.Count)]);
                        tile.tag = "not target";
                    }
                    
                    Material material = new Material(Shader.Find("Diffuse"));
                    material.mainTexture = texture;
                    tile.GetComponent<Renderer>().material = material;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: raycast for VR controller?
        if( Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;
            
            if( Physics.Raycast( ray, out hit, 100 ) )
            {
                if (hit.transform.gameObject.tag == "target") {
                    Debug.Log("Target found!");

                    // TODO: end / reset game
                    var tiles = GameObject.FindGameObjectsWithTag("not target");
                    foreach (var t in tiles) {
                        Destroy(t);
                    }
                }
                else {
                    Debug.Log("Invalid target");
                }
            }
        }
    }
}
