using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PostProcessingExtends.Effects;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject hairItemPrefab;
    public GameObject glassesItemPrefab;
    public GameObject beardItemPrefab;
    public GameObject clothesItemPrefab;

    public SkinnedMeshRenderer hair, glasses, beard, head, body, top, bottom, footwear;

    private Material currentHeadMat;
    private Material cachedHeadMat;
    
    private void Start()
    {
        for (int i = 0; i < HairList.Count; i++)
        {
            var instance = Instantiate(hairItemPrefab);
            instance.transform.SetParent(hairItemPrefab.transform.parent,false);
            instance.transform.SetSiblingIndex(hairItemPrefab.transform.parent.childCount - 2);
            instance.GetComponent<Image>().sprite = HairList[i].thumb;
            instance.SetActive(true);
            Toggle toggle = instance.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                InitHair(i);
            }
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    int index = instance.transform.GetSiblingIndex();
                    
                    InitHair(index);

                }
            });
        }
        
        for (int i = 0; i < ClothesList.Count; i++)
        {
            var instance = Instantiate(clothesItemPrefab);
            instance.transform.SetParent(clothesItemPrefab.transform.parent,false);
            instance.transform.SetSiblingIndex(clothesItemPrefab.transform.parent.childCount - 2);
            instance.GetComponent<Image>().sprite = ClothesList[i].thumb;
            instance.SetActive(true);
            Toggle toggle = instance.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                InitClothes(i);
            }
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    int index = instance.transform.GetSiblingIndex();

                   InitClothes(index);
                }
            });
        }
        
        for (int i = 0; i < GlassesList.Count; i++)
        {
            var instance = Instantiate(glassesItemPrefab);
            instance.transform.SetParent(glassesItemPrefab.transform.parent,false);
            instance.transform.SetSiblingIndex(glassesItemPrefab.transform.parent.childCount - 2);
            instance.GetComponent<Image>().sprite = GlassesList[i].thumb;
            instance.SetActive(true);
            Toggle toggle = instance.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                InitGlasses(i);
            }
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    int index = instance.transform.GetSiblingIndex();

                    InitGlasses(index);
                }
            });
        }
        
        for (int i = 0; i < BeardList.Count; i++)
        {
            var instance = Instantiate(beardItemPrefab);
            instance.transform.SetParent(beardItemPrefab.transform.parent,false);
            instance.transform.SetSiblingIndex(beardItemPrefab.transform.parent.childCount - 2);
            instance.GetComponent<Image>().sprite = BeardList[i].thumb;
            instance.SetActive(true);
            Toggle toggle = instance.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                InitBeard(i);
            }
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    int index = instance.transform.GetSiblingIndex();

                    InitBeard(index);
                    
                }
            });
        }
    }

    void InitHair(int i)
    {
        hair.sharedMesh = HairList[i].hairMesh;
        hair.sharedMaterial = HairList[i].hairMat;
    }

    void InitClothes(int i)
    {
        head.sharedMesh = ClothesList[i].headMesh;
        head.sharedMaterial = currentHeadMat != null ? currentHeadMat : ClothesList[i].headMat;
        cachedHeadMat = ClothesList[i].headMat;
                    
        body.sharedMesh = ClothesList[i].bodyMesh;
        body.sharedMaterial = ClothesList[i].bodyMat;

        top.sharedMesh = ClothesList[i].topMesh;
        top.sharedMaterial = ClothesList[i].topMat;
                    
        bottom.sharedMesh = ClothesList[i].bottomMesh;
        bottom.sharedMaterial = ClothesList[i].bottomMat;
                    
        footwear.sharedMesh = ClothesList[i].footwearMesh;
        footwear.sharedMaterial = ClothesList[i].footwearMat;
    }

    void InitGlasses(int i)
    {
        glasses.sharedMesh = GlassesList[i].glassesMesh;
        glasses.sharedMaterial = GlassesList[i].glassesMat;
    }

    void InitBeard(int i)
    {
        switch (BeardList[i].type)
        {
            case Beard.BeardType.Mesh:
                beard.sharedMesh = BeardList[i].beardMesh;
                beard.sharedMaterial = BeardList[i].beardMat;
                currentHeadMat = null;
                if (cachedHeadMat != null)
                {
                    head.sharedMaterial = cachedHeadMat;
                }
                break;
            case Beard.BeardType.Texture:
                head.sharedMaterial = BeardList[i].beardMat;
                currentHeadMat = BeardList[i].beardMat;
                beard.sharedMesh = null;
                beard.sharedMaterial = null;
                break;
        }
    }
}
