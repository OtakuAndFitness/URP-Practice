using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GenerateLights : MonoBehaviour
{
    public GameObject pl;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject pointLight = Instantiate(pl, new Vector3(Random.Range(-40, 40), 1.216f, Random.Range(-40, 45)), Quaternion.identity);
            Light lightComponent = pointLight.GetComponent<Light>();
            Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            lightComponent.color = color;
            lightComponent.intensity = 30;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
