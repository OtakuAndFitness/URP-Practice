using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRotate : MonoBehaviour
{
    public GameObject go;

    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (go != null)
        {
            transform.RotateAround(go.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            
            transform.LookAt(go.transform.position + new Vector3(0,4,0));
        }
    }
}
