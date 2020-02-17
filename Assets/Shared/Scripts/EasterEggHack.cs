using CommonCore;
using CommonCore.Async;
using CommonCore.Scripting;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EasterEggHack
{

    private static CancellationTokenSource EasterEggTokenSource;

    [CCScript, CCScriptHook(Hook = ScriptHook.AfterMainMenuCreate)]
    public static void InjectEasterEggHack()
    {
        if (EasterEggTokenSource == null)
            EasterEggTokenSource = new CancellationTokenSource();

        AsyncUtils.RunWithExceptionHandling(() => EasterEggAsync(EasterEggTokenSource.Token));
    }

    [CCScript, CCScriptHook(Hook = ScriptHook.OnSceneUnload)]
    public static void ClearEasterEggHack()
    {
        EasterEggTokenSource?.Cancel();
        EasterEggTokenSource = null;
    }


    public static async Task EasterEggAsync(CancellationToken ct)
    {
        //await Task.Delay(1000 * 10); //10 seconds

        //await Task.Delay(1000 * 60 * 10); //10 minutes

        //await Task.Delay(1000 * 60 * 60); //an hour

        await Task.Delay((int)(1000f * 60f * 4.2f)); //4 minutes and change; the length of the song

        AsyncUtils.ThrowIfEditorStopped();

        if (SceneManager.GetActiveScene().name != "MainMenuScene" || ct.IsCancellationRequested)
            return;

        SharedUtils.ChangeScene("EasterEggScene");
    }


}
