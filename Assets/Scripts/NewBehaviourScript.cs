using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    public Board board;

    [Header("Materials")]
    public Material[] lightSquareMaterial;
    public Material[] darkSquareMaterial;

    [Header("Prefabs")]
    public GameObject queenObject;
    public GameObject bishopObject;
    public GameObject pawnObject;

    [SerializeField] private Transform cam;
    public GameObject[,] boardObject;

    private void Start()
    {
        board = new Board();
        cam.transform.position = new Vector3((float)Board.LENGTH_X / 2 - 0.5f, (float)Board.LENGTH_Y / 2 - 0.5f, -10f);
        
        GameObject pieceLookAt = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pieceLookAt.transform.position = new Vector3((float)Board.LENGTH_X / 2 - 0.5f, (float)Board.LENGTH_Y / 2 - 0.5f, 3f);
        pieceLookAt.tag = "PieceLookAt";

        GenerateTiles(Board.LENGTH_X, Board.LENGTH_Y);
        GeneratePieces();
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
            //print(touchInput.name);

            if (touchInput == null || touchInput == selectedPiece)
            {
                selectedPiece = null;
            }
            else if (selectedPiece == null && IsValidPiece(touchInput))
            {
                selectedPiece = touchInput;
                legalMoves = HighlightLegalMoves(selectedPiece, 1);
            }
            else if (selectedPiece != null)
            {
                if(legalMoves.Contains(VectorToOneD(touchInput.transform.position)))
                    selectedPieceMoveTo = touchInput;
                else
                    selectedPiece = null;

                legalMoves = null;
            }
        
        }

        if (selectedPiece != null && selectedPieceMoveTo != null)
        {
            MakeMove(selectedPiece, selectedPieceMoveTo);

            selectedPiece = null;
            selectedPieceMoveTo = null;
        }

    }

    private void MakeMove(GameObject fromObj, GameObject toObj)
    {
        int from = VectorToOneD(fromObj.transform.position);
        int to = VectorToOneD(toObj.transform.position);
        
        board.MakeMove(from, to);

        if (toObj.CompareTag("Piece"))
        {
            fromObj.GetComponent<Piece>().SetPosition(toObj.transform.position);

            if (board.IsOnSameSide(from, to))
            {
                Destroy(fromObj);
                Destroy(toObj);

                GeneratePiece(board.PiecePromote(from, to), to);
                //print(board.PiecePromote(from, to));
            }
            else
            {
                PieceCaptured(toObj);
            }
        }
        else
        {
            fromObj.GetComponent<Piece>().SetPosition(toObj.transform.position + new Vector3(0, 0, -1));
            //selectedPiece.transform.position = selectedPieceMoveTo.transform.position + new Vector3(0,0,-1);
        }

    }

    private void PieceCaptured(GameObject captured)
    {
        int pos = VectorToOneD(captured.transform.position);
        //board.PieceCaptured(pos);

        captured.transform.localScale = captured.transform.localScale / 1.7f; ;
        
        if (pos < Board.MID_OF_BOARD)
        {
            captured.GetComponent<Piece>().SetPosition(new Vector3(-1, 8 - board.p2Captures.Count / 2f, -1));
            //captured.transform.position = new Vector3(-1, 8 - board.p2Captures.Count/2f, -1);
        }
        else
        {
            captured.GetComponent<Piece>().SetPosition(new Vector3(4, (board.p1Captures.Count - 1) / 2f, -1));
            //captured.transform.position = new Vector3(4, (board.p1Captures.Count - 1)/2f , -1);
        }
        
    }

    private bool IsValidPiece(GameObject touchInput)
    {
        if(!touchInput.CompareTag("Piece")) return false;

        int pieceIndex = VectorToOneD(touchInput.transform.position);

        return board.IsValidPiece(pieceIndex);
    }

    private List<int> HighlightLegalMoves(GameObject piece, int shade)
    {
        int boardPos = VectorToOneD(piece.transform.position);

        List<int> legalMoves = board.GetLegalMoves(boardPos);

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
        for (int i = 0; i < Board.LENGTH; i++)
            GeneratePiece(board.Get(i), i);        
    }

    private void GeneratePiece(int type, int pos)
    {
        int x = pos % Board.LENGTH_X;
        int y = pos / Board.LENGTH_X;

        GameObject piece;
        
        if (type == 1)
        {
            piece = Instantiate(pawnObject, new Vector3(x, y, -0.5f), Quaternion.identity);
            piece.tag = "Piece";
            piece.layer = LayerMask.NameToLayer("Pieces");
            //piece.AddComponent<Piece>();
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
        // Construct a ray from touch coordinates
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if (Physics.Raycast(ray, out RaycastHit hit, 20.0f))
            return hit.collider.gameObject;
        else
            return null;
    }

    private int VectorToOneD(Vector3 vect) => (int)vect.y * Board.LENGTH_X + (int)vect.x;

    private Vector3 OneDToVector(int t) => new(t % Board.LENGTH_X, t / Board.LENGTH_X, 0);
   

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
