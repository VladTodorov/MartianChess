using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Material[] darkSquareMaterial;
    public Material[] lightSquareMaterial;



    private GameObject[,] board;

    private void Awake()
    {
        GenerateTiles(4, 8);
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
        if((x+y)%2 == 0)
            tile.GetComponent<MeshRenderer>().material = lightSquareMaterial[0];
        else
            tile.GetComponent<MeshRenderer>().material = darkSquareMaterial[0];

        tile.transform.parent = transform;
        //tile.transform.Rotate(90.0f, 0.0f, 0.0f);
        tile.transform.position = new Vector3(x-1.5f, y+0.5F, 0.0f);
        tile.AddComponent<BoxCollider>();




        return tile;
    }

}
