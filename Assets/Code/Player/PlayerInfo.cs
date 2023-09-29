using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private int hp;
    //Allows for getting and setting speed 
    public int Hp
    {
        get { return hp; }

        set { hp = value; }
    }

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
