using System.Collections;
using System.Collections.Generic;


public class HumanPlayer : Player
{
    public const string type = "Human";

    public HumanPlayer(int playerNum) : base(playerNum) { }

    public override int GetPlayerMove()
    {
        throw new System.NotImplementedException();
    }
}
