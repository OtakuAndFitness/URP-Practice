using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopTranslate : MonoBehaviour
{
    public bool moveToLeft = true;

    public float speed = 2;

    private void Update()

    {

        Move();

    }

    private void Move()

    {

        if (transform.position.x <= -8 && moveToLeft)

        {

            moveToLeft = false;

        }

        else if (transform.position.x >= 5 && !moveToLeft)

            moveToLeft = true;

        transform.position += (moveToLeft ? Vector3.left : Vector3.right) * Time.deltaTime * speed;

    }
}
