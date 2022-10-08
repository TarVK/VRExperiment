using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class InitSceneV2 : MonoBehaviour
{
    public int NrTilesHeight;
    public int NrTilesWidth;
    public float TileSize;

    public GameObject pointer;
    public GameObject pointingHighlighter;
    public Vector3 pointerDir = new Vector3(0, 1f, -1);
    public GameObject headset;

    public bool downwardsWalls;

    private HeadLogger logger;
    private GameObject[] walls;
    private GameObject target;
    private Object[] textures;
    private Material wallMaterial;
    private Material targetMaterial;

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

        // define materials
        targetMaterial = new Material(Shader.Find("Diffuse"));
        targetMaterial.mainTexture = textures[0] as Texture2D;

        wallMaterial = new Material(Shader.Find("Diffuse"));
        wallMaterial.mainTexture = textures[1] as Texture2D;
        wallMaterial.mainTextureScale = new Vector2(NrTilesWidth, NrTilesHeight);

        // create walls
        walls = new GameObject[4];
        for (int w = 0; w < 4; w++) {
            // Place wall relative to script object
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wall.transform.parent = gameObject.transform;
            wall.transform.localPosition = new Vector3(0,0,0);

            if (downwardsWalls) {
                wall.transform.Translate(0, 0, (NrTilesWidth / 2) * TileSize);
            }
            else {
                wall.transform.Translate(0, (NrTilesHeight / 2) * TileSize, (NrTilesWidth / 2) * TileSize);
            }
            
            wall.transform.localScale = new Vector3(TileSize * NrTilesWidth * 0.1f, 1f, TileSize * NrTilesHeight * 0.1f);
            wall.transform.localRotation = Quaternion.Euler(90, 0, 180);
            wall.transform.RotateAround(gameObject.transform.position, Vector3.up, 90 * w);

            wall.tag = "not target";
            walls[w] = wall;
        }

        // TEMP (I can't start the game without controller)
        setSearching(true);
    }

    void initializeSymbols()
    {
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<Renderer>().material = wallMaterial;
        }

        // create target tile
        int wall_id = Random.Range(0, 4);
        int row = Random.Range(0, NrTilesHeight);
        int col = Random.Range(0, NrTilesWidth);
        Debug.Log(wall_id);
        Debug.Log(row);
        Debug.Log(col);
        
        target = GameObject.CreatePrimitive(PrimitiveType.Plane);
        target.transform.parent = gameObject.transform;
        target.transform.localPosition = new Vector3(0,0,0);

        // Move target to correct position on wall
        if (downwardsWalls) {
            target.transform.Translate((col + 0.5f - NrTilesWidth / 2) * TileSize, 
                                    (row + 0.5f - NrTilesHeight / 2) * TileSize, 
                                    (NrTilesWidth / 2) * TileSize - 0.001f);
        }
        else {
            target.transform.Translate((col + 0.5f - NrTilesWidth / 2) * TileSize, 
                                    (row + 0.5f) * TileSize, 
                                    (NrTilesWidth / 2) * TileSize - 0.001f);
        }
                          
        target.transform.localScale = new Vector3(TileSize * 0.1f, TileSize * 0.1f, TileSize * 0.1f);
        target.transform.localRotation = Quaternion.Euler(90, 0, 180);
        target.transform.RotateAround(gameObject.transform.position, Vector3.up, 90 * wall_id);

        target.GetComponent<Renderer>().material = targetMaterial;
        target.tag = "target";
    }

    void hideSymbols()
    {
        Material material = new Material(Shader.Find("Diffuse"));
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<Renderer>().material = material;
        }
        
        Destroy(target);
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
