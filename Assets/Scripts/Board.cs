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

    public int? winner;
    public List<int> p1Captures;
    public List<int> p2Captures;

    public bool playerOneTurn;
    public (int from, int to) lastCrossedBorder;
    public int movesSinceLastCapture;
    private List<Move> moves;

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
        /*board = new int[]
        {
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 3, 0,
            0, 0, 0, 0,

            0, 0, 0, 0,
            0, 1, 1, 2,
            0, 1, 2, 3,
            0, 2, 3, 3,
        };
        board = new int[]
        {
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 3, 0,
            0, 0, 0, 0,

            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 3, 0,
            0, 0, 0, 0,
        };*/
        PopulateTilesToEdge();
        p1Captures = new List<int>();
        p2Captures = new List<int>();
        playerOneTurn = true;
        winner = null;
        moves = new List<Move>();
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
                    else if (!IsOnSameSide(boardPos, boardPos + directionOffset[i]))
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
                    else if (!IsOnSameSide(startPos, pos + directionOffset[i]))
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else if (canPromoteToQueen && board[pos + directionOffset[i]] == 1)
                    {
                        legalMoves.Add(pos + directionOffset[i]);
                        break;
                    }
                    else break; 

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
                    else if (!IsOnSameSide(startPos, pos + directionOffset[i]))
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



    public void MakeMove(int from, int to)
    {
        int pieceType = board[from];

        ++movesSinceLastCapture;

        if (board[to] != 0)
        {
            if (IsOnSameSide(from, to))
            {
                board[to] = PiecePromote(from, to);
                board[from] = 0;
            }
            else
            {
                PieceCaptured(to);
                board[from] = 0;
                board[to] = pieceType;
                movesSinceLastCapture = 0;
            }
        }
        else
        {
            board[from] = 0;
            board[to] = pieceType;
        }

        if (!IsOnSameSide(from, to))
        {
            lastCrossedBorder.from = from;
            lastCrossedBorder.to = to;
        }
        else
        {
            lastCrossedBorder.from = -1;
            lastCrossedBorder.to = -1;
        }

        
        if (CheckGameOver())
            SetWinner();

        playerOneTurn = !playerOneTurn;

        //Debug.Log(movesSinceLastCapture);
        //Debug.Log(PrintBoard());
    }

    public Move MakeMove(Move move)
    {
        int pieceType = board[move.from];
        ++movesSinceLastCapture;

        if (board[move.to] != 0)
        {
            move.capture = board[move.to];
            if (IsOnSameSide(move.from, move.to))
            {
                board[move.to] = PiecePromote(move.from, move.to);
                board[move.from] = 0;
            }
            else
            {
                PieceCaptured(move.to);
                board[move.from] = 0;
                board[move.to] = pieceType;
                movesSinceLastCapture = 0;
            }
        }
        else
        {
            board[move.from] = 0;
            board[move.to] = pieceType;
        }

        // border crossed
        if (!IsOnSameSide(move.from, move.to))
        {
            lastCrossedBorder.from = move.from;
            lastCrossedBorder.to = move.to;
        }
        else
        {
            lastCrossedBorder.from = -1;
            lastCrossedBorder.to = -1;
        }

        if (CheckGameOver())
            SetWinner();

        playerOneTurn = !playerOneTurn;

        //Debug.Log(PrintBoard());

        return move;

        //Debug.Log(movesSinceLastCapture);
    }


    public void UndoMove(Move move)
    {
        playerOneTurn = !playerOneTurn;

        if (CheckGameOver())
            winner = null;


        board[move.from] = move.piece;
        board[move.to] = move.capture;

        //Debug.Log(move.capture);

        if (move.capture != 0 && move.playerNum == 1)
        {
            if (p1Captures.Any())
                p1Captures.RemoveAt(p1Captures.Count - 1);
        }
        else if (move.capture != 0 && move.playerNum == 2)
        {
            if (p2Captures.Any())
                p2Captures.RemoveAt(p2Captures.Count - 1);
        }
            


        lastCrossedBorder = (move.lastCrossedBorder.from, move.lastCrossedBorder.to);
        movesSinceLastCapture = move.movesSinceLastCapture;

        //Debug.Log(movesSinceLastCapture);
        //Debug.Log(PrintBoard());
    }


    public bool CheckGameOver()
    {
        bool gameOver = true;
        for (int i = 0; i < MID_OF_BOARD; i++)
        {
            if (board[i] != 0)
            { 
                gameOver = false;
                break;
            }
        }

        if (!gameOver)
        {
            gameOver = true;
            for (int i = MID_OF_BOARD; i < LENGTH; i++)
            {
                if (board[i] != 0)
                {
                    gameOver = false;
                    break;
                }
            }
        }

        //Debug.Log(movesSinceLastCapture);

        if (!gameOver)
            if(movesSinceLastCapture > 14)
                gameOver = true;

        return gameOver;
    }

    private int SetWinner()
    {
        int p1Points = p1Captures.AsQueryable().Sum();
        int p2Points = p2Captures.AsQueryable().Sum();

        if (p1Points == p2Points)    // fix to total moves
            winner = movesSinceLastCapture % 2 == 0 ? 2 : 1;
        else if (p1Points > p2Points)
            winner = 1;
        else
            winner = 2;


        return (int)winner;
    }

    public int PiecePromote(int from, int to)
    {
        if (from == 2 || to == 2)
            return 3;
        else
            return 2;
    }

    public void PieceCaptured(int captred)
    {
        //Debug.Log(captred);
        if (playerOneTurn)
            p1Captures.Add(board[captred]);
        else
            p2Captures.Add(board[captred]);
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

    public bool IsValidPiece(int pieceIndex) =>
        (playerOneTurn && pieceIndex < MID_OF_BOARD) || (!playerOneTurn && pieceIndex >= MID_OF_BOARD);

    public static bool IsValidPiece(int pieceIndex, int playerNum) =>
        (playerNum == 1 && pieceIndex < MID_OF_BOARD) || (playerNum == 2 && pieceIndex >= MID_OF_BOARD);

    public bool IsOnSameSide(int pos1, int pos2) =>
        (pos1 < MID_OF_BOARD && pos2 < MID_OF_BOARD) || (pos1 >= MID_OF_BOARD && pos2 >= MID_OF_BOARD);
    
    private bool IsTakebackMove(int from, int to) =>
        from == lastCrossedBorder.to && to == lastCrossedBorder.from;

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
        //Debug.Log(boardString);
        
    }

    //Debug
    public string PrintBoard()
    {
        string boardString = "\n";
        for (int i = 1; i <= board.Length; i++)
        {
            if (i % 4 == 0)
                boardString += string.Format("{0}\n", board[i - 1]);
            else
                boardString += string.Format("{0} ", board[i - 1]);
        }
        return boardString;
    }

    public string PrintCaptures(List<int> cap)
    {
        string capString = "captures:  ";

        foreach(int i in cap)
        {
            capString += string.Format("{0} ", i);
        }
        return capString;
    }


}
