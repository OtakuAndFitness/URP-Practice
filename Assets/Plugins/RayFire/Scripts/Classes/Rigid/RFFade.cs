﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RayFire
{
    [Serializable]
    public class RFFade
    {
        // UI
        public bool           onDemolition;
        public bool           onActivation; 
        public float          byOffset;
        public FadeType       fadeType;
        public float          fadeTime;
        public RFFadeLifeType lifeType;
        public float          lifeTime;
        public float          lifeVariation;
        public float          sizeFilter;
        public int            shardAmount;
        
        // Non Serialized
        [NonSerialized] public int         state; // 1-Living, 2-Fading, 3-Faded
        [NonSerialized] public bool        stop;
        [NonSerialized] public bool        offsetCorState;
        [NonSerialized] public IEnumerator offsetEnum;
        
        // Event
        public RFFadingEvent fadingEvent = new RFFadingEvent();
        
        /// /////////////////////////////////////////////////////////
        /// Constructor
        /// /////////////////////////////////////////////////////////

        // Constructor
        public RFFade()
        {
            InitValues();
            LocalReset();
        }
        
        void InitValues()
        {
            onDemolition  = true;
            onActivation  = false;
            byOffset      = 0f;
            fadeType      = FadeType.None;
            fadeTime      = 5f;
            lifeType      = RFFadeLifeType.ByLifeTime;
            lifeTime      = 7f;
            lifeVariation = 3f;
            sizeFilter    = 0f;
            shardAmount   = 5;
        }
        
        // Reset
        public void LocalReset()
        {
            state          = 0;
            stop           = false;
            offsetCorState = false;
            offsetEnum     = null;
        }
        
        // Pool Reset
        public void GlobalReset()
        {
            InitValues();
            LocalReset();
            fadingEvent = new RFFadingEvent();
        }
        
        // Copy from
        public void CopyFrom (RFFade source)
        {
            onDemolition  = source.onDemolition;
            onActivation  = source.onActivation;
            byOffset      = source.byOffset;
            lifeType      = source.lifeType;
            lifeTime      = source.lifeTime;
            lifeVariation = source.lifeVariation;
            fadeType      = source.fadeType;
            fadeTime      = source.fadeTime;
            sizeFilter    = source.sizeFilter;
            shardAmount   = source.shardAmount;

            LocalReset();
        }

        /// /////////////////////////////////////////////////////////
        /// Fade for demolished fragments
        /// /////////////////////////////////////////////////////////

        // Fading init from parent node
        public void DemolitionFade (List<RayfireRigid> fadeObjects)
        {
            // No fading
            if (fadeType == FadeType.None)
                return;

            // No objects
            if (fadeObjects.Count == 0)
                return;
            
            // Add Fade script and init fading
            for (int i = 0; i < fadeObjects.Count; i++)
            {
                // Check for null
                if (fadeObjects[i] == null)
                    continue;
                
                // Init fading
                FadeRigid (fadeObjects[i]);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Offset fade coroutine
        /// /////////////////////////////////////////////////////////
        
        // Check offset for fade
        public static IEnumerator FadeOffsetCor (RayfireRigid scr)
        {
            // Object living, fading or faded
            if (scr.fading.state > 0)
                yield break;
            
            // Stop if running 
            if (scr.fading.offsetCorState == true)
                yield break;

            // Repeat check time
            WaitForSeconds delay = new WaitForSeconds (2f);
            
            // Random start
            yield return new WaitForSeconds (Random.Range(0f, 2f));
            
            // Set running state
            scr.fading.offsetCorState = true;
            
            // Check
            while (scr.fading.state == 0 && scr.fading.byOffset > 0)
            {
                if (Vector3.Distance (scr.tsf.position, scr.physics.initPosition) > scr.fading.byOffset)
                {
                    scr.Fade();
                    break;
                }
                
                yield return delay;
            }
            
            // Set state
            scr.fading.offsetCorState = false;
        }
        
        // Check offset for fade
        public static IEnumerator FadeOffsetCor (RayfireRigidRoot scr)
        {
            // Stop if running 
            if (scr.fading.offsetCorState == true)
                yield break;

            // Repeat check time
            WaitForSeconds delay = new WaitForSeconds (2f);
            
            // Random start
            yield return new WaitForSeconds (Random.Range(0f, 2f));
            
            // Set running state
            scr.fading.offsetCorState = true;

            // Check all shards
            while (scr.offsetFadeShards.Count > 0)
            {
                for (int i = scr.offsetFadeShards.Count - 1; i >= 0; i--)
                {
                    // Remove if shards was clustered by Connectivity into Connected Cluster
                    if (scr.offsetFadeShards[i].fade == -1)
                    {
                        scr.offsetFadeShards.RemoveAt (i);
                        continue;
                    }
                    
                    // Calculate offset distance and fade if big enough, remove from list
                    if (Vector3.Distance (scr.offsetFadeShards[i].tm.position, scr.offsetFadeShards[i].pos) > scr.offsetFadeShards[i].fo)
                    {
                        FadeShard (scr, scr.offsetFadeShards[i]);
                        scr.offsetFadeShards.RemoveAt (i);
                    }
                }

                yield return delay;
            }
            
            // Set state
            scr.fading.offsetCorState = false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Object Fade init
        /// /////////////////////////////////////////////////////////

        // Fading init for Rigid object
        public static void FadeRigid (RayfireRigid scr)
        {
            // No fading
            if (scr.fading.fadeType == FadeType.None)
                return;

            // Object inactive, Skip
            if (scr.gameObject.activeSelf == false)
                return;

            // Initialize if not
            if (scr.initialized == false)
                scr.Initialize();

            // Size check
            if (scr.fading.sizeFilter > 0 && scr.limitations.bboxSize > scr.fading.sizeFilter)
                return;
            
            // Shard amount check
            if (scr.fading.shardAmount > 0 && scr.objTp == ObjectType.ConnectedCluster)
                if (scr.clsDemol.cluster.shards.Count > scr.fading.shardAmount)
                    return;

            // Object living, fading or faded
            if (scr.fading.state > 0)
                return;

            // Start life coroutine
            scr.StartCoroutine (LivingCor (scr));

            // Stop Physics data cor. Not for nested clusters TODO use if rigid is not going to be reset
            //if (scr.objectType != ObjectType.NestedCluster && scr.physics.physicsEnum != null)
            //    scr.StopCoroutine (scr.physics.physicsEnum);
        }
        
        // Fading init for Shard object
        public static void FadeShard (RayfireRigidRoot scr, RFShard shard)
        {
            // No fading
            if (scr.fading.fadeType == FadeType.None)
                return;
            
            // Shard living, fading or faded
            if (shard.fade > 0)
                return;
            
            // Size check
            if (scr.fading.sizeFilter > 0 && shard.sz > scr.fading.sizeFilter)
                return;
            
            // Start life coroutine
            scr.StartCoroutine (LivingCor (scr, shard));
        }

        // Fade Cluster's detached rigid fragments or Shards if cluster has RigidRoot parent
        public static void FadeClusterShards(RayfireRigid scr, List<RFShard> fadeShards)
        {
            if (scr.fading.onDemolition == true)
            {
                // Fading for detached fragments
                if (scr.rigidRoot == null)
                    scr.fading.DemolitionFade (scr.fragments);

                // Fading for detached shards because has Rigid Root
                else
                    for (int i = 0; i < fadeShards.Count; i++)
                        FadeShard (scr.rigidRoot, fadeShards[i]);

                // Self fade for main cluster
                scr.Fade();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Living
        /// /////////////////////////////////////////////////////////

        // Start life coroutine
        static IEnumerator LivingCor (RayfireRigid scr)
        {
            // Set living
            scr.fading.state = 1;
            
            // Wait for simulation get rest
            if (scr.fading.lifeType == RFFadeLifeType.BySimulationAndLifeTime)
                yield return scr.StartCoroutine(SimulationLivingCor (scr.tsf));
            
            // Get final life duration
            float lifeDuration = scr.fading.lifeTime;
            if (scr.fading.lifeVariation > 0)
                lifeDuration += Random.Range (0f, scr.fading.lifeVariation);
            
            // Wait life time
            if (lifeDuration > 0)
                yield return new WaitForSeconds (lifeDuration);

            // Stop fading
            if (scr.fading.stop == true)
            {
                scr.fading.LocalReset();
                yield break;
            }
            
            // Set fading
            scr.fading.state = 2;
            
            // Event
            scr.fading.fadingEvent.InvokeLocalEvent (scr.transform);
            RFFadingEvent.InvokeGlobalEvent (scr.transform);

            // Exclude from simulation and keep object in scene
            if (scr.fading.fadeType == FadeType.SimExclude)
                FadeExclude (scr);

            // Fall under ground, destroy
            else if (scr.fading.fadeType == FadeType.FallDown)
                scr.StartCoroutine (FallDownCor (scr));

            // Start scale down and destroy
            else if (scr.fading.fadeType == FadeType.ScaleDown)
                scr.StartCoroutine (ScaleDownCor (scr));

            // Exclude from simulation, Move down and destroy
            else if (scr.fading.fadeType == FadeType.MoveDown)
                scr.StartCoroutine (FadeMoveDownCor (scr));
            
            // Destroy object
            else if (scr.fading.fadeType == FadeType.Destroy)
            {
                // Set faded and destroy
                scr.fading.state = 3;
                RayfireMan.DestroyFragment (scr, scr.rtP);
            }
            
            // Static object
            else if (scr.fading.fadeType == FadeType.SetStatic)
            {
                // Set faded and destroy rigidbody
                scr.fading.state = 3;
                Object.Destroy (scr.physics.rb);
                scr.physics.rb = null;
            }
            
            // Kinematic object
            else if (scr.fading.fadeType == FadeType.SetKinematic)
            {
                // Set faded and set kinematic
                scr.fading.state                  = 3;
                scr.physics.rb.isKinematic = true;
            }
        }
        
        // Start life coroutine
        static IEnumerator LivingCor (RayfireRigidRoot root, RFShard shard)
        {
            // Wait for simulation get rest
            if (root.fading.lifeType == RFFadeLifeType.BySimulationAndLifeTime)
                yield return root.StartCoroutine(SimulationLivingCor (shard.tm));

            // Set living
            shard.fade = 1;
            
            // Get final life duration
            float lifeDuration = root.fading.lifeTime;
            if (root.fading.lifeVariation > 0)
                lifeDuration += Random.Range (0f, root.fading.lifeVariation);
            
            // Wait life time
            if (lifeDuration > 0)
                yield return new WaitForSeconds (lifeDuration);

            // Set fading
            shard.fade = 2;
            
            // Event
            root.fading.fadingEvent.InvokeLocalEvent (shard.tm);
            RFFadingEvent.InvokeGlobalEvent (shard.tm);
            
            // Exclude from simulation and keep object in scene
            if (root.fading.fadeType == FadeType.SimExclude)
                FadeExclude (root, shard);
            
            // Exclude from simulation, fall under ground, destroy
            else if (root.fading.fadeType == FadeType.FallDown)
                root.StartCoroutine (FallDownCor (root, shard));
            
            // Start scale down and destroy
            else if (root.fading.fadeType == FadeType.ScaleDown)
                root.StartCoroutine (ScaleDownCor (root, shard));

            // Exclude from simulation, Move down and destroy
            else if (root.fading.fadeType == FadeType.MoveDown)
                root.StartCoroutine (FadeMoveDownCor (root, shard));
            
            // // Destroy/Deactivate
            else if (root.fading.fadeType == FadeType.Destroy)
            {
                // Set faded and destroy
                shard.fade = 3;
                RayfireMan.DestroyShard (root, shard);
            }
            
            // Static object
            else if (root.fading.fadeType == FadeType.SetStatic)
            {
                // Set faded and destroy rigidbody
                shard.fade = 3;
                Object.Destroy (shard.rb);
            }

            // Kinematic object
            else if (root.fading.fadeType == FadeType.SetKinematic)
            {
                // Set faded and set kinematic
                shard.fade           = 3;
                shard.rb.isKinematic = true;
            }
        }
        
        // Check for simulation state
        static IEnumerator SimulationLivingCor (Transform tm)
        {
            float   timeStep = Random.Range (2.5f, 3.5f);
            Vector3 oldPos;
            float   distanceThreshold = 0.15f;
            
            // Wait
            WaitForSeconds wait = new WaitForSeconds (timeStep);
            
            while (true)
            {
                // Save position
                oldPos = tm.position;
                
                // Wait step time
                yield return wait;
                
                if (Vector3.Distance (tm.position, oldPos) < distanceThreshold)
                    break;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Exclude
        /// /////////////////////////////////////////////////////////
        
        // Exclude from simulation and keep object in scene
        static void FadeExclude (RayfireRigid rigid)
        {
            // Set faded
            rigid.fading.state = 3;

            // Not going to be reused
            if (rigid.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
            {
                UnityEngine.Object.Destroy (rigid.physics.rb);
                UnityEngine.Object.Destroy (rigid.physics.mc);
                UnityEngine.Object.Destroy (rigid);
            }

            // Going to be reused 
            else if (rigid.reset.action == RFReset.PostDemolitionType.DeactivateToReset)
            {
                // Set kinematic
                rigid.physics.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rigid.physics.rb.isKinematic            = true;
                
                // Disable mesh collider // Null check because of Planar check fragments without collider
                if (rigid.objTp == ObjectType.Mesh && rigid.physics.mc != null)
                    rigid.physics.mc.enabled = false;
                
                // Disable cluster colliders TODO test nested cluster
                else if (rigid.objTp == ObjectType.ConnectedCluster || rigid.objTp == ObjectType.NestedCluster)
                    for (int i = 0; i < rigid.physics.cc.Count; i++)
                        rigid.physics.cc[i].enabled = false;
                
                // Stop all cors
                rigid.StopAllCoroutines();
            }
        }
        
        // Exclude from simulation and keep object in scene
        static void FadeExclude (RayfireRigidRoot root, RFShard shard)
        {
            // Faded
            shard.fade = 3;
            
            // Not going to be reused
            if (root.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
            {
                UnityEngine.Object.Destroy (shard.rb);
                UnityEngine.Object.Destroy (shard.col);
            }

            // Going to be reused 
            else if (root.reset.action == RFReset.PostDemolitionType.DeactivateToReset)
            {
                shard.rb.isKinematic = true;
                shard.col.enabled    = false;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Fall Down
        /// /////////////////////////////////////////////////////////
        
        // Exclude from simulation, fall under ground, destroy
        static IEnumerator FallDownCor (RayfireRigid rigid)
        {
            // Activate inactive
            if (rigid.simTp == SimType.Inactive)
                rigid.Activate();

            // Wake up if sleeping
            rigid.physics.rb.WakeUp();
            
            // Turn off collider
            if (rigid.objTp == ObjectType.Mesh && rigid.physics.mc != null)
                rigid.physics.mc.enabled = false;
            
            else if (rigid.objTp == ObjectType.ConnectedCluster || rigid.objTp == ObjectType.NestedCluster)
                DisableClusterColliders (rigid);
            
            // Wait to fall down
            yield return new WaitForSeconds (rigid.fading.fadeTime);
            
            // Set faded
            rigid.fading.state = 3;
            
            // Check if fragment is the last child in root and delete root as well
            RayfireMan.DestroyFragment (rigid, rigid.rtP);
        }
        
        // Exclude from simulation, move under ground, destroy
        static IEnumerator FallDownCor (RayfireRigidRoot root, RFShard shard)
        {
            // Activate inactive
            if (shard.sm == SimType.Inactive)
                RFActivation.ActivateShard (shard, root);

            // Wake up if sleeping
            shard.rb.WakeUp();
            
            // Turn off collider
            if (shard.col != null)
                shard.col.enabled = false;

            // Wait to fall down
            yield return new WaitForSeconds (root.fading.fadeTime);
            
            // Faded
            shard.fade = 3;
            
            // Destroy/Deactivate
            RayfireMan.DestroyShard (root, shard);
        }
                
        /// /////////////////////////////////////////////////////////
        /// Scale Down
        /// /////////////////////////////////////////////////////////
        
        // Exclude from simulation, move under ground, destroy
        static IEnumerator ScaleDownCor (RayfireRigid scr)
        {
            // Scale object down during fade time
            float   waitStep   = 0.04f;
            int     steps      = (int)(scr.fading.fadeTime / waitStep);
            Vector3 vectorStep = scr.tsf.localScale / steps;

            // Wait
            WaitForSeconds wait = new WaitForSeconds (waitStep);
            
            // Repeat
            while (steps > 0)
            {
                steps--;
                
                // Scale down
                scr.tsf.localScale -= vectorStep;
                
                // Wait
                yield return wait;
               
                // Destroy when too small
                if (steps < 4)
                {
                    // Set faded
                    scr.fading.state = 3;
                    
                    // Destroy
                    RayfireMan.DestroyFragment (scr, scr.rtP);
                    
                    yield break;
                }
            }
        }
        
        // Exclude from simulation, move under ground, destroy
        static IEnumerator ScaleDownCor (RayfireRigidRoot root, RFShard shard)
        {
            // Scale object down during fade time
            float   waitStep   = 0.04f;
            int     steps      = (int)(root.fading.fadeTime / waitStep);
            Vector3 vectorStep = shard.tm.localScale / steps;
            
            // Wait
            WaitForSeconds wait = new WaitForSeconds (waitStep);
            
            // Repeat
            while (steps > 0)
            {
                if (shard.tm == null)
                    break;

                steps--;
                
                // Scale down
                shard.tm.localScale -= vectorStep;
                
                // Wait
                yield return wait;

                // Destroy when too small
                if (steps < 4)
                {
                    // Faded
                    shard.fade = 3;
                    
                    // Destroy/Deactivate
                    RayfireMan.DestroyShard (root, shard);
                    
                    yield break;
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Move Down
        /// /////////////////////////////////////////////////////////

        // Exclude from simulation, fall under ground, destroy
        static IEnumerator FadeMoveDownCor (RayfireRigid rigid)
        {
            // Stop simulation
            rigid.physics.rb.useGravity      = false;
            rigid.physics.rb.velocity        = Vector3.zero;
            rigid.physics.rb.angularVelocity = Vector3.zero;
            
            // Scale object down during fade time
            float   extraSize  = 1.2f;
            float   waitStep   = 0.03f;
            int     steps      = (int)(rigid.fading.fadeTime / waitStep);
            Vector3 vectorStep = rigid.limitations.bboxSize * extraSize / steps * Vector3.down;
            
            // Turn off collider
            if (rigid.objTp == ObjectType.Mesh && rigid.physics.mc != null)
            {
                // Disable colliders
                rigid.physics.mc.enabled = false;
                
                // Set correct vector by renderer size
                vectorStep = rigid.mRnd.bounds.size.y * extraSize / steps * Vector3.down;
            }
            else if (rigid.objTp == ObjectType.ConnectedCluster || rigid.objTp == ObjectType.NestedCluster)
                DisableClusterColliders (rigid);

            // Wait
            WaitForSeconds wait = new WaitForSeconds (waitStep);
            
            // Move down for size distance
            while (steps > 0)
            {
                steps--;
                
                // Scale down
                rigid.tsf.Translate (vectorStep, Space.World);
                
                // Wait
                yield return wait;
               
                // Destroy when too small
                if (steps < 4)
                {
                    // Set faded
                    rigid.fading.state = 3;
                    
                    // Destroy
                    RayfireMan.DestroyFragment (rigid, rigid.rtP);
                    
                    yield break;
                }
            }
        }
        
        // Exclude from simulation, move under ground, destroy
        static IEnumerator FadeMoveDownCor (RayfireRigidRoot root, RFShard shard)
        {
            // Stop simulation
            shard.rb.useGravity      = false;
            shard.rb.velocity        = Vector3.zero;
            shard.rb.angularVelocity = Vector3.zero;
            
            // Scale object down during fade time
            float   extraSize  = 1.2f;
            float   waitStep   = 0.03f;
            int     steps      = (int)(root.fading.fadeTime / waitStep);
            Vector3 vectorStep = shard.sz * extraSize / steps * Vector3.down;
            
            // Turn off collider
            if (shard.col != null)
                shard.col.enabled = false;

            // Wait
            WaitForSeconds wait = new WaitForSeconds (waitStep);
            
            // Wait to fall down
            yield return new WaitForSeconds (root.fading.fadeTime);
            
            // Move down for size distance
            while (steps > 0)
            {
                steps--;
                
                // Scale down
                shard.tm.Translate (vectorStep, Space.World);
                
                // Wait
                yield return wait;
               
                // Destroy when too small
                if (steps < 4)
                {
                    // Set faded
                    shard.fade = 3;
                    
                    // Destroy
                    RayfireMan.DestroyShard (root, shard);
                    
                    yield break;
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////
        
        // Disable cluster colliders and set fade state for shards
        static void DisableClusterColliders (RayfireRigid rigid)
        {
            if (rigid.physics.HasClusterColliders == true)
            {
                // Disable colliders
                for (int i = 0; i < rigid.physics.cc.Count; i++)
                    if (rigid.physics.cc[i] != null)
                        rigid.physics.cc[i].enabled = false;

                // Set fade state for shards
                for (int i = 0; i < rigid.clsDemol.cluster.shards.Count; i++)
                    rigid.clsDemol.cluster.shards[i].fade = 3;
            }
        }
        
        // Setup offset fading shards for rigid root
        public static void SetOffsetFadeList (RayfireRigidRoot root)
        {
            // Setup offset fade list
            if (root.offsetFadeShards == null)
                root.offsetFadeShards = new List<RFShard>();
            else
                root.offsetFadeShards.Clear();

            // Collect rigidRoot shards with offset fade
            if (root.fading.byOffset > 0)
            {
                root.offsetFadeShards.Capacity = root.rigidRootShards.Count;
                for (int i = 0; i < root.rigidRootShards.Count; i++)
                {
                    root.rigidRootShards[i].fo  = root.fading.byOffset;
                    root.rigidRootShards[i].pos = root.rigidRootShards[i].tm.position;
                    root.rigidRootShards[i].los = root.rigidRootShards[i].tm.localPosition;
                    root.offsetFadeShards.Add (root.rigidRootShards[i]);
                }
            }

            // Collect meshRoot shards with offset fade
            for (int i = 0; i < root.meshRootShards.Count; i++)
                if (root.meshRootShards[i].rigid.fading.byOffset > 0)
                {
                    root.meshRootShards[i].fo  = root.meshRootShards[i].rigid.fading.byOffset;
                    root.meshRootShards[i].pos = root.meshRootShards[i].tm.position;
                    root.meshRootShards[i].los = root.meshRootShards[i].tm.localPosition;
                    root.offsetFadeShards.Add (root.meshRootShards[i]);
                }
        }
    }
}