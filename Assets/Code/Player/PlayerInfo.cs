using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int hp;

    //Damages Player
    public void Damage()
    {
        hp--;

        if(hp <= 0)
        {
            Debug.Log("Player Death");
        }
    }
}
