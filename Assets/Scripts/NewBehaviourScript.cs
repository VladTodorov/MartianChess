using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    private static readonly int BOARD_LENGTH_X = 4;
    private static readonly int BOARD_LENGTH_Y = 8;
    private static readonly int MID_OF_BOARD = BOARD_LENGTH_Y / 2 * BOARD_LENGTH_X;

    private int[] board;
    private static int[][] tilesToEdge;
    private static readonly int[] directionOffset = new int[] { BOARD_LENGTH_X, -BOARD_LENGTH_X, -1, 1, BOARD_LENGTH_X - 1, BOARD_LENGTH_X + 1, -BOARD_LENGTH_X + 1, -BOARD_LENGTH_X - 1 };

    public Material[] lightSquareMaterial;
    public Material[] darkSquareMaterial;
    [SerializeField] private Transform cam;
    public GameObject[,] boardObject;

    public GameObject queenObject;
    public GameObject bishopObject;
    public GameObject pawnObject;

    private void Start()
    {
        cam.transform.position = new Vector3((float)BOARD_LENGTH_X/2 - 0.5f, (float)BOARD_LENGTH_Y/2 - 0.5f, -10f);
        GenerateTiles(BOARD_LENGTH_X, BOARD_LENGTH_Y);
        GeneratePieces();
        PopulateTilesToEdge();
    }


    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    List<int> legalMoves = null;

    void Update()
    {

        //UsingMouse();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (selectedPiece != null)
                HighlightLegalMoves(selectedPiece, 0);

            GameObject touchInput = GetTouch();
            if (touchInput == null)
            {
                selectedPiece = null;
            }
            else if (selectedPiece == null && touchInput.CompareTag("Piece"))
            {
                selectedPiece = touchInput;

                legalMoves = HighlightLegalMoves(selectedPiece, 1);
            }
            else if (touchInput != selectedPiece && selectedPiece != null)
            {
                if(IsLegalMove(touchInput, legalMoves))
                    selectedPieceMoveTo = touchInput;
                else
                    selectedPiece = null;

                legalMoves = null;
            }
        }

        if (selectedPiece != null && selectedPieceMoveTo != null)
        {
            //selectedPiece.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];
            //selectedPieceMoveTo.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];
            //Debug.Log(selectedPiece.name + " ---- " + selectedPieceMoveTo.name);
            
            UpdateBoard(selectedPiece.transform.position, selectedPieceMoveTo.transform.position);
            
            selectedPiece.transform.position = selectedPieceMoveTo.transform.position;

            selectedPiece = null;
            selectedPieceMoveTo = null;
        }

    }


    private List<int> HighlightLegalMoves(GameObject piece, int shade)
    {
        int boardPos = VectorToOneD(piece.transform.position);

        List<int> legalMoves = GetLegalMoves(boardPos);

        foreach (int t in legalMoves)
        {
            //add layermask to tiles/pieces
            Vector3 origin, direction;
            origin = OneDToVector(t);
            direction = new Vector3(0, 0, -1);
            origin.z = 1;

            //Debug.Log(origin);
            //Debug.Log(direction);

            int tileBitmask = 1 << 6;

            Physics.Raycast(origin, direction, out RaycastHit hit, 2.0f, tileBitmask);
            Debug.DrawRay(origin, direction, Color.yellow, 15, false);

            //Debug.Log(hit.collider.gameObject.name);

            SetTileMaterial(hit.collider.gameObject, shade);
        }
        return legalMoves;

    }

    private List<int> GetLegalMoves(int boardPos)
    {
        int type = board[boardPos];

        List<int> legalMoves = new();

        if (type == 1)
        {
            for (int i = 4; i < directionOffset.Length; i++)
            {
                if (tilesToEdge[boardPos][i] > 0)
                {
                    if (board[boardPos + directionOffset[i]] == 0)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                    else if (boardPos < MID_OF_BOARD && boardPos + directionOffset[i] > MID_OF_BOARD)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                    else if (boardPos > MID_OF_BOARD && boardPos + directionOffset[i] < MID_OF_BOARD)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                }
            }
        }

        return legalMoves;
    }

    private bool IsLegalMove(GameObject touchInput, List<int> legalMoves)
    {
        int pos = VectorToOneD(touchInput.transform.position);   
        foreach (int t in legalMoves)
            if (t == pos)
                return true;

        return false;
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
        tile.layer = LayerMask.NameToLayer("Tiles");

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
            GeneratePiece(board[i], i);        
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
            piece.layer = LayerMask.NameToLayer("Pieces");
        }
        else if (type == 2)
        {
            piece = Instantiate(bishopObject, new Vector3(x, y, -1), Quaternion.identity);
            piece.tag = "Piece";
            piece.layer = LayerMask.NameToLayer("Pieces");
        }
        else if (type == 3)
        {
            piece = Instantiate(queenObject, new Vector3(x, y, -1), Quaternion.identity);
            piece.tag = "Piece";
            piece.layer = LayerMask.NameToLayer("Pieces");
        }

    }

    private static void PopulateTilesToEdge()
    {
        tilesToEdge = new int[32][];
        for (int x = 0; x < BOARD_LENGTH_X; ++x)
        {
            for (int y = 0; y < BOARD_LENGTH_Y; ++y)
            {
                int tilesUp = BOARD_LENGTH_Y - y;
                int tilesDown = y;
                int tilesLeft = x;
                int tilesRight = BOARD_LENGTH_X - x;

                int boardIndex = y * BOARD_LENGTH_X + x;

                tilesToEdge[boardIndex] = new int[] {
                    tilesUp,
                    tilesDown,
                    tilesLeft,
                    tilesRight,
                    Math.Min(tilesUp, tilesLeft),
                    Math.Min(tilesUp, tilesRight),
                    Math.Min(tilesDown, tilesLeft),
                    Math.Min(tilesDown, tilesRight),
                };
            }
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
        return (int)vect.y * BOARD_LENGTH_X + (int)vect.x;
    }

    private Vector3 OneDToVector(int t)
    {
        Vector3 result = new (t % BOARD_LENGTH_X, t / BOARD_LENGTH_X, 0);
        return result;
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
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 50.0f))
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
