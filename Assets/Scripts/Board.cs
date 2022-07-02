using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Board
{
    public static readonly int LENGTH_X = 4;
    public static readonly int LENGTH_Y = 8;
    public static readonly int LENGTH = LENGTH_X * LENGTH_Y;
    public static readonly int MID_OF_BOARD = LENGTH / 2;

    public int[] board;
    private static int[][] tilesToEdge;
    private static readonly int[] directionOffset = new int[] {LENGTH_X, -LENGTH_X, -1, 1, LENGTH_X - 1, LENGTH_X + 1, -LENGTH_X - 1, -LENGTH_X + 1};

    public List<int> p1Captures;
    public List<int> p2Captures;

    public bool playerOneTurn;
    private (int from, int to) lastCrossedBorder;

    public Board(int[] board)
    {
        this.board = board;
        PopulateTilesToEdge();
        p1Captures = new List<int>();
        p2Captures = new List<int>();
    }

    public Board()
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
        PopulateTilesToEdge();
        p1Captures = new List<int>();
        p2Captures = new List<int>();
        playerOneTurn = true;
    }

    public int Get(int i) => board[i];

    public List<int> GetLegalMoves(int boardPos)
    {
        int type = board[boardPos];
        List<int> legalMoves = new();

        bool canPromoteToQueen = CanPromoteTo(3);

        if (type == 1)
        {
            for (int i = 4; i < directionOffset.Length; i++)
            {
                if (tilesToEdge[boardPos][i] > 0 && !IsTakebackMove(boardPos, boardPos + directionOffset[i]))
                {
                    if (board[boardPos + directionOffset[i]] == 0)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                    else if (boardPos < MID_OF_BOARD && boardPos + directionOffset[i] >= MID_OF_BOARD)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                    else if (boardPos >= MID_OF_BOARD && boardPos + directionOffset[i] < MID_OF_BOARD)
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }
                    else if ((CanPromoteTo(2) && board[boardPos + directionOffset[i]] == 1) || (canPromoteToQueen && board[boardPos + directionOffset[i]] == 2))
                    {
                        legalMoves.Add(boardPos + directionOffset[i]);
                    }

                }
            }
        }
        else if (type == 2)
        {
            for (int i = 0; i < 4; ++i)
            {
                int pos = boardPos;
                int startPos = boardPos;
                int j = 0;
                while (tilesToEdge[pos][i] > 0 && j < 2)
                {

                    if (IsTakebackMove(startPos, pos + directionOffset[i])) { }
                    else if (board[pos + directionOffset[i]] == 0)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                    }
                    else if (startPos < MID_OF_BOARD && pos + directionOffset[i] >= MID_OF_BOARD)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else if (startPos >= MID_OF_BOARD && pos + directionOffset[i] < MID_OF_BOARD)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else if (canPromoteToQueen && board[pos + directionOffset[i]] == 1)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else
                        break; 

                    pos += directionOffset[i];
                    ++j;
                }
            }
        }
        else if (type == 3)
        {
            for (int i = 0; i < directionOffset.Length; ++i)
            {
                int pos = boardPos;
                int startPos = boardPos;
                while (tilesToEdge[pos][i] > 0)
                {
                    if (IsTakebackMove(startPos, pos + directionOffset[i])) { }
                    else if (board[pos + directionOffset[i]] == 0)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                    }
                    else if (startPos < MID_OF_BOARD && pos + directionOffset[i] >= MID_OF_BOARD)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else if (startPos >= MID_OF_BOARD && pos + directionOffset[i] < MID_OF_BOARD)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else break;
                    pos += directionOffset[i];
                }
            }
        }

        //could remove take back moves from list here for readability

        return legalMoves;
    }

    private bool CanPromoteTo(int piece)
    {
        if (playerOneTurn)
        {
            for (int i = 0; i < MID_OF_BOARD; ++i)
                if (board[i] == piece)
                    return false;
        }
        else
        {
            for (int i = MID_OF_BOARD; i < LENGTH; ++i)
                if (board[i] == piece)
                    return false;
        }
        return true;
    }



    public void MakeMove(int from, int to)
    {
        int pieceType = board[from];

        if (board[to] != 0) PieceCaptured(to);
        if (CrossedBorder(from, to))
        {
            lastCrossedBorder.from = from;
            lastCrossedBorder.to = to;
        }
        else
        {
            lastCrossedBorder.from = -1;
            lastCrossedBorder.to = -1;
        }

        board[from] = 0;
        board[to] = pieceType;

        playerOneTurn = !playerOneTurn;

        //Debug.Log(PrintBoard());
    }


    public void PieceCaptured(int captred)
    {
        if (captred < MID_OF_BOARD)
        {
            p2Captures.Add(captred);
            //p1Captures.ForEach(p => Debug.Log(p));
            //Debug.Log("add " + p1Captures.Count);
        }
        else
        {
            p1Captures.Add(captred);
            //p2Captures.ForEach(p => Debug.Log(p));
        }
    }

    public bool IsValidPiece(int pieceIndex)
    {
        if ((playerOneTurn && pieceIndex < MID_OF_BOARD) || (!playerOneTurn && pieceIndex >= MID_OF_BOARD))
            return true;
        return false;
    }

    private bool CrossedBorder(int from, int to) =>
        (from < MID_OF_BOARD && to >= MID_OF_BOARD) || (from >= MID_OF_BOARD && to < MID_OF_BOARD);

    private bool IsTakebackMove(int from, int to)
    {
        //Debug.Log(from + " " + to + "  last " + lastCrossedBorder.to + " " + lastCrossedBorder.from);
        return from == lastCrossedBorder.to && to == lastCrossedBorder.from;
    }

    private static void PopulateTilesToEdge()
    {
        tilesToEdge = new int[32][];
        for (int x = 0; x < LENGTH_X; ++x)
        {
            for (int y = 0; y < LENGTH_Y; ++y)
            {
                int tilesUp = LENGTH_Y - 1 - y;
                int tilesDown = y;
                int tilesLeft = x;
                int tilesRight = LENGTH_X - 1 - x;

                int boardIndex = y * LENGTH_X + x;

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
        
        string boardString = "";
        for (int n = 0; n < tilesToEdge.Length; n++)
        {
            boardString += string.Format("Row({0}): ", n);
            for (int k = 0; k < tilesToEdge[n].Length; k++)
            {
                boardString += string.Format("{0} ", tilesToEdge[n][k]);
            }
            boardString += string.Format("\n");
        }
        Debug.Log(boardString);
        
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


}
