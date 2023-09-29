using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public int hp;
    //Allows for getting and setting hp 
    public int Hp
    {
        get { return hp; }

        set { hp = value; }
    }
}
