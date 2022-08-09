using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotPlayer : MonoBehaviour
{
    public const string type = "Bot";

    public int playerNumber;

    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    //List<int> legalMoves = null;

    public IEnumerator MakeMove(Board board, Helper helper)
    {
        (Move move, int eval) = PickMove(board);
        print(eval);

        selectedPiece = helper.GetGameObject(move.from);
        selectedPieceMoveTo = helper.GetGameObject(move.to);

        helper.MakeMove(selectedPiece, selectedPieceMoveTo);
        yield break;
    }


    //private (int from, int to) PickMove(Board board) => (22, 19);

    private (Move move, int eval) PickMove(Board board)
    {
        List<Move> moves = AllLegalMoves(board);

        int maxEval = -10000000;
        Move bestMove = null;
        
        for(int i = 0; i < moves.Count; i++)
        //for(int i = 0; i < 1; i++)
        {
            moves[i] = board.MakeMove(moves[i]);
            int eval = MoveEvaluation(board, 1, true);
            board.UndoMove(moves[i]);

            //print(moves[i].from + " " + moves[i].to+ "  e "+ eval);

            if (eval > maxEval)
            {
                bestMove = moves[i];
                maxEval = eval;
                //print(moves[i].from + " " + moves[i].to + "  e " + eval);
            }
        }

        return (bestMove, maxEval);
    }


    private int MoveEvaluation(Board board, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || board.CheckGameOver())
        {
            return BoardEvaluation(board);
        }

        if (maximizingPlayer)
        {
            int maxEval = -10000;
            List<Move> moves = AllLegalMoves(board);

            for (int i = 0; i < moves.Count; i++)
            {
                moves[i] = board.MakeMove(moves[i]);
                int eval = MoveEvaluation(board, depth - 1, false);
                board.UndoMove(moves[i]);

                if (eval > maxEval)
                {
                    maxEval = eval;
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = 10000;
            List<Move> moves = AllLegalMoves(board);

            for (int i = 0; i < moves.Count; i++)
            {
                moves[i] = board.MakeMove(moves[i]);
                int eval = MoveEvaluation(board, depth - 1, false);
                board.UndoMove(moves[i]);

                if (eval < minEval)
                {
                    minEval = eval;
                }
            }
            return minEval;
        }

    }

    private int BoardEvaluation(Board board)
    {

        if (board.CheckGameOver())
        {
            if (board.winner == playerNumber) return 100;
            else return -100;
        }

        return PosNetPoints(board);
    }

    private List<Move> AllLegalMoves(Board board)
    {
        List<Move> moves = new();
        for(int i = 0; i < Board.LENGTH; ++i)
        {
            if(Board.IsValidPiece(i, playerNumber))
            {
                List<int> pieceMoves = board.GetLegalMoves(i);

                foreach (int moveTo in pieceMoves)
                    moves.Add(new Move(i, moveTo, board));
            }
        }

        //moves.Sort(); captures first

        return moves;
    }


    private int PosNetPoints(Board board)
    {
        int botPoints = 0;
        int opponentPoints = 0;

        if (playerNumber == 1)
        {
            botPoints = board.p1Captures.AsQueryable().Sum();
            opponentPoints = board.p2Captures.AsQueryable().Sum();
        }
        else if (playerNumber == 2)
        {
            opponentPoints = board.p1Captures.AsQueryable().Sum();
            botPoints = board.p2Captures.AsQueryable().Sum();
        }

        return botPoints - opponentPoints;
    }


    public void Moo() =>
        Debug.Log("Moo");

}
