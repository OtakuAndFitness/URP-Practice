using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IceController : MonoBehaviour
{
    public GameObject dragon;
    private Animator _animator;
    private Material _material;
    private int iceSlider = Shader.PropertyToID("_IceSlider");
    // Start is called before the first frame update
    void Start()
    {
        _material = dragon.transform.Find("DragonMesh").GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        _material.SetFloat(iceSlider,0);
        _animator = dragon.GetComponent<Animator>();
        
        PlayEffect();
    }

    void PlayEffect()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(4f);
        sequence.Append(_material.DOFloat(1, iceSlider, 1));
        sequence.AppendCallback(PauseAnimation);
        sequence.AppendInterval(2f);
        sequence.Append(_material.DOFloat(0, iceSlider, 1));
        sequence.AppendCallback(RestartAnimation);
        sequence.SetLoops(-1);
        sequence.Play();
    }

    private void RestartAnimation()
    {
        _animator.speed = 1;
    }

    void PauseAnimation()
    {
        _animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
