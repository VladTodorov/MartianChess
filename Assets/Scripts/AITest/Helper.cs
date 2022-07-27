using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



public class Helper : MonoBehaviour
{
    private Board board;
    //public GameOverScreen gameOverScreen;

    [Header("Materials")]
    public Material[] lightSquareMaterial;
    public Material[] darkSquareMaterial;

    [Header("Prefabs")]
    public GameObject queenObject;
    public GameObject bishopObject;
    public GameObject pawnObject;


    /*void Update()
    {

        //UsingMouse();
        if (board.winner != null)
        {
            StartCoroutine(GameOver());
            enabled = false;
        }

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
                if (legalMoves.Contains(VectorToOneD(touchInput.transform.position)))
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

    }*/

    public void SetBoard(Board _board) 
    {
        board = _board;
        board.PrintBoard();
    }

    private void Start() { }


    public void MakeMove(GameObject fromObj, GameObject toObj)
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
            fromObj.GetComponent<Piece>().SetPosition(toObj.transform.position + new Vector3(0, 0, -0.1f));
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
            captured.GetComponent<Piece>().SetPosition(new Vector3(-0.75f, 8 - board.p2Captures.Count / 2f, 0));
            //captured.transform.position = new Vector3(-1, 8 - board.p2Captures.Count/2f, -1);
        }
        else
        {
            captured.GetComponent<Piece>().SetPosition(new Vector3(3.75f, (board.p1Captures.Count - 1) / 2f, 0));
            //captured.transform.position = new Vector3(4, (board.p1Captures.Count - 1)/2f , -1);
        }

    }

    public bool IsValidPiece(GameObject touchInput)
    {
        if (!touchInput.CompareTag("Piece")) return false;

        int pieceIndex = VectorToOneD(touchInput.transform.position);

        return board.IsValidPiece(pieceIndex);
    }

    public List<int> HighlightLegalMoves(GameObject piece, int shade)
    {
        int boardPos = VectorToOneD(piece.transform.position);
        Vector3 direction = new(0, 0, -1);
        int tileBitmask = 1 << 6;

        List<int> legalMoves = board.GetLegalMoves(boardPos);

        Physics.Raycast(piece.transform.position + new Vector3(0, 0, 1), direction, out RaycastHit hit, 2.0f, tileBitmask);
        Debug.DrawRay(piece.transform.position + new Vector3(0, 0, 1), direction, Color.yellow, 15, false);

        if (shade != 0)
            SetTileMaterial(hit.collider.gameObject, shade + 2);
        else
            SetTileMaterial(hit.collider.gameObject, shade);

        foreach (int t in legalMoves)
        {
            //add layermask to tiles/pieces
            Vector3 origin = OneDToVector(t);
            origin.z = 1;

            //Debug.Log(origin);
            //Debug.Log(direction);

            Physics.Raycast(origin, direction, out RaycastHit hit2, 2.0f, tileBitmask);
            Debug.DrawRay(origin, direction, Color.yellow, 15, false);

            //Debug.Log(hit.collider.gameObject.name);

            if (board.Get(t) == 0 || shade == 0)
                SetTileMaterial(hit2.collider.gameObject, shade);
            else
                SetTileMaterial(hit2.collider.gameObject, shade + 1);

        }
        return legalMoves;

    }

    public void SetTileMaterial(GameObject tile, int shade)
    {
        int[] tilePos = tile.name.Split(' ').Take(2).Select(int.Parse).ToArray();

        if ((tilePos[0] + tilePos[1]) % 2 == 0)
            tile.GetComponent<MeshRenderer>().material = lightSquareMaterial[shade];
        else
            tile.GetComponent<MeshRenderer>().material = darkSquareMaterial[shade];
    }

    public GameObject GetTouch()
    {
        // Construct a ray from touch coordinates
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if (Physics.Raycast(ray, out RaycastHit hit, 20.0f))
            return hit.collider.gameObject;
        else
            return null;
    }

    public int VectorToOneD(Vector3 vect) => (int)vect.y * Board.LENGTH_X + (int)vect.x;

    public Vector3 OneDToVector(int t) => new(t % Board.LENGTH_X, t / Board.LENGTH_X, 0);


    public GameObject GenerateTile(int x, int y, Transform parent)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        tile.name = string.Format("{0} {1}", x, y);
        tile.transform.parent = parent;
        tile.layer = LayerMask.NameToLayer("Tiles");

        SetTileMaterial(tile, 0);

        //tile.transform.Rotate(90.0f, 0.0f, 0.0f);
        tile.transform.position = new Vector3(x, y, 0.0f);
        tile.AddComponent<BoxCollider>();

        return tile;
    }
    //add parent object for piece
    public void GeneratePiece(int type, int pos)
    {
        int x = pos % Board.LENGTH_X;
        int y = pos / Board.LENGTH_X;

        GameObject piece;

        if (type == 0) return;
        else if (type == 1)
        {
            piece = Instantiate(pawnObject, new Vector3(x, y, -0.1f), Quaternion.identity);
            piece.tag = "Piece";
        }
        else if (type == 2)
        {
            piece = Instantiate(bishopObject, new Vector3(x, y, -0.1f), Quaternion.identity);
            piece.tag = "Piece";
        }
        else
        {
            piece = Instantiate(queenObject, new Vector3(x, y, -0.1f), Quaternion.identity);
            piece.tag = "Piece";
        }

        piece.layer = LayerMask.NameToLayer("Pieces");


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
