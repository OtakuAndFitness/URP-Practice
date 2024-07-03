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

    private void Start()
    {
        for (int i = 0; i < HairList.Count; i++)
        {
            var instance = Instantiate(hairItemPrefab);
            instance.transform.SetParent(hairItemPrefab.transform.parent,false);
            instance.transform.SetSiblingIndex(hairItemPrefab.transform.parent.childCount - 2);
            instance.GetComponent<Image>().sprite = HairList[i].thumb;
            instance.SetActive(true);
            instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    int index = instance.transform.GetSiblingIndex();
                    hair.sharedMesh = HairList[index].hairMesh;
                    hair.sharedMaterial = HairList[index].hairMat;

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
            instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    Debug.LogError(instance.transform.GetSiblingIndex());
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
            instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    Debug.LogError(instance.transform.GetSiblingIndex());
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
            instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    Debug.LogError(instance.transform.GetSiblingIndex());
                }
            });
        }
    }
}
