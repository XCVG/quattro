using CommonCore;
using CommonCore.Async;
using CommonCore.Scripting;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class EasterEggHack
{

    [CCScript, CCScriptHook(Hook = ScriptHook.AfterMainMenuCreate)]
    public static void InjectEasterEggHack()
    {
        AsyncUtils.RunWithExceptionHandling(EasterEggAsync);
    }


    public static async Task EasterEggAsync()
    {
        //await Task.Delay(1000 * 10); //10 seconds

        //await Task.Delay(1000 * 60 * 10); //10 minutes

        await Task.Delay(1000 * 60 * 60); //an hour

        AsyncUtils.ThrowIfEditorStopped();

        SharedUtils.ChangeScene("EasterEggScene");
    }


}
