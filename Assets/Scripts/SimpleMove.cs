using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SimpleMove : MonoBehaviour
{
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        SRDebug.Init();
        speed *= Time.deltaTime;
        bool flag = SystemInfo.supportsComputeShaders;
        string gdn = SystemInfo.graphicsDeviceName;
        ProceduralGrassRenderer[] renderers = GetComponentsInChildren<ProceduralGrassRenderer>();
        Text dv = GameObject.Find("DriverVersion").GetComponent<Text>();
        // Text gv = GameObject.Find("GPUVersion").GetComponent<Text>();
        // gv.text = gdn;

        if (flag)
        {
            if (gdn.StartsWith("Mali"))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Renderer r = cube.GetComponent<Renderer>();
                // r.material = new Material(Shader.Find("Lit"));
                cube.transform.localPosition = Vector3.zero;
                cube.transform.localScale = Vector3.one * 100;
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].enabled = true;
                }
            }
            
            string str = GetInfo().ToString();
            dv.text = str;// + " - 111";
        }
        else
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
            string str = GetInfo().ToString();
            dv.text = str + " - 000";
        }
    }

    StringBuilder GetInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("设备信息");
        sb.AppendLine("操作系统名称: " + SystemInfo.operatingSystem);
        sb.AppendLine("处理器名称: " + SystemInfo.processorType);
        sb.AppendLine("处理器数量: " + SystemInfo.processorCount);
        sb.AppendLine("当前系统内存大小: " + SystemInfo.systemMemorySize + "MB");
        sb.AppendLine("当前显存大小: " + SystemInfo.graphicsMemorySize + "MB");
        sb.AppendLine("显卡名字: " + SystemInfo.graphicsDeviceName);
        sb.AppendLine("显卡厂商: " + SystemInfo.graphicsDeviceVendor);
        sb.AppendLine("显卡的标识符代码: " + SystemInfo.graphicsDeviceID);
        sb.AppendLine("显卡厂商的标识符代码: " + SystemInfo.graphicsDeviceVendorID);
        sb.AppendLine("该显卡所支持的图形API版本: " + SystemInfo.graphicsDeviceVersion);
        sb.AppendLine("图形设备着色器性能级别: " + SystemInfo.graphicsShaderLevel);
        sb.AppendLine("显卡的近似像素填充率: " + SystemInfo.graphicsPixelFillrate);
        sb.AppendLine("是否支持内置阴影: " + SystemInfo.supportsShadows);
        sb.AppendLine("是否支持渲染纹理: " + SystemInfo.supportsRenderTextures);
        sb.AppendLine("是否支持图像效果: " + SystemInfo.supportsImageEffects);
        sb.AppendLine("设备唯一标识符: " + SystemInfo.deviceUniqueIdentifier);
        sb.AppendLine("用户定义的设备的名称: " + SystemInfo.deviceName);
        sb.AppendLine("设备的模型: " + SystemInfo.deviceModel);
        return sb;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed);
        }
    }
}