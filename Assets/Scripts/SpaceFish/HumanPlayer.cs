using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : MonoBehaviour
{
    public const string type = "Human";

    public int playerNumber;

    private GameObject selectedPiece;
    private GameObject selectedPieceMoveTo;
    List<int> legalMoves = null;

    public IEnumerator MakeMove(Board board, Helper helper)
    {

        while (true)
        {

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (selectedPiece != null)
                    helper.HighlightLegalMoves(selectedPiece, 0);

                GameObject touchInput = helper.GetTouch();
                //print(touchInput.name);

                if (touchInput == null || touchInput == selectedPiece)
                {
                    selectedPiece = null;
                }
                else if (selectedPiece == null && helper.IsValidPiece(touchInput))
                {
                    selectedPiece = touchInput;
                    legalMoves = helper.HighlightLegalMoves(selectedPiece, 1);
                }
                else if (selectedPiece != null)
                {
                    if (legalMoves.Contains(helper.VectorToOneD(touchInput.transform.position)))
                        selectedPieceMoveTo = touchInput;
                    else
                        selectedPiece = null;

                    legalMoves = null;
                }

            }

            if (selectedPiece != null && selectedPieceMoveTo != null)
            {
                helper.MakeMove(selectedPiece, selectedPieceMoveTo);

                selectedPiece = null;
                selectedPieceMoveTo = null;

                break;
            }
            yield return null;
        }

    }

}
