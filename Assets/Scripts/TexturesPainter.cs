using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshCollider))]
public class TexturesPainter : MonoBehaviour
{
    public string ControlMaskTextureFolder;
    public bool isPainting = false;
    public float BrushSize = 1f;
    public float BrushStrength = 1f;
    public LayerMask layer;

    [HideInInspector] public Texture[] TextureAlbedo;
    [HideInInspector] public Texture[] BrushTexture;
    [HideInInspector] public int SelectedBrush = 0;
    [HideInInspector] public int SelectedTexture = 0;

    public Texture2D ControlMaskTexture;
    public int BrushSizeInPrecent;
    public float DrawBrushSize;

}
