using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using File = UnityEngine.Windows.File;

[CustomEditor(typeof(TexturesPainter))]
public class TexturesPainterEditor : Editor
{
   private readonly string ControlMaskProperty = "_Mask";

   private TexturesPainter _painter;
   private MeshRenderer _meshRenderer;
   private MeshFilter _meshFilter;
   private MeshCollider _meshCollider;

   private Color _brushColor;

   private Vector3 hitNormal;
   private Vector3 hitPosGizoms;

   private void OnEnable()
   {
      _painter = target as TexturesPainter;
      _meshRenderer = _painter.gameObject.GetComponent<MeshRenderer>();
      _meshCollider = _painter.gameObject.GetComponent<MeshCollider>();
      _meshFilter = _painter.gameObject.GetComponent<MeshFilter>();
      BrushInitialization();
      TextureInitialization();
   }

   private void TextureInitialization()
   {
      _painter.TextureAlbedo = new Texture[4];
      _painter.TextureAlbedo[0] = AssetPreview.GetAssetPreview(_meshRenderer.sharedMaterial.GetTexture("_Albedo1"));
      _painter.TextureAlbedo[1] = AssetPreview.GetAssetPreview(_meshRenderer.sharedMaterial.GetTexture("_Albedo2"));
      _painter.TextureAlbedo[2] = AssetPreview.GetAssetPreview(_meshRenderer.sharedMaterial.GetTexture("_Albedo3"));
      _painter.TextureAlbedo[3] = AssetPreview.GetAssetPreview(_meshRenderer.sharedMaterial.GetTexture("_Albedo4"));

   }

   private void BrushInitialization()
   {
      string brushFolder = "Assets/Resources/Textures/Brushes/";
      ArrayList brushList = new ArrayList();
      Texture brushTexture;
      int brushIndex = 0;
      do
      {
         brushTexture = (Texture)AssetDatabase.LoadAssetAtPath(brushFolder + "Brush" + brushIndex + ".png", typeof(Texture));
         if (brushTexture)
         {
            brushList.Add(brushTexture);
         }
         brushIndex++;
      } while (brushTexture);

      _painter.BrushTexture = brushList.ToArray(typeof(Texture)) as Texture[];
   }

