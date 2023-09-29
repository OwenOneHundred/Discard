using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool atObjective;

    public Transform objective;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        //Moves Enemy Object towards objective if no at it already
        if(atObjective != true)
        {
            transform.position = Vector2.MoveTowards(transform.position, objective.position, speed * Time.deltaTime);
        }
    }

    //Gets Distance Bewteen Player and gun
    /*public float DetermineDistance(Transform target)
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(target, currentPos);

        return dist;
    }*/
}
