﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonCore.State;

namespace CommonCore.World
{

    /// <summary>
    /// General utilities for working with (CommonCore) scenes and the objects in them
    /// </summary>
    public static class WorldUtils
    {

        private static GameObject PlayerObject;

        /// <summary>
        /// Gets the player object (or null if it doesn't exist)
        /// </summary>
        public static GameObject GetPlayerObject()
        {
            if (PlayerObject != null)
                return PlayerObject;

            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                PlayerObject = go;
                return go;
            }

            go = GameObject.Find("Player");

            if (go != null)
            {
                PlayerObject = go;
                return go;
            }

            var tf = CoreUtils.GetWorldRoot().FindDeepChild("Player");

            if (tf != null)
                go = tf.gameObject;

            if (go != null)
            {
                PlayerObject = go;
                return go;
            }

            Debug.LogWarning("Couldn't find player!");

            return null;
        }

        /// <summary>
        /// Finds the player and returns their controller (does not guarantee an actual PlayerController!)
        /// </summary>
        public static BaseController GetPlayerController() //TODO split into Get() and TryGet()
        {
            var pc = WorldUtils.GetPlayerObject()?.GetComponent<BaseController>(); //should be safe because GetPlayerObject returns true null
            if (pc != null)
            {
                return pc;
            }
            else
            {
                Debug.LogWarning("Couldn't find PlayerController!");
                return null;
            }
        }
        
        /// <summary>
        /// Finds a child by name, recursively, and ignores placeholders
        /// </summary>
        public static Transform FindDeepChildIgnorePlaceholders(this Transform aParent, string aName)
        {
            Transform result = null;
            foreach (Transform child in aParent)
            {
                if (child.gameObject.name == aName && child.GetComponent<IPlaceholderComponent>() == null)
                {
                    result = child;
                    break;
                }
            }
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChildIgnorePlaceholders(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Finds an object by thing ID (name)
        /// </summary>
        public static GameObject FindObjectByTID(string TID)
        {
            var targetTransform = GameObject.Find("WorldRoot").transform.FindDeepChild(TID);
            if (targetTransform != null)
                return targetTransform.gameObject;
            return null;
        }

        /// <summary>
        /// Finds an entity by thing ID (name)
        /// </summary>
        public static BaseController FindEntityByTID(string TID)
        {
            var targetTransform = GameObject.Find("WorldRoot").transform.FindDeepChild(TID);
            if (targetTransform != null)
            {
                var bc = targetTransform.GetComponent<BaseController>();
                if (bc != null)
                    return bc;
            }
            return null;
        }

        /// <summary>
        /// Finds all entities with form ID (entity name)
        /// </summary>
        public static IList<BaseController> FindEntitiesWithFormID(string formID)
        {
            List<BaseController> foundObjects = new List<BaseController>();
            foreach (BaseController c in CoreUtils.GetWorldRoot().gameObject.GetComponentsInChildren<BaseController>(true))
            {
                if (c.FormID == formID)
                {
                    foundObjects.Add(c);
                }
            }

            return foundObjects;
        }

        /// <summary>
        /// Finds all entities with CommonCore tag
        /// </summary>
        public static IList<BaseController> FindEntitiesWithTag(string tag)
        {
            List<BaseController> foundObjects = new List<BaseController>();
            foreach (BaseController c in CoreUtils.GetWorldRoot().gameObject.GetComponentsInChildren<BaseController>(true))
            {
                if (c.Tags.Contains(tag))
                {
                    foundObjects.Add(c);
                }
            }

            return foundObjects;
        }

        /// <summary>
        /// Sets parameters and loads a different scene
        /// </summary>
        public static void ChangeScene(string scene, string spawnPoint, Vector3 position, Vector3 rotation, bool skipLoading)
        {
            MetaState mgs = MetaState.Instance;
            if (spawnPoint != null)
                mgs.PlayerIntent = new PlayerSpawnIntent(spawnPoint); //handle string.Empty as default spawn point
            else
                mgs.PlayerIntent = new PlayerSpawnIntent(position, Quaternion.Euler(rotation));

            SharedUtils.ChangeScene(scene, skipLoading);
        }

        /// <summary>
        /// Sets parameters and loads a different scene
        /// </summary>
        public static void ChangeScene(string scene, string spawnPoint, Vector3 position, Vector3 rotation)
        {
            ChangeScene(scene, spawnPoint, position, rotation, false);
        }

        /// <summary>
        /// Spawn an entity into the world (entities/*)
        /// </summary>
        public static GameObject SpawnEntity(string formID, string thingID, Vector3 position, Vector3 rotation, Transform parent)
        {
            if (parent == null)
                parent = CoreUtils.GetWorldRoot();

            var prefab = CoreUtils.LoadResource<GameObject>("Entities/" + formID);
            if (prefab == null)
                return null;

            var go = UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler(rotation), parent) as GameObject;
            if (string.IsNullOrEmpty(thingID))
                thingID = string.Format("{0}_{1}", go.name.Replace("(Clone)", "").Trim(), GameState.Instance.NextUID);
            go.name = thingID;
            return go;
        }

        /// <summary>
        /// Spawn an effect into the world (Effects/*)
        /// </summary>
        public static GameObject SpawnEffect(string effectID, Vector3 position, Vector3 rotation, Transform parent) => SpawnEffect(effectID, position, rotation, parent, false);

        /// <summary>
        /// Spawn an effect into the world (Effects/*)
        /// </summary>
        public static GameObject SpawnEffect(string effectID, Vector3 position, Vector3 rotation, Transform parent, bool useUniqueId)
        {
            if (parent == null)
                parent = CoreUtils.GetWorldRoot();

            var prefab = CoreUtils.LoadResource<GameObject>("Effects/" + effectID);
            if (prefab == null)
                return null;

            var go = UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler(rotation), parent) as GameObject;
            go.name = string.Format("{0}_{1}", go.name.Replace("(Clone)", "").Trim(), useUniqueId ? GameState.Instance.NextUID.ToString() : "fx");

            return go;
        }