   public override void OnInspectorGUI()
   {
      if (Application.isPlaying)
      {
         EditorGUILayout.LabelField("Please edit without running");
         return;
      }
      _painter.ControlMaskTexture = _meshRenderer.sharedMaterial.GetTexture(ControlMaskProperty) as Texture2D;
      if (_painter.ControlMaskTexture == null)
      {
         _painter.ControlMaskTextureFolder =
            EditorGUILayout.TextField("Control Mask Folder", _painter.ControlMaskTextureFolder);
         if (GUILayout.Button("Start Painting"))
         {
            CreateMaskTexture();
         }
      }
      else
      {
         if (_meshCollider)
         {
            if (_meshCollider.convex)
            {
               EditorGUILayout.HelpBox(new GUIContent("Do not toggle convex in mesh collider"));
               return;
            }
         }

         _painter.isPainting = GUILayout.Toggle(_painter.isPainting, "isPainting");
         if (_painter.isPainting)
         {
            EditorGUILayout.LabelField("Brush Parameters: ");
            _painter.BrushSize = EditorGUILayout.Slider("Brush Size", _painter.BrushSize, 1, 16);
            _painter.BrushStrength = EditorGUILayout.Slider("Brush Strength", _painter.BrushStrength, 0, 1f);
            LayerMask layerMask = EditorGUILayout.MaskField("Layer: ",
               InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_painter.layer), InternalEditorUtility.layers);
            _painter.layer = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask);
            _painter.SelectedTexture = GUILayout.SelectionGrid(_painter.SelectedTexture, _painter.TextureAlbedo, 4,
               "gridlist", GUILayout.Width(340), GUILayout.Height(86));
            _painter.SelectedBrush = GUILayout.SelectionGrid(_painter.SelectedBrush, _painter.BrushTexture, 9,
               "gridlist", GUILayout.Width(340), GUILayout.Height(70));
            _painter.BrushSizeInPrecent = Mathf.Max(1,
               Mathf.RoundToInt(_painter.BrushSize * _painter.ControlMaskTexture.width / 100));
            _painter.DrawBrushSize = _painter.BrushSize * _painter.transform.localScale.x *
                                     (_meshFilter.sharedMesh.bounds.size.x / 200);

         }
      }
   }

   private void CreateMaskTexture()
   {
      Texture2D maskTex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
      Color[] colorBase = new Color[1024 * 1024];
      for (int i = 0; i < colorBase.Length; i++)
      {
         colorBase[i] = new Color(1, 0, 0, 0);
      }
      
      maskTex.SetPixels(colorBase);

      string fileName = Selection.activeTransform.name;

      string path = Path.Combine(_painter.ControlMaskTextureFolder, fileName + ".png");
      
      path = AssetDatabase.GenerateUniqueAssetPath(path);
      
      byte[] bytes = maskTex.EncodeToPNG();
      File.WriteAllBytes(path, bytes);
      
      AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

      TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
      textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
      textureImporter.isReadable = true;
      textureImporter.mipmapEnabled = false;
      textureImporter.wrapMode = TextureWrapMode.Clamp;
      SaveTextureFormat(textureImporter);
       
      Texture2D ControlTex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
      _meshRenderer.sharedMaterial.SetTexture(ControlMaskProperty, ControlTex);

   }

   private void SaveTextureFormat(TextureImporter textureImporter)
   {
      //"Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PS4", "XboxOne", "Nintendo Switch" and "tvOS"
      TextureImporterPlatformSettings platformSettings = textureImporter.GetPlatformTextureSettings("Standalone");
      TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings()
      {
         format = TextureImporterFormat.RGBA32,
         name = platformSettings.name,
         overridden = true
      };
      textureImporter.SetPlatformTextureSettings(settings);
      textureImporter.SaveAndReimport();
      AssetDatabase.Refresh();

   }

   private void OnSceneGUI()
   {
      _brushColor.a = 1f;
      Handles.color = _brushColor;
      Handles.DrawWireDisc(hitPosGizoms, hitNormal, _painter.DrawBrushSize);
      
      SceneView.RepaintAll();
      OnSceneGUIFunc(SceneView.currentDrawingSceneView);
   }

   private void OnSceneGUIFunc(SceneView currentDrawingSceneView)
   {
      if (_painter.isPainting && Selection.Contains(_painter.gameObject))
      {
         HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
         Event e = Event.current;
         Vector2 mousePos = e.mousePosition;
         mousePos.x *= EditorGUIUtility.pixelsPerPoint;
         mousePos.y = currentDrawingSceneView.camera.pixelHeight - mousePos.y * EditorGUIUtility.pixelsPerPoint;
         Ray ray = currentDrawingSceneView.camera.ScreenPointToRay(mousePos);
         int layer = _painter.layer.value;
         if (Physics.Raycast(ray, out RaycastHit hitGizmo, Mathf.Infinity, layer))
         {
            hitPosGizoms = hitGizmo.point;
            hitNormal = hitGizmo.normal;
            if (e.type == EventType.MouseDrag && e.button == 0)
            {
               PaintTextureColor(hitGizmo);
               e.Use();
            }else if (e.type == EventType.MouseUp)
            {
               SaveControlMask();
            }
         }
      }
   }

   private void SaveControlMask()
   {
      if (_painter.ControlMaskTexture)
      {
         string path = AssetDatabase.GetAssetPath(_painter.ControlMaskTexture);
         byte[] bytes = _painter.ControlMaskTexture.EncodeToPNG();
         File.WriteAllBytes(path, bytes);
      }
   }

   private void PaintTextureColor(RaycastHit hitGizmo)
   {
      Color color = new Color(1, 0, 0, 0);
      switch (_painter.SelectedTexture)
      {
         case 0:
            color = new Color(1, 0, 0, 0);
            break;
         case 1:
            color = new Color(0, 1, 0, 0);
            break;
         case 2:
            color = new Color(0, 0, 1, 0);
            break;
         case 3:
            color = new Color(0, 0, 0, 1);
            break;
      }

      int brushSizeInPrecent = _painter.BrushSizeInPrecent;
      Texture2D mask = _painter.ControlMaskTexture;
      Vector2 pixelUV = hitGizmo.textureCoord;
      int PuX = Mathf.FloorToInt(pixelUV.x * mask.width);
      int PuY = Mathf.FloorToInt(pixelUV.y * mask.height);
      int x = Mathf.Clamp(PuX - brushSizeInPrecent / 2, 0, mask.width - 1);
      int y = Mathf.Clamp(PuY - brushSizeInPrecent / 2, 0, mask.height - 1);
      int width = Mathf.Clamp(PuX + brushSizeInPrecent / 2, 0, mask.width) - x;
      int height = Mathf.Clamp(PuY + brushSizeInPrecent / 2, 0, mask.height) - y;

      Color[] terrainBay = mask.GetPixels(x, y, width, height,0);
      
      Texture2D brush = _painter.BrushTexture[_painter.SelectedBrush] as Texture2D;
      float[] brushAlpha = new float[brushSizeInPrecent * brushSizeInPrecent];

      for (int i = 0; i < brushSizeInPrecent; i++)
      {
         for (int j = 0; j < brushSizeInPrecent; j++)
         {
            brushAlpha[j * brushSizeInPrecent + i] =
               brush.GetPixelBilinear((float)i / brushSizeInPrecent, (float)j / brushSizeInPrecent).a;
         }
      }

      for (int i = 0; i < height; i++)
      {
         for (int j = 0; j < width; j++)
         {
            int index = i * width + j;
            float strength =
               brushAlpha[
                  Mathf.Clamp(y + i - (PuY - brushSizeInPrecent / 2), 0, brushSizeInPrecent - 1) * brushSizeInPrecent +
                  Mathf.Clamp(x + j - (PuX - brushSizeInPrecent / 2), 0, brushSizeInPrecent - 1)] * _painter.BrushStrength;
            terrainBay[index] = Color.Lerp(terrainBay[index], color, strength);
         }
      }
      
      Undo.RegisterCompleteObjectUndo(mask, "meshPaint");
      mask.SetPixels(x,y,width,height,terrainBay,0);
      mask.Apply();
   }
}
