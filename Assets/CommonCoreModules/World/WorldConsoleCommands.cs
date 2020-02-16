﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using CommonCore.State;
using CommonCore.Console;

namespace CommonCore.World
{
    /// <summary>
    /// World/object console commands (general)
    /// </summary>
    public static class WorldConsoleCommands
    {
        //TODO cleanup and XML comments

        public static GameObject SelectedObject { get; private set; }
        public static string SelectedTID { get; private set; }

        //***** CHEATS

        [Command]
        static void God()
        {
            if (MetaState.Instance.SessionFlags.Contains("GodMode"))
                MetaState.Instance.SessionFlags.Remove("GodMode");
            else
                MetaState.Instance.SessionFlags.Add("GodMode");
        }

        [Command]
        static void Noclip()
        {
            if (MetaState.Instance.SessionFlags.Contains("NoClip"))
                MetaState.Instance.SessionFlags.Remove("NoClip");
            else
                MetaState.Instance.SessionFlags.Add("NoClip");
        }

        [Command]
        static void Notarget()
        {
            if (MetaState.Instance.SessionFlags.Contains("NoTarget"))
                MetaState.Instance.SessionFlags.Remove("NoTarget");
            else
                MetaState.Instance.SessionFlags.Add("NoTarget");
        }

        //***** SCENE WARP

        [Command]
        static void Warp(string scene, string spawnPoint)
        {
            WorldUtils.ChangeScene(scene, spawnPoint, Vector3.zero, Vector3.zero);
        }

        [Command]
        static void Warp(string scene, Vector3 position)
        {
            Warp(scene, position, Vector3.zero);
        }

        [Command]
        static void Warp(string scene, Vector3 position, Vector3 rotation)
        {
            WorldUtils.ChangeScene(scene, null, position, rotation);
        }

        //***** STATE/PLAYERFLAG MANIPULATION

        [Command (alias = "ListFlags", className = "Player")]
        static void ListPlayerFlags()
        {
            string playerFlags = GameState.Instance.PlayerFlags.GetAllFlags().ToNiceString();
            ConsoleModule.WriteLine(playerFlags);
        }

        [Command(alias = "AddFlag", className = "Player")]
        static void AddPlayerFlag(string flag)
        {
            GameState.Instance.PlayerFlags.Add(flag);
        }

        [Command(alias = "RemoveFlag", className = "Player")]
        static void RemovePlayerFlag(string flag)
        {
            GameState.Instance.PlayerFlags.Remove(flag);
        }

        [Command(alias = "AddTempFlag", className = "Player")]
        static void AddTempPlayerFlag(string flag)
        {
            var sceneController = SharedUtils.TryGetSceneController() as WorldSceneController;
            if (sceneController != null)
                sceneController.TempPlayerFlags.Add(flag);
        }

        [Command(alias = "RemoveTempFlag", className = "Player")]
        static void RemoveTempPlayerFlag(string flag)
        {
            var sceneController = SharedUtils.TryGetSceneController() as WorldSceneController;
            if (sceneController != null)
                sceneController.TempPlayerFlags.Remove(flag);
        }

        //***** OBJECT MANIPULATION

        [Command]
        static void Spawn(string fid)
        {
            WorldUtils.SpawnEntity(fid, null, (WorldUtils.GetPlayerObject().transform.position + (WorldUtils.GetPlayerObject().transform.forward * 1.0f)), Vector3.zero, null);
        }

        //pick object by form id
        [Command]
        static void Prid(string fid)
        {
            var objs = WorldUtils.FindEntitiesWithFormID(fid);
            if (objs == null || objs.Count < 1)
            {
                ConsoleModule.WriteLine("No refs found with form id!");
                return;
            }
            if (objs.Count > 1)
                ConsoleModule.WriteLine("More than one ref with form id!");
            var obj = objs[0];
            if (obj != null && obj.GetComponent<BaseController>())
            {
                SelectedTID = obj.gameObject.name;
                SelectedObject = obj.gameObject;
            }
            else
            {
                ConsoleModule.WriteLine("Ref null or invalid!");
            }

            ConsoleModule.WriteLine("Found TID: " + SelectedTID);
        }

        //pick object by tag (ARES tag, not Unity tag)
        [Command]
        static void Prbt(string tag)
        {
            var objs = WorldUtils.FindEntitiesWithTag(tag);
            if (objs == null || objs.Count < 1)
            {
                ConsoleModule.WriteLine("No refs found with tag!");
                return;
            }
            if (objs.Count > 1)
                ConsoleModule.WriteLine("More than one ref with tag!");
            var obj = objs[0];
            if (obj != null && obj.GetComponent<BaseController>())
            {
                SelectedTID = obj.gameObject.name;
                SelectedObject = obj.gameObject;
            }
            else
            {
                ConsoleModule.WriteLine("Ref null or invalid!");
            }

            ConsoleModule.WriteLine("Found TID: " + SelectedTID);
        }

        //pick object by TID
        [Command]
        static void Pick(string tid)
        {
            var obj = WorldUtils.FindObjectByTID(tid);
            if (obj == null)
            {
                ConsoleModule.WriteLine("Couldn't find TID!");
                return;
            }
            if (obj.GetComponent<BaseController>() == null)
            {
                ConsoleModule.WriteLine("Ref has no controller!");
            }

            SelectedTID = tid;
            SelectedObject = obj;

            ConsoleModule.WriteLine("Found TID: " + tid);
        }

        //display info of selected ref
        [Command]
        static void GetInfo()
        {
            if (SelectedObject != null)
            {
                StringBuilder sb = new StringBuilder(256);

                //name, form id
                sb.AppendFormat("TID: {0} | FID: {1} \n", SelectedObject.name, SelectedObject.GetComponent<BaseController>().Ref()?.FormID ?? "N/A");

                //coords
                sb.AppendFormat("Location: ({0:f2},{1:f2},{2:f2})\n", SelectedObject.transform.position.x, SelectedObject.transform.position.y, SelectedObject.transform.position.z);

                //enabled? active?
                sb.AppendFormat("Active: {0} | Visible: {1}", SelectedObject.activeSelf, SelectedObject.GetComponent<BaseController>().Ref()?.GetVisibility().ToString() ?? "N/A");

                ConsoleModule.WriteLine(sb.ToString());
            }
            else
            {
                ConsoleModule.WriteLine("No object selected!");
            }
        }

        //deselect ref
        [Command]
        static void Dref()
        {
            SelectedTID = null;
            SelectedObject = null;
        }

        //***** ACTOR MANIPULATION
        

        [Command]
        static void Enable()
        {
            SelectedObject.SetActive(true);
        }

        [Command]
        static void Disable()
        {
            SelectedObject.SetActive(false);
        }

        [Command]
        static void Destroy()
        {
            GameObject.Destroy(SelectedObject);
        }

    }
}
