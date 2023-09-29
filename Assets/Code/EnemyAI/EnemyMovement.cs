using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool atObjective;

    public Transform objective;
    public Vector3 objectiveV3;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        //Moves Enemy Object towards objective if no at it already
        if(atObjective != true)
        {
            if(objective != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, objective.position, speed * Time.deltaTime);
            }
            else if(objectiveV3 != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, objectiveV3, speed * Time.deltaTime);
            }
        }
    }
}
