using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotPlayer : MonoBehaviour
{
    public const string type = "Bot";

    public int playerNumber;
    public int opponentNumber;

    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    //List<int> legalMoves = null;

    public IEnumerator MakeMove(Board board, Helper helper)
    {
        //print("Bot Num: " + playerNumber);
        (Move move, int eval) = PickMove(board);
        //print(eval);
        print(board.PrintBoard());
        //print(move.from + " " + move.to + "  e " + eval);

        selectedPiece = helper.GetGameObject(move.from);
        selectedPieceMoveTo = helper.GetGameObject(move.to);

        helper.MakeMove(selectedPiece, selectedPieceMoveTo, move.from, move.to);
        board.MakeMove(move.from, move.to);

        print(board.PrintBoard());

        yield break;
    }


    //private (int from, int to) PickMove(Board board) => (22, 19);

    private (Move move, int eval) PickMove(Board board)
    {
        List<Move> moves = AllLegalMoves(board, true);

        int maxEval = -10000000;
        Move bestMove = null;
        
        for(int i = 0; i < moves.Count; i++)
        //for(int i = 0; i < 1; i++)
        {
            moves[i] = board.MakeMove(moves[i]);
            int eval = MoveEvaluation(board, 1, false);
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
            int eval = BoardEvaluation(board);
            //print(eval);
            return eval;
        }

        if (maximizingPlayer)
        {
            int maxEval = -10000;
            List<Move> moves = AllLegalMoves(board, maximizingPlayer);

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
            List<Move> moves = AllLegalMoves(board, maximizingPlayer);

            for (int i = 0; i < moves.Count; i++)
            {
                moves[i] = board.MakeMove(moves[i]);
                int eval = MoveEvaluation(board, depth - 1, true);
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
        //print("PosNetPoints: " + PosNetPoints(board));
        //print(board.PrintBoard());
        return PosNetPoints(board);
    }

    private List<Move> AllLegalMoves(Board board, bool forPlayer)
    {
        List<Move> moves = new();

        if (forPlayer)
        {
            for (int i = 0; i < Board.LENGTH; ++i)
            {
                if (Board.IsValidPiece(i, playerNumber))
                {
                    List<int> pieceMoves = board.GetLegalMoves(i);

                    foreach (int moveTo in pieceMoves)
                        moves.Add(new Move(i, moveTo, board));
                }
            }
        }
        else
        {
            for (int i = 0; i < Board.LENGTH; ++i)
            {
                if (Board.IsValidPiece(i, opponentNumber))
                {
                    List<int> pieceMoves = board.GetLegalMoves(i);

                    foreach (int moveTo in pieceMoves)
                        moves.Add(new Move(i, moveTo, board));
                }
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
        //print("bot: " + botPoints + "   opp: " + opponentPoints);
        //print(board.PrintCaptures(board.p2Captures));
        return botPoints - opponentPoints;
    }

    public void SetPlayerNumber (int playerNum)
    {
        if(playerNum == 1)
        {
            playerNumber = 1;
            opponentNumber = 2;
        }
        else
        {
            playerNumber = 2;
            opponentNumber = 1;
        }
    }


    public void Moo() =>
        Debug.Log("Moo");

}
