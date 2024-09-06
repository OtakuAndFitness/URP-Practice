using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public GameObject go;

    public int rows = 16;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateGameObjects());
    }

    IEnumerator CreateGameObjects()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject player = Instantiate(go);
                // player.transform.parent = transform;
                player.transform.localPosition = new Vector3(i, go.transform.localPosition.y, j);
                player.SetActive(true);
                
                yield return new WaitForEndOfFrame();

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
