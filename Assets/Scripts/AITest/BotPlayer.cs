using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlayer : MonoBehaviour
{
    public const string type = "Bot";


    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    List<int> legalMoves = null;

    public IEnumerator MakeMove(Board board, Helper helper)
    {
        (int from, int to) = PickMove(board);

        selectedPiece = helper.GetGameObject(from);
        selectedPieceMoveTo = helper.GetGameObject(to);

        helper.MakeMove(selectedPiece, selectedPieceMoveTo);
        yield break;
    }


    private (int from, int to) PickMove(Board board)
    {
        return (22, 19);
    }

    public void Moo() =>
        Debug.Log("Moo");

}
