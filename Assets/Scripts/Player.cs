using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int playerNum;

    //public List<int> capturedPices
    
    public Player(int playerNum)
    {
        this.playerNum = playerNum;
    }

    public abstract int GetPlayerMove();
}