        /// <summary>
        /// Check if this object is considered a CommonCore Entity
        /// </summary>
        public static bool IsEntity(this GameObject gameObject)
        {
            return gameObject.Ref()?.GetComponent<BaseController>() != null;
        }

        /// <summary>
        /// Checks if this object is considered the player object
        /// </summary>
        public static bool IsPlayer(this GameObject gameObject)
        {
            return gameObject == GetPlayerObject();
        }

        /// <summary>
        /// Checks if this object is considered an "actor" object
        /// </summary>
        public static bool IsActor(this GameObject gameObject)
        {
            var bc = gameObject.Ref()?.GetComponent<BaseController>();
            if (bc != null && bc.Tags.Contains("Actor"))
                return true;

            return false;
        }

        public static LayerMask GetAttackLayerMask()
        {
            return LayerMask.GetMask("Default", "ActorHitbox", "Actor");
        }

        /// <summary>
        /// Raycasts and gets the closest/best hit on an IHitboxComponent or ITakeDamage
        /// </summary>
        /// <remarks>
        /// <para>Hits on originator will always be ignored. If you don't want to, leave originator blank</para>
        /// </remarks>
        public static HitInfo RaycastAttackHit(Vector3 origin, Vector3 direction, float range, bool rejectBullets, bool useSubHitboxes, BaseController originator)
        {
            var hits = Physics.RaycastAll(origin, direction, range, GetAttackLayerMask(), QueryTriggerInteraction.Collide);

            //no hits, return default
            if(hits.Length == 0)
                return new HitInfo(null, null, default, default, default);

            return GetAttackHit(hits, rejectBullets, useSubHitboxes, originator);
        }

        /// <summary>
        /// Raycasts and gets the closest/best hit on an IHitboxComponent or ITakeDamage (loose variant)
        /// </summary>
        /// <remarks>
        /// <para>Hits on originator will always be ignored. If you don't want to, leave originator blank</para>
        /// </remarks>
        public static HitInfo SpherecastAttackHit(Vector3 origin, Vector3 direction, float radius, float range, bool rejectBullets, bool useSubHitboxes, BaseController originator)
        {
            var hits = Physics.SphereCastAll(origin, radius, direction, range, GetAttackLayerMask(), QueryTriggerInteraction.Collide);

            //no hits, return default
            if (hits.Length == 0)
                return new HitInfo(null, null, default, default, default);

            return GetAttackHit(hits, rejectBullets, useSubHitboxes, originator);
        }

        /// <summary>
        /// Gets the closest/best hit on an IHitboxComponent or ITakeDamage
        /// </summary>
        /// <remarks>
        /// <para>Hits on originator will always be ignored. If you don't want to, leave originator blank</para>
        /// <para>This variant doesn't raycast and expects you to do it yourself.</para>
        /// </remarks>
        public static HitInfo GetAttackHit(IEnumerable<RaycastHit> hits, bool rejectBullets, bool useSubHitboxes, BaseController originator)
        {
            RaycastHit closestHit = default;
            closestHit.distance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.distance < closestHit.distance)
                {
                    //reject bullets
                    if (rejectBullets && hit.collider.GetComponent<BulletScript>())
                        continue;

                    if (hit.collider.isTrigger) //if it's non-solid, it only counts if it's a hitbox
                    {
                        var ihc = hit.collider.GetComponent<IHitboxComponent>();
                        if (ihc != null && (originator == null || ihc.ParentController != originator)) //handle originator
                            closestHit = hit;
                    }
                    else //if it's solid, closer always counts
                    {
                        if (originator != null)
                        {
                            var ihc = hit.collider.GetComponent<IHitboxComponent>();
                            if (ihc != null && ihc.ParentController == originator)
                                continue;
                            var bc = hit.collider.GetComponent<BaseController>();
                            if (bc != null && bc == originator)
                                continue;

                            closestHit = hit;
                        }
                        else
                            closestHit = hit;
                    }

                }
            }

            //Debug.Log($"{closestHit.collider.Ref()?.name}");

            //sentinel: we didn't hit anything
            if (closestHit.distance == float.MaxValue)
                return new HitInfo(null, null, default, default, default);

            //try to find an actor hitbox
            var actorHitbox = closestHit.collider.GetComponent<IHitboxComponent>();
            if (actorHitbox != null)
                return new HitInfo(actorHitbox.ParentController, actorHitbox, closestHit.point, actorHitbox.HitLocationOverride, actorHitbox.HitMaterial);

            //try to find a basecontroller
            var otherController = closestHit.collider.GetComponent<BaseController>();
            if (otherController == null)
                otherController = closestHit.collider.GetComponentInParent<BaseController>();

            //special case: see if we have a more specific hitbox we can use (headshots mostly)
            if (otherController != null && useSubHitboxes)
            {
                foreach (var hit in hits)
                {
                    var specificActorHitbox = hit.collider.GetComponent<IHitboxComponent>();
                    if (specificActorHitbox != null && specificActorHitbox.ParentController == otherController)
                        return new HitInfo(otherController, specificActorHitbox, hit.point, specificActorHitbox.HitLocationOverride, specificActorHitbox.HitMaterial);
                }
            }

            return new HitInfo(otherController, null, closestHit.point, 0, otherController?.HitMaterial ?? 0);
        }


    }
}