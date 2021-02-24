using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        speed *= Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.right * speed);
        }
    }
}
