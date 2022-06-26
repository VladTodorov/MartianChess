using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int num;
    
    public Player(int num)
    {
        this.num = num;
    }

    public abstract int GetPlayerMove();
}
