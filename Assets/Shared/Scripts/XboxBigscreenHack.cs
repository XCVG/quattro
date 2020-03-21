using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonCore.Scripting;
using CommonCore.Config;
using CommonCore;

public static class XboxBigscreenHack
{
    [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterMainMenuCreate)]
    public static void DoXboxBigscreenHack()
    {
        //set bigscreen mode based on whether we're on an Xbox
        if(SystemInfo.deviceType == DeviceType.Console)
        {
            ConfigState.Instance.UseBigScreenMode = true;
        }

        if(ConfigState.Instance.UseBigScreenMode)
        {
            SceneManager.LoadScene("BigscreenMainMenuScene");
        }
    }
}
