using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CalculateFrustum : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
        float tanHalfFOV = Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad);
        float halfHeight = tanHalfFOV * cam.nearClipPlane;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 toTop = cam.transform.up * halfHeight;
        Vector3 toRight = cam.transform.right * halfWidth;
        Vector3 forward = cam.transform.forward * cam.nearClipPlane;
        Vector3 toTopLeft = forward + toTop - toRight;
        Vector3 toBottomLeft = forward - toTop - toRight;
        Vector3 toTopRight = forward + toTop + toRight;
        Vector3 toBottomRight = forward - toTop + toRight;
            
        toTopLeft /= cam.nearClipPlane;
        toBottomLeft /= cam.nearClipPlane;
        toTopRight /= cam.nearClipPlane;
        toBottomRight /= cam.nearClipPlane;
            
        Matrix4x4 frustumDir = Matrix4x4.identity;
        frustumDir.SetRow(0, toBottomLeft);
        frustumDir.SetRow(1, toBottomRight);
        frustumDir.SetRow(2, toTopLeft);
        frustumDir.SetRow(3, toTopRight);
        
        Shader.SetGlobalMatrix("_FrustumDir", frustumDir);
        Shader.SetGlobalVector("_CameraForward", cam.transform.forward);
    }
}
