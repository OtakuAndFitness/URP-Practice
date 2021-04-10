using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAverage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh tempMesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
        tempMesh = NormalsAverage(tempMesh);
        GetComponent<MeshFilter>().sharedMesh = tempMesh;
    }

    Mesh NormalsAverage(Mesh mesh)
    {
        Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (!map.ContainsKey(mesh.vertices[i]))
            {
                map.Add(mesh.vertices[i], new List<int>());
            }
            map[mesh.vertices[i]].Add(i);
        }

        Vector3[] normals = mesh.normals;
        Vector3 normal;
        foreach (var item in map)
        {
            normal = Vector3.zero;
            foreach (var index in item.Value)
            {
                normal += mesh.normals[index];
            }
            normal /= item.Value.Count;
            foreach (var index in item.Value)
            {
                normals[index] = normal;
            }
        }

        mesh.normals = normals;
        return mesh;
    }
}
