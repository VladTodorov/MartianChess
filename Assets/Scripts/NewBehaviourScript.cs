using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        tile.name = string.Format("{0}, {1}", x, y);
        tile.transform.parent = transform;

        if ((x + y) % 2 == 0)
            tile.GetComponent<MeshRenderer>().material = lightSquareMaterial[0];
        else
            tile.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];

        //tile.transform.Rotate(90.0f, 0.0f, 0.0f);
        tile.transform.position = new Vector3(x, y, 0.0f);
        tile.AddComponent<BoxCollider>();


        return tile;
    }



}
