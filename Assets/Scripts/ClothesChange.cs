using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClothesChange : MonoBehaviour
{
    [Serializable]
    public class Clothes
    {
        public Sprite thumb;

        public Mesh headMesh;
        public Material headMat;
        
        public Mesh bodyMesh;
        public Material bodyMat;
        
        public Mesh topMesh;
        public Material topMat;
        
        public Mesh bottomMesh;
        public Material bottomMat;
        
        public Mesh footwearMesh;
        public Material footwearMat;
    }
    
    [Serializable]
    public class Hair
    {
        public Sprite thumb;
        
        public Mesh hairMesh;
        public Material hairMat;
        
    }
    
    [Serializable]
    public class Glasses
    {
        public Sprite thumb;
        
        public Mesh glassesMesh;
        public Material glassesMat;
    }

    [Serializable]
    public class Beard
    {
        public Sprite thumb;
        
        public enum BeardType
        {
            Mesh,
            Texture
        }

        public BeardType type = BeardType.Mesh;
        
        public Mesh beardMesh;
        public Material beardMat;
    }
    
    public List<Clothes> ClothesList;
    public List<Hair> HairList;
    public List<Glasses> GlassesList;
    public List<Beard> BeardList;


    private void Start()
    {
        
    }

    
}
