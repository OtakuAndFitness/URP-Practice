using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[ExecuteInEditMode]
public class Rotator : MonoBehaviour
{
    public float speed = 30f;

    [Serializable]
    public enum MyAxis
    {
        none,
        x,
        y,
        z
    }

    public MyAxis myAxis = MyAxis.y;
    private MyAxis _lastAxis = MyAxis.none;

    // private Vector3 _initialPos;
    // private quaternion _initialRot;

    private void Start()
    {
        ResetTransform();
        _lastAxis = myAxis;
    }

    private void OnEnable()
    {
        // _initialPos = transform.position;
        // _initialRot = transform.rotation;
        ResetTransform();
        _lastAxis = myAxis;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.update += EditorUpdate;
        }
#endif
        
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.update -= EditorUpdate;
        }
#endif
        
        
        ResetTransform();
        _lastAxis = MyAxis.none;

    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            HandleRotation();
        }
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (!Application.isPlaying)
        {
            HandleRotation();
            
            SceneView.RepaintAll();
        }
    }
#endif
    

    private void HandleRotation()
    {
        if (_lastAxis != myAxis)
        {
            ResetTransform();
            _lastAxis = myAxis;
        }
            
        switch (myAxis)
        {
            case MyAxis.x:
                transform.Rotate(Vector3.right, Time.deltaTime * speed);
                break;
            case MyAxis.y:
                transform.Rotate(Vector3.up, Time.deltaTime * speed);
                break;
            case MyAxis.z:
                transform.Rotate(Vector3.forward, Time.deltaTime * speed);
                break;

        }
    }

    private void ResetTransform()
    {
        // transform.position = _initialPos;
        // transform.rotation = _initialRot;
        transform.rotation = Quaternion.identity;
    }
}
