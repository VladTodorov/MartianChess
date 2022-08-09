using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int from;
    public int to;
    public int piece;
    
    public int capture;
    public int playerNum;

    public (int from, int to) lastCrossedBorder;
    public int movesSinceLastCapture;

    public Move (int from, int to, Board board)
    {
        this.from = from;
        this.to = to;
        this.capture = board.Get(to);
        this.piece = board.Get(from);

        if (board.playerOneTurn)
            this.playerNum = 1;
        else
            this.playerNum = 2;

        this.lastCrossedBorder = (board.lastCrossedBorder.from, board.lastCrossedBorder.to);
        this.movesSinceLastCapture = board.movesSinceLastCapture;
    }
}
