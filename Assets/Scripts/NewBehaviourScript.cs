using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NewBehaviourScript : MonoBehaviour
{
    public Material[] darkSquareMaterial;
    public Material[] lightSquareMaterial;
    [SerializeField] private Transform cam;
    private GameObject[,] board;

    private readonly int BOARD_LENGTH_X = 4;
    private readonly int BOARD_LENGTH_Y = 8;

    private void Awake()
    {
        cam.transform.position = new Vector3((float)BOARD_LENGTH_X/2 - 0.5f, (float)BOARD_LENGTH_Y/2 - 0.5f, -10f);
        GenerateTiles(BOARD_LENGTH_X, BOARD_LENGTH_Y);
        //GeneratePieces();
    }


    void Update()
    {
        //UsingMouse();
    }


    private void GenerateTiles(int boardWidth, int boardHight)
    {
        board = new GameObject[boardWidth, boardHight];

        for (int y = 0; y < boardHight; y++)
            for (int x = 0; x < boardWidth; x++)
                board[x, y] = GenerateTile(x, y);
        
    }

    private GameObject GenerateTile(int x, int y)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tile.name = string.Format("{0} {1}", x, y);
        tile.transform.parent = transform;

        SetTileMaterial(tile, 0);

        //tile.transform.Rotate(90.0f, 0.0f, 0.0f);
        tile.transform.position = new Vector3(x, y, 0.0f);
        tile.AddComponent<BoxCollider>();

        return tile;
    }

    private void SetTileMaterial(GameObject tile, int shade)
    {
        int[] tilePos = tile.name.Split(' ').Take(2).Select(int.Parse).ToArray();

        if ((tilePos[0] + tilePos[1]) % 2 == 0)
            tile.GetComponent<MeshRenderer>().material = lightSquareMaterial[shade];
        else
            tile.GetComponent<MeshRenderer>().material = darkSquareMaterial[shade];

    }




    private GameObject currHighlightedTile;
    private void UsingMouse()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            SetTileMaterial(hit.collider.gameObject, 1);

            if (currHighlightedTile == null)
            {
                currHighlightedTile = hit.collider.gameObject;
            }
            else if (currHighlightedTile != hit.collider.gameObject)
            {
                SetTileMaterial(currHighlightedTile, 0);
                currHighlightedTile = hit.collider.gameObject;
            }
        }
        else
        {
            if (currHighlightedTile != null)
            {
                SetTileMaterial(currHighlightedTile, 0);
            }
        }
    }



}
