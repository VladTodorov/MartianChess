using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlayer : MonoBehaviour
{
    public const string type = "Human";


    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    List<int> legalMoves = null;

    public IEnumerator MakeMove(Board board, Helper helper)
    {
        //int pos
        throw new NotImplementedException();
    }

}
