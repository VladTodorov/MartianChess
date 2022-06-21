using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NewBehaviourScript : MonoBehaviour
{
    private int[] board;

    public Material[] lightSquareMaterial;
    public Material[] darkSquareMaterial;
    [SerializeField] private Transform cam;
    public GameObject[,] boardObject;

    public GameObject queenObject;
    public GameObject bishopObject;
    public GameObject pawnObject;

    private readonly int BOARD_LENGTH_X = 4;
    private readonly int BOARD_LENGTH_Y = 8;

    private void Start()
    {
        cam.transform.position = new Vector3((float)BOARD_LENGTH_X/2 - 0.5f, (float)BOARD_LENGTH_Y/2 - 0.5f, -10f);
        GenerateTiles(BOARD_LENGTH_X, BOARD_LENGTH_Y);
        GeneratePieces();
    }


    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;

    void Update()
    {

        //UsingMouse();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            GameObject touchInput = GetTouch();
            if (touchInput == null)
            {
                selectedPiece = null;
            }
            else if (selectedPiece == null && touchInput.CompareTag("Piece"))
            {
                selectedPiece = touchInput;

                // add highlight possible moves for piece here
                // need a getPosition function
            }
            else if (touchInput != selectedPiece && selectedPiece != null)
            {
                selectedPieceMoveTo = touchInput;
                Debug.Log(selectedPiece.name + " ==== ");
            }
        }

        if (selectedPiece != null && selectedPieceMoveTo != null)
        {
            selectedPiece.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];
            //selectedPieceMoveTo.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];
            
            Debug.Log(selectedPiece.name + " ---- " + selectedPieceMoveTo.name);
            UpdateBoard(selectedPiece.transform.position, selectedPieceMoveTo.transform.position);
            
            selectedPiece.transform.position = selectedPieceMoveTo.transform.position;

            selectedPiece = null;
            selectedPieceMoveTo = null;
        }

    }

    private void UpdateBoard(Vector3 from, Vector3 to)
    {
        int initialPosOneD = VectorToOneD(from);
        int finalPosOneD = VectorToOneD(to);
        int pieceType = board[initialPosOneD];

        board[initialPosOneD] = 0;
        board[finalPosOneD] = pieceType;

        Debug.Log(PrintBoard());
    }




    //Init
    private void GenerateTiles(int boardWidth, int boardHight)
    {
        boardObject = new GameObject[boardWidth, boardHight];

        for (int y = 0; y < boardHight; y++)
            for (int x = 0; x < boardWidth; x++)
                boardObject[x, y] = GenerateTile(x, y);
        
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

    private void GeneratePieces()
    {
        board = new int[]
        {
            3, 3, 2, 0,
            3, 2, 1, 0,
            2, 1, 1, 0,
            0, 0, 0, 0,

            0, 0, 0, 0,
            0, 1, 1, 2,
            0, 1, 2, 3,
            0, 2, 3, 3,
        };


        for (int i = 0; i < board.Length; i++)
        {
            GeneratePiece(board[i], i);
        }
        
    }

    private void GeneratePiece(int type, int pos)
    {
        int x = pos % BOARD_LENGTH_X;
        int y = pos / BOARD_LENGTH_X;

        GameObject piece;
        
        if (type == 1)
        {
            piece = Instantiate(pawnObject, new Vector3(x, y, -1), Quaternion.identity);
            piece.tag = "Piece";
        } else if (type == 2)
        {
            piece = Instantiate(bishopObject, new Vector3(x, y, -1), Quaternion.identity);
            piece.tag = "Piece";
        } else if (type == 3)
        {
            piece = Instantiate(queenObject, new Vector3(x, y, -1), Quaternion.identity);
            piece.tag = "Piece";
        }

    }


    //Helper
    private void SetTileMaterial(GameObject tile, int shade)
    {
        int[] tilePos = tile.name.Split(' ').Take(2).Select(int.Parse).ToArray();

        if ((tilePos[0] + tilePos[1]) % 2 == 0)
            tile.GetComponent<MeshRenderer>().material = lightSquareMaterial[shade];
        else
            tile.GetComponent<MeshRenderer>().material = darkSquareMaterial[shade];

    }

    private GameObject GetTouch()
    {
        //gets phone touch input and highlights selected tile

        // Construct a ray from touch coordinates
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out RaycastHit hit, 20.0f))
        {
            //SetTileMaterial(hit.collider.gameObject, 1);
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }

    }

    private int VectorToOneD(Vector3 vect)
    {
        return (int)vect.y * 4 + (int)vect.x;
    }


    //Debug
    private string PrintBoard()
    {
        string boardString = "";
        for (int i = 1; i <= board.Length; i++)
        {
            if (i % 4 == 0)
                boardString += string.Format("{0}\n", board[i - 1]);
            else
                boardString += string.Format("{0} ", board[i - 1]);
        }
        return boardString;
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
