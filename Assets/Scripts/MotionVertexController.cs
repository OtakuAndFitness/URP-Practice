using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MotionVertexController : MonoBehaviour
{
    private Transform _transform;

    private Material[] _materials;

    private Vector3 lastPostion;

    private Vector3 newPosition;

    private Vector3 direction;

    private float t = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        lastPostion = newPosition = _transform.position;

        Renderer[] renderers = _transform.GetComponentsInChildren<Renderer>();
        _materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            _materials[i] = renderers[i].sharedMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        newPosition = _transform.position;

        if (newPosition == lastPostion) t = 0;
        t += Time.deltaTime;
        lastPostion = Vector3.Lerp(lastPostion, newPosition, t / 2);
        direction = lastPostion - newPosition;
        foreach (var mat in _materials)
        {
            mat.SetVector("_Direction", new Vector4(direction.x,direction.y, direction.z, mat.GetVector("_Direction").w));
        }
    }
}
