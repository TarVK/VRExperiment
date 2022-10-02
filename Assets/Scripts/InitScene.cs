using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class InitScene : MonoBehaviour
{
    public int NrTilesHeight;
    public int NrTilesWidth;
    public float TileSize;

    public GameObject pointer;
    public GameObject pointingHighlighter;
    public Vector3 pointerDir = new Vector3(0, 1f, -1);
    public GameObject headset;

    public bool randomTarget;
    public bool downwardsWalls;

    private HeadLogger logger;
    private GameObject[,,] tiles;
    private Object[] textures;

    private bool searching = false;

    // Start is called before the first frame update
    void Start()
    {
        logger = headset.GetComponent<HeadLogger>();
        logger.randomSeedData = Random.Range(0, (int)1e8);
        Random.InitState(logger.randomSeedData);

        textures = Resources.LoadAll(@"TestImages", typeof(Texture2D));
        if (textures.Length == 0) { 
            Debug.LogWarning("No tile textures found. Abort scene initialization"); 
            return; 
        }
        if (textures.Length == 1) { 
            Debug.LogWarning("At least two images needes, one image found. Abort scene initialization"); 
            return; 
        }

        tiles = new GameObject[4, NrTilesHeight, NrTilesWidth];
        for (int wall_id = 0; wall_id < 4; wall_id++) {
            for (int row = 0; row < NrTilesHeight; row++) {
                for (int col = 0; col < NrTilesWidth; col++) {

                    // Place tile at script object
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    tiles[wall_id, row, col] = tile;
                    tile.transform.parent = gameObject.transform;
                    tile.transform.localPosition = new Vector3(0,0,0);

                    // Move tile to correct position on wall
                    if (downwardsWalls) {
                        tile.transform.Translate((col - NrTilesWidth / 2) * TileSize, 
                                                (row + 0.5f - NrTilesHeight / 2) * TileSize, 
                                                (NrTilesWidth / 2 + 0.5f) * TileSize);
                    }
                    else {
                        tile.transform.Translate((col - NrTilesWidth / 2) * TileSize, 
                                                (row + 0.5f) * TileSize, 
                                                (NrTilesWidth / 2 + 0.5f) * TileSize);
                    }
                                        
                    // Scale tile
                    tile.transform.localScale = new Vector3(TileSize * 0.1f, TileSize * 0.1f, TileSize * 0.1f);

                    // Orient tile 
                    tile.transform.localRotation = Quaternion.Euler(90, 0, 180);

                    // Rotate around script position for each wall
                    tile.transform.RotateAround(gameObject.transform.position, Vector3.up, 90 * wall_id);
                }
            }
        }
    }

    void initializeSymbols()
    {

        // select target image
        int target_id = 0;
        if (randomTarget) target_id = Random.Range(0, textures.Length);

        Texture2D target_texture = textures[target_id] as Texture2D;
        Debug.Log(target_texture.name);
        Material target_material = new Material(Shader.Find("Diffuse"));
        target_material.mainTexture = target_texture;

        List<Material> other_materials = new List<Material>();
        for (int i = 0; i < textures.Length; i++)
        {
            if (i != target_id)
            {
                Texture texture = textures[i] as Texture2D;
                Material material = new Material(Shader.Find("Diffuse"));
                material.mainTexture = texture;
                other_materials.Add(material);
            }
        }

        //int target_wall = 0;
        int target_wall = Random.Range(0, 4);
        int target_row = Random.Range(0, NrTilesHeight);
        int target_col = Random.Range(0, NrTilesWidth);
        Debug.Log("Wall " + target_wall + "row " + target_row + "col " + target_col);

        for (int wall_id = 0; wall_id < 4; wall_id++)
        {
            for (int row = 0; row < NrTilesHeight; row++)
            {
                for (int col = 0; col < NrTilesWidth; col++)
                {
                    GameObject tile = tiles[wall_id, row, col];

                    // Apply random texture
                    Material material;
                    if (target_wall == wall_id && target_row == row && target_col == col)
                    {
                        material = target_material;
                        tile.tag = "target";
                    }
                    else
                    {
                        material = other_materials[Random.Range(0, other_materials.Count)];
                        tile.tag = "not target";
                    }

                    tile.GetComponent<Renderer>().material = material;
                }
            }
        }
    }

    void hideSymbols()
    {
        Material material = new Material(Shader.Find("Diffuse"));
        for (int wall_id = 0; wall_id < 4; wall_id++)
        {
            for (int row = 0; row < NrTilesHeight; row++)
            {
                for (int col = 0; col < NrTilesWidth; col++)
                {
                    GameObject tile = tiles[wall_id, row, col];
                    tile.GetComponent<Renderer>().material = material;
                }
            }
        }
    }

    public void setSearching(bool searching)
    {
        if (this.searching == searching) return;

        this.searching = searching;
        logger.setActive(searching);
        if (searching)
        {
            initializeSymbols();
        } else
        {
            hideSymbols();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Manage pointing
        RaycastHit hit;
        Transform pointerTransform = pointer.transform;
        if (Physics.Raycast(pointerTransform.position, pointerTransform.TransformDirection(pointerDir.normalized), out hit, 200))
        {
            if (Input.GetButtonDown("XRI_Right_TriggerButton"))
            {
                if (hit.transform.gameObject.tag == "target")
                {
                    Debug.Log("Target found!");
                    setSearching(false);
                }
                else
                {
                    Debug.Log("Invalid target");
                }
            }


            pointingHighlighter.transform.position = hit.point;
            pointingHighlighter.SetActive(true);
        } else
        {
            pointingHighlighter.SetActive(false);
        }



        // Check for buttons
        if (Input.GetButtonDown("XRI_Right_PrimaryButton"))
            setSearching(true);
    }
}
