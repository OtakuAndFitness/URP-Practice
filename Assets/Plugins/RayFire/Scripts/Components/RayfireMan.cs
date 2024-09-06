using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayFire
{
    [DisallowMultipleComponent]
    [AddComponentMenu ("RayFire/Rayfire Man")]
    [HelpURL ("https://rayfirestudios.com/unity-online-help/components/unity-man-component/")]
    public class RayfireMan : MonoBehaviour
    {
        // UI
        public bool                       setGravity;
        public float                      multiplier       = 1f;
        public RigidbodyInterpolation     interpolation    = RigidbodyInterpolation.None;
        public float                      colliderSize     = 0.05f;
        public int                        coplanarVerts    = 30;
        public MeshColliderCookingOptions cookingOptions   = (MeshColliderCookingOptions)30;
        public CollisionDetectionMode     meshCollision    = CollisionDetectionMode.ContinuousDynamic;
        public CollisionDetectionMode     clusterCollision = CollisionDetectionMode.Discrete;
        public float                      minimumMass      = 0.1f;
        public float                      maximumMass      = 400f;
        public RFMaterialPresets          materialPresets  = new RFMaterialPresets();
        public GameObject                 parent;
        public float                      globalSolidity               = 1f;
        public float                      timeQuota                    = 0.03f;
        public QuotaType                  quotaAction                  = QuotaType.Skip;
        public RFManDemolition            advancedDemolitionProperties = new RFManDemolition();
        public RFPoolingFragment          fragments                    = new RFPoolingFragment();
        public RFPoolingParticles         particles                    = new RFPoolingParticles();
        public bool                       debug                        = true;
        public bool                       debugBuild;
        public bool                       debugEditor;
        
        // Coroutines
        public List<RFPhysic> physicList = new List<RFPhysic>();
        public List<RayfireRigid> dmlList = new List<RayfireRigid>();
        
        // Non Serialized
        [NonSerialized] public RFStorage storage;
        [NonSerialized] public bool      physicsDataCorState;
        [NonSerialized] public bool      dmlCorState;
        [NonSerialized] public float     dmlThisFrame;
        [NonSerialized] public Transform transForm;


        // Static
        public static RayfireMan                 inst;
        public const  int                        buildMajor           = 1;
        public static int                        buildMinor           = 70;
        public static float                      colliderSizeStatic   = 0.05f;
        public static int                        coplanarVertLimit    = 30;
        public static MeshColliderCookingOptions cookingOptionsStatic = (MeshColliderCookingOptions)30;
        public static bool                       debugStatic          = true;
        public static bool                       debugBuildStatic;
        public static bool                       debugEditorStatic;
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        // Awake
        void Awake()
        {
            // Set static instance
            SetInstance();
        }

        /*
        // Late Update
        void LateUpdate()
        {
            dmlThisFrame = 0f;
        }
        */
        
        /// /////////////////////////////////////////////////////////
        /// Instance
        /// /////////////////////////////////////////////////////////

        // Set instance
        void SetInstance()
        {
            // Inst not define, set to this
            if (inst == null)
            {
                inst = this;
            }

            // Inst defined
            if (inst != null)
            {
                // Instance is this mono - > Init
                if (inst == this)
                {
                    // Set vars
                    SetVariables();

                    // Runtime ops
                    if (Application.isPlaying == true)
                        Init();
                }

                // Inst is not this mono. Destroy.
                if (inst != this)
                {
                    if (Application.isPlaying == true)
                        Destroy (gameObject);
                    else if (Application.isEditor == true)
                        DestroyImmediate (gameObject);
                }
            }
        }

        // init ops
        void Init()
        {
            // Start pooling objects for fragments
            SetPooling();

            // Create storage and stat root check coroutine
            SetStorage();
                        
            // Rigid.Physics.Velocity cache
            StartCoroutine (PhysicsManCor ());

            // Rigid. Demolition
            StartCoroutine (DemolitionManCor()); 
        }
        
        /// /////////////////////////////////////////////////////////
        /// Enable/Disable
        /// /////////////////////////////////////////////////////////
        
        // Disable
        void OnDisable()
        {
            fragments.inProgress   = false;
            particles.poolProgress = false;
            physicsDataCorState    = false;
            dmlCorState     = false;
            if (storage != null)
                storage.inProgress   = false;
        }

        // Activation
        void OnEnable()
        {
            if (Application.isPlaying == true && gameObject.activeSelf == true)
                Init();
        }

        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////

        // Set vars
        void SetVariables()
        {
            // Get components
            transForm = GetComponent<Transform>();

            // Reset amount
            advancedDemolitionProperties.ResetCurrentAmount();

            // Set gravity
            SetGravity();

            // Set Physic Materials if needed
            materialPresets.SetMaterials();

            colliderSizeStatic   = colliderSize;
            cookingOptionsStatic = cookingOptions;
            debugStatic          = debug;
            debugBuildStatic     = debugBuild;
            debugEditorStatic    = debugEditor;
            coplanarVertLimit    = coplanarVerts;
        }

        // Set gravity
        void SetGravity()
        {
            if (setGravity == true)
                Physics.gravity = -9.81f * multiplier * Vector3.up;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Physics data cor
        /// /////////////////////////////////////////////////////////
        
        // Collect Rigid.Physics for Velocity cache
        public void AddPhysicList (RayfireRigid rigid)
        {
            if (rigid.dmlTp != DemolitionType.None)
                if (rigid.physics.velCache == false)
                    if (rigid.physics.rb != null)
                    {
                        rigid.physics.velCache = true;
                        physicList.Add (rigid.physics);
                    }
        }

        // Check list, remove null, cache velocity
        void VelocityCache()
        {
            if (physicList.Count > 0)
                for (int i = physicList.Count - 1; i >= 0; i--)
                {
                    if (physicList[i].rb == null)
                    {
                        physicList[i].velCache = false;
                        physicList.RemoveAt (i);
                        continue;
                    }
                    physicList[i].velocity = physicList[i].rb.velocity;
                }
        }
        
        // Rigid.Physics.Velocity cache
        IEnumerator PhysicsManCor ()
        {
            // Stop if running 
            if (physicsDataCorState == true) 
                yield break;
            
            // Set running state
            physicsDataCorState = true;
            
            // Check list, remove null, cache velocity
            VelocityCache();
            
            while (physicsDataCorState == true)
            {
                // Check list, remove null, cache velocity
                VelocityCache();
                
                // TODO check for active state and velCache state every 5-10 seconds
                // TODO set velCache = false at demolition
                
                yield return null;
            }
   
            // Set state
            physicsDataCorState = false;
        }

        /// /////////////////////////////////////////////////////////
        /// Demolition cor
        /// /////////////////////////////////////////////////////////
        
        // Demolition
        IEnumerator DemolitionManCor ()
        {
            // Stop if running 
            if (dmlCorState == true) 
                yield break;
            
            // Set running state
            dmlCorState = true;
            
            while (dmlCorState == true)
            {
                // Check list, remove null
                DmlNullCheck();

                // Reset current frame total demolition time
                dmlThisFrame = 0;
                
                if (dmlList.Count > 0)
                {
                    for (int i = dmlList.Count - 1; i >= 0; i--)
                    {
                        // Debug(dmlList[i].name);
                        // Debug(dmlList.Count.ToString());
                        // TODO replace by demolition data class
  
                                                
                        // Demolition Time quota check
                        if (timeQuota > 0 && dmlThisFrame > timeQuota)
                        {
                            // Continue at next frame
                            if (quotaAction == QuotaType.Postpone)
                                break;
                            
                            // Skip object demolition completely
                            if (quotaAction == QuotaType.Skip)
                            {
                                dmlList.RemoveAt (i);
                                continue;
                            }
                        }
                        
                        // Timestamp
                        float t1 = Time.realtimeSinceStartup;
                        
                        // Init demolition
                        if (dmlList[i].limitations.demolitionShould == true)
                            dmlList[i].Demolish();

                        // Check for slicing planes and init slicing
                        else if (dmlList[i].limitations.bld == true && dmlList[i].limitations.slicePlanes != null && dmlList[i].limitations.slicePlanes.Count > 1)
                            dmlList[i].Slice();
                        
                        // Remove
                        dmlList.RemoveAt (i);
                        
                        // Sum total demolition time
                        dmlThisFrame += Time.realtimeSinceStartup - t1;
                    }
                }

                // Wait for next frame
                yield return null;
            }
   
            // Set state
            dmlCorState = false;
        }

        // Null check, None dml type
        void DmlNullCheck()
        {
            if (dmlList.Count > 0)
                for (int i = dmlList.Count - 1; i >= 0; i--)
                    if (dmlList[i] == null)
                        dmlList.RemoveAt (i);
        }

        // Add Rigid object to demolition list
        public void AddToDemolition(RayfireRigid rigid)
        {
            // Already in list
            if (dmlList.Contains (rigid) == true)
                return;

            // Max depth reached
            if (rigid.limitations.depth > 0 && rigid.limitations.currentDepth >= rigid.limitations.depth)
                rigid.dmlTp = DemolitionType.None;
            
            // None demolition type
            if (rigid.dmlTp == DemolitionType.None)
                return;
            
            // Add at 0 index
            dmlList.Insert (0, rigid);
        }

        /// /////////////////////////////////////////////////////////
        /// Other
        /// /////////////////////////////////////////////////////////
        
        // Create RayFire manager if not created
        public static void RayFireManInit()
        {
            if (inst == null)
            {
                GameObject rfMan = new GameObject ("RayFireMan");
                inst = rfMan.AddComponent<RayfireMan>();
            }

            if (Application.isPlaying == false)
            {
                inst.SetInstance();
            }
        }
        
        // Max fragments amount check
        public static bool MaxAmountCheck
        {
            get
            {
                if (inst.advancedDemolitionProperties.currentAmount < inst.advancedDemolitionProperties.maximumAmount)
                    return true;

                inst.advancedDemolitionProperties.AmountWarning();
                return false;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Pooling
        /// /////////////////////////////////////////////////////////

        // Enable objects pooling for fragments                
        void SetPooling()
        {
            // Setup emitter pooling
            particles.SetupEmitters(transform);
            
            // Pooling. Mot in editor
            if (Application.isPlaying == true && particles.Enable == true && particles.poolProgress == false)
                StartCoroutine (particles.StartPoolingCor ());
            
            // Create pool root
            fragments.CreatePoolRoot (transform);

            // Create pool instance
            fragments.CreateInstance (transform);

            // Pooling. Mot in editor
            if (Application.isPlaying == true && fragments.enable == true && fragments.inProgress == false)
                StartCoroutine (fragments.StartPoolingCor (transForm));
        }
        
        // Emitter Pooling
        public void StartEmitterPooling()
        {
            // Already running
            if (particles.emitProgress == true)
                return;
            
            // Only at play mode
            if (Application.isPlaying == false) 
                return;
            
            // Global particle pooling disabled
            if (particles.Enable == false)
                return;
            
            // All emitters are full
            if (particles.NeedState() == false)
                return;
            
            // Start pooling
            StartCoroutine (particles.StartEmitterPoolingCor ());
        }
        
        /// /////////////////////////////////////////////////////////
        /// Storage
        /// /////////////////////////////////////////////////////////
        
        // Create storage root
        void SetStorage()
        {
            // Create
            if (storage == null)
                storage = new RFStorage();
            
            // Create storage if has no
            storage.CreateStorageRoot (transform);
            
            // Start empty root removing coroutine if not running
            if (Application.isPlaying == true && storage.inProgress == false)
                StartCoroutine (storage.StorageCor ());
        }

        // Destroy all storage objects
        public void DestroyStorage()
        {
            storage.DestroyAll();
        }

        /// /////////////////////////////////////////////////////////
        /// Parent
        /// /////////////////////////////////////////////////////////

        // Set root to manager or to the same parent
        public static void SetParentByManager (Transform tm, Transform original, bool noRegister = false)
        {
            if (inst == null)
                return;
            
            // Storage
            if (inst.advancedDemolitionProperties.parent == FragmentParentType.Manager)
                tm.parent = inst.storage.storageRoot;
            
            // Global parent
            else if (inst.advancedDemolitionProperties.parent == FragmentParentType.GlobalParent 
                     && inst.advancedDemolitionProperties.globalParent != null)
                tm.parent = inst.advancedDemolitionProperties.globalParent;
            
            // Storage if no local parent
            else if (original == null || original.parent == null)
                tm.parent = inst.storage.storageRoot;
            
            // Local parent
            else
                tm.parent = original.parent;

            // Register in storage
            if (noRegister == false)
                inst.storage.RegisterRoot (tm);
        }
        
        // Set root to manager or to the same parent
        public static void SetParentByManager (Transform tm)
        {
            if (inst == null)
                return;

            if (inst.advancedDemolitionProperties.parent == FragmentParentType.Manager)
                tm.parent = inst.storage.storageRoot;
            
            // Global parent
            else if (inst.advancedDemolitionProperties.parent == FragmentParentType.GlobalParent
                     && inst.advancedDemolitionProperties.globalParent != null)
                tm.parent = inst.advancedDemolitionProperties.globalParent;
            
            // Register in storage
            inst.storage.RegisterRoot (tm);
        }
        
        // Get parent for connected cluster detached shards 
        public static Transform GetParentByManager(RayfireRigid scr)
        {
            // Manager parent
            if (inst != null && inst.advancedDemolitionProperties.parent == FragmentParentType.Manager)
                return inst.storage.storageRoot;
            
            // Parent of main cluster
            if (scr.clsDemol.cluster.mainCluster != null && scr.clsDemol.cluster.mainCluster.tm != null)
                return scr.clsDemol.cluster.mainCluster.tm.parent;
            
            // Parent of Rigid
            return scr.transform.parent;
        }

        /// /////////////////////////////////////////////////////////
        /// Destroy/Deactivate Fragment/Shard
        /// /////////////////////////////////////////////////////////

        // Check if fragment is the last child in root and delete root as well
        public static void DestroyFragment (RayfireRigid scr, Transform tm, float time = 0f)
        {
            // Decrement total amount.
            if (Application.isPlaying == true)
                inst.advancedDemolitionProperties.currentAmount--;
            
            // Deactivate
            scr.gameObject.SetActive (false);

            // Destroy
            if (scr.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
                DestroyOp (scr, tm, time);
        }
        
        // Destroy rigidroot shard
        public static void DestroyShard (RayfireRigidRoot scr, RFShard shard)
        {
            // Deactivate
            shard.tm.gameObject.SetActive (false);
            
            // Destroy
            if (scr.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
                DestroyGo (shard.tm.gameObject);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Destroy
        /// /////////////////////////////////////////////////////////
        
        // Check if fragment is the last child in root and delete root as well
        public static void DestroyGo (GameObject go)
        {
            Destroy (go);
        }

        // Check if fragment is the last child in root and delete root as well
        static void DestroyOp (RayfireRigid scr, Transform tm, float time = 0f)
        {
            // Set delay
            if (time == 0)
                time = scr.reset.destroyDelay;

            // Object is going to be destroyed. Timer is on
            scr.reset.toBeDestroyed = true;

            // Destroy object
            inst.fragments.DestroyOrReset (scr, time);

            // Destroy root
            if (tm != null && tm.childCount == 0)
            {
                // TODO collect root in special roots list, check every 10 seconds and destroy if they are empty
                Destroy (tm.gameObject, time);
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Debug
        /// /////////////////////////////////////////////////////////
        
        // Debug message
        public static void Log (string str, UnityEngine.Object go = null)
        {
            // Disabled
            if (debugStatic == false)
                return;
            
            // Only in Editor
            if (debugEditorStatic == true && Application.isEditor == false)
                return;
            
            // Only in Debug build
            if (debugBuildStatic == true && Debug.isDebugBuild == false)
                return;
            
            Debug.Log (str, go);
        }
    }
}

