//#define ENABLE_TEST_SROPTIONS

using System.ComponentModel;
using SRDebugger;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class SROptions
{
	private bool _enableDebug = false;

	private bool _enableBattleDebug = false;

	private int _testLargeIncrement = 0;

	private float _test01Range = 0;

//	[Category("Test"), DisplayName("Test Action Renamed")]
//	public void OpenLogText()
//	{
//		Debug.Log ("Test Button Action");
//	}
//
//    [Category("Debug Switch")]
//    public bool EnableDebug 
//    {
//		get { return _enableDebug; }
//        set
//        {
//			_enableDebug = value;
//        }
//    }
//
//	[Category("Debug Switch")]
//    public bool EnableDebugError
//    {
//		get { return _enableBattleDebug; }
//        set
//        {
//			_enableBattleDebug = value;
//        }
//    }
//    
//	[Category("Test Number")]
//	[NumberRange(0, 100)]
//	public float Test01Range
//	{
//		get { return _test01Range; }
//		set
//		{
//			//OnValueChanged("Test01Range", value);
//			_test01Range = value;
//		}
//	}
//
//	[Category("Test Number")]
//	[Increment(25)]
//	public int TestLargeIncrement
//	{
//		get { return _testLargeIncrement; }
//		set
//		{
//			//OnValueChanged("TestLargeIncrement", value);
//			_testLargeIncrement = value;
//		}
//	}

    //[Category("Debug")]
    //public void DisableGuide()
    //{
    //    var go = GameObject.Find("WndGuide");
    //    if(go != null) go.SetActive(false);
    //}

    //[Category("Debug")]
    //[NumberRange(0, 2)]
    //public int AvatarDamage
    //{
    //    get { return AccountAvatar.DebugDamage; }
    //    set { AccountAvatar.DebugDamage = value; }
    //}
    
    
    //[Category("Postprocess")]
    //public bool BloomEnabled
    //{
    //    get { return (MainCamera.bloom != null) && MainCamera.bloom.enabled; }
    //    set { if(MainCamera.bloom != null)MainCamera.bloom.enabled = value; }
    //}

    ////[Category("Postprocess")]
    ////[NumberRange(0, 1)]
    ////[Increment(0.05)]
    ////public float BloomThreshold
    ////{
    ////    get { return MainCamera.bloom.threshhold; }
    ////    set
    ////    {
    ////        var bloom = MainCamera.bloom;
    ////        if(bloom.threshhold != value)
    ////        {
    ////            bloom.threshhold = value;
    ////        }
    ////    }
    ////}

    ////[Category("Postprocess")]
    ////[NumberRange(0, 3)]
    ////[Increment(0.1)]
    ////public float BloomIntensity
    ////{
    ////    get { return MainCamera.bloom.intensity; }
    ////    set
    ////    {
    ////        var bloom = MainCamera.bloom;
    ////        if(bloom.intensity != value)
    ////        {
    ////            bloom.intensity = value;
    ////        }
    ////    }
    ////}

    ////[Category("Postprocess")]
    ////[NumberRange(0, 3)]
    ////[Increment(0.1)]
    ////public float BloomSoftness
    ////{
    ////    get { return MainCamera.bloom.blurSize; }
    ////    set
    ////    {
    ////        var bloom = MainCamera.bloom;
    ////        if(bloom.blurSize != value)
    ////        {
    ////            bloom.blurSize = value;
    ////        }
    ////    }
    ////}
    
    //[Category("Postprocess")]
    //public bool BeautifyEnabled
    //{
    //    get { return (MainCamera.beautify != null) && MainCamera.beautify.enabled; }
    //    set { if(MainCamera.beautify != null)MainCamera.beautify.enabled = value; }
    //}
    
    ////[Category("Gameplay")]
    ////public bool AnimationSpeedManager
    ////{
    ////    get { return AnimationNode.EnableAnimationCurve; }
    ////    set { AnimationNode.EnableAnimationCurve = value; }
    ////}

    //public void DestroyBehaviours()
    //{
    //    _DestroyBehaviours("_GameApp");
    //    _DestroyBehaviours("UI Root (GameApp)");
    //    _DestroyBehaviours("Download Manager");
    //    _DestroyBehaviours("Loader");
    //}
    
    //public void DisableUI()
    //{
    //    ViewManager.uiRoot.gameObject.SetActive(false);
    //}

    //private void _DestroyBehaviours(string name)
    //{
    //    var list = new System.Collections.Generic.List<MonoBehaviour>(GameObject.Find(name).GetComponentsInChildren<MonoBehaviour>());
    //    list.RemoveAll(b =>{
    //        return (b is UIRect);
    //    });
    //    list.Sort((a, b)=>{
    //        var _a = 0;
    //        var _b = 0;
    //        if(a is UISpriteAnimation) _a += 100;
    //        if(b is UISpriteAnimation) _b += 100;
    //        if(a is  E3DStereoCam) _a += 100;
    //        if(b is  E3DStereoCam) _a += 100;
    //        return _a - _b;
    //    });
    //    foreach(var b in list)
    //    {
    //        GameObject.Destroy(b);
    //    }
    //}
    
    //public void SetEmptyTarget()
    //{
    //    var renderTexture = new RenderTexture(1, 1, 0);
    //    MainCamera.realCam.targetTexture = renderTexture;
    //}

    //public void ReplaceShader()
    //{
    //    string[] RootNodes = new[] { "_GameApp", "UI Root (GameApp)", "Download Manager", "Loader" };
    
    //    var shader = Shader.Find("Custom/NewShader(Dummy)");

    //    foreach (var rootNodeName in RootNodes)
    //    {
    //        foreach (var r in GameObject.Find(rootNodeName).GetComponentsInChildren<Renderer>())
    //        {
    //            foreach (var m in r.sharedMaterials)
    //            {
    //                if(m == null) continue;
    //                m.shader = shader;
    //            }
    //        }
    //    }
    //}

    //public void LoadEmptyScene()
    //{
    //    Application.LoadLevel("TestConfigFiles");
    //}

    //public bool EnableMainCamera
    //{
    //    get { return MainCamera.realCam.gameObject.activeSelf; }
    //    set { MainCamera.realCam.gameObject.SetActive(value); }
    //}

    //public bool EnableUICamera
    //{
    //    get { return ViewManager.uiCamera.camera.enabled; }
    //    set { ViewManager.uiCamera.camera.enabled = value; }
    //}

    //private List<GameObject> npcList = new List<GameObject>();

    //public bool ActiveNPC
    //{
    //    get { return npcList.Count == 0; }
    //    set
    //    {
    //        if(value)
    //        {
    //            foreach (var avatar in npcList)
    //            {
    //                avatar.SetActive(true);
    //            }
    //            npcList.Clear();
    //        }
    //        else
    //        {
    //            npcList.Clear();
    //            var parent = SceneRoot.Go.transform.parent;
    //            for (int i = 0; i < parent.childCount; ++i)
    //            {
    //                var t = parent.GetChild(i);
    //                if(t.GetComponent<Avatar>() != null)
    //                {
    //                    var go = t.gameObject;
    //                    go.SetActive(false);
    //                    npcList.Add(go);
    //                }
    //            }
    //        }
    //    }
    //}
    //private bool _killer;
    //private bool _killer1;
    //public bool Killer
    //{
    //    get
    //    {
    //        return _killer;
    //    }
    //    set
    //    {
    //        _killer = value;
    //        AccountAvatar.DebugDamage = value ? 2 : 0;
    //    }
    //}
    //public bool Killer1
    //{
    //    get
    //    {
    //        return _killer1;
    //    }
    //    set
    //    {
    //        _killer1 = value;
    //        AccountAvatar.DebugDamage = value ? 1 : 0;
    //    }
    //}
    //private bool _showFps;
    //public bool SHOW_FPS
    //{
    //    get
    //    {
    //        return _showFps;
    //    }
    //    set
    //    {
    //        ShowFPS.show = value;
    //    }
    //}
    //private bool _showGM;
    //public bool SHOW_GM
    //{
    //    get
    //    {
    //        return _showGM;
    //    }
    //    set
    //    {
    //        _showGM = value;
    //        if (_showGM)
    //            ViewManager.LoadView(ViewDict.GM);
    //    }
    //}
    //private GameObject geomGO;
    //public bool ActiveSceneGeometry
    //{
    //    get
    //    {
    //        if(geomGO == null)
    //        {
    //           geomGO = E3D_Utils.getChildByName(SceneRoot.Go.transform, "area_01").gameObject;
    //        }
    //        return geomGO.activeSelf;
    //    }
    //    set
    //    {
    //        geomGO.SetActive(value);
    //    }
    //}

    //public bool ActiveMainRole
    //{
    //    get { return World.ins.mainRole.gameObject.activeSelf; }
    //    set { World.ins.mainRole.gameObject.SetActive(value); }
    //}

    //private bool activeWaterEffectInit = true;
    //private GameObject waterEffectGO;
    //public bool ActiveWaterEffect
    //{
    //    get
    //    {
    //        if(activeWaterEffectInit)
    //        {
    //            waterEffectGO = GameObject.Find("Effect_water");
    //            activeWaterEffectInit = true;
    //        }
    //        return waterEffectGO != null && waterEffectGO.activeSelf;
    //    }
    //    set
    //    {
    //        if(waterEffectGO != null) waterEffectGO.SetActive(value);
    //    }
    //}

    //public void SpawnMonster()
    //{
    //    var scene = World.ins.pveScene;
    //    if(scene == null) return;
    //    var nodeInfo = scene.fightNodeList[0].bornAreaList[0].bornPointList[0].nodeInfo;
    //    var monsterInfo = new MonsterVo(nodeInfo.mid);
    //    MonsterAsset monsterAsset = SceneAssetsManager.getMonsterAsset(monsterInfo.modelInfo.Model);
    //    var objInPool = monsterAsset.pool.objectQueue.Peek();

    //    var go = GameObject.Instantiate(objInPool) as GameObject;

    //    go.SetActive(true);
    //    go.transform.parent = scene.transform.parent;
    //    var pos = scene.world.mainRole.transform.position;
    //    float a = 1.5f;
    //    pos.x += Random.Range(-a, a);
    //    pos.z += Random.Range(-a, a);
    //    go.transform.position = pos;

    //    Transform mesh_body = E3D_Utils.getChildByName(go.transform, "mesh_body");
    //    if (!mesh_body)
    //        return;
    //    SkinnedMeshRenderer skinMesh = E3D_Utils.GetOrAddComponent<SkinnedMeshRenderer>(mesh_body.gameObject);
    //    MaterialsAsset materialsAsset = SceneAssetsManager.getMaterialAsset(monsterInfo.modelInfo.Material);
    //    if (skinMesh && materialsAsset != null)
    //    {
    //        if (materialsAsset.material != null)
    //        {
    //            skinMesh.material = materialsAsset.material;
    //        }
    //    }
    //    go.GetComponent<Animation>().Play("idle");
    //}

    //public void SpawnMainRole()
    //{
    //    var mainRole = World.ins.mainRole;
    //    mainRole.gameObject.SetActive(false);

    //    var go = GameObject.Instantiate(mainRole.gameObject) as GameObject;

    //    var comp1 = go.GetComponent<MainRole>();
    //    comp1.enabled = false;
    //    Object.Destroy(comp1);

    //    var comp2 = go.GetComponent<NewInputController>();
    //    comp2.enabled = false;
    //    Object.Destroy(comp2);

    //    var comp3 = go.GetComponent<RoleModelEffect>();
    //    comp3.enabled = false;
    //    Object.Destroy(comp3);

    //    go.SetActive(true);
    //    go.transform.parent = World.ins.currScene.transform.parent;
    //    var pos = mainRole.transform.position;
    //    float a = 1.5f;
    //    pos.x += Random.Range(-a, a);
    //    pos.z += Random.Range(-a, a);
    //    go.transform.position = pos;

    //    mainRole.gameObject.SetActive(true);

    //    go.GetComponent<Animation>().Play("idle");
    //}
}
