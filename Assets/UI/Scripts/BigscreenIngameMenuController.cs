using CommonCore;
using CommonCore.Config;
using CommonCore.LockPause;
using CommonCore.Scripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BigscreenIngameMenuController : MonoBehaviour
{
    [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterIGUIMenuCreate)]
    public static void InjectBigscreenIngameMenuController()
    {
        if (!ConfigState.Instance.UseBigScreenMode)
            return;

        if (CoreUtils.GetUIRoot() != null && CoreUtils.GetUIRoot().GetComponentInChildren<BigscreenIngameMenuController>() != null)
            return;

        Instantiate(CoreUtils.LoadResource<GameObject>("UI/BigscreenIngameMenu"), CoreUtils.GetUIRoot());
    }

    [SerializeField]
    private GameObject MainPanel = null;

    private void Start()
    {
        MainPanel.SetActive(false); //in case I forget to deactivate it

        EasterEggHack.InjectEasterEggHack(); //because
    }

    private void Update()
    {
        CheckMenuOpen();
    }

    private void CheckMenuOpen()
    {
        //bool menuToggled = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
        bool menuToggled = UnityEngine.Input.GetKeyDown(KeyCode.JoystickButton7);

        if (menuToggled)
        {
            //if we're locked out, let the menu be closed but not opened
            if (!AllowMenu)
            {
                if (MainPanel.activeSelf)
                {
                    MainPanel.SetActive(false);

                    DoUnpause();

                }
            }
            else
            {
                //otherwise, flip state
                bool newState = !MainPanel.activeSelf;
                MainPanel.SetActive(newState);

                //handle pause
                if (newState)
                    DoPause();
                else
                    DoUnpause();
                

                if (newState)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(MainPanel.transform.FindDeepChild("ButtonResume").gameObject);
                    //EventSystem.current.SetSelectedGameObject(MainPanel.transform.FindDeepChild("ButtonResume").gameObject);
                }

            }
        }

    }

    private void DoPause()
    {
        LockPauseModule.PauseGame(PauseLockType.AllowMenu, this);
        LockPauseModule.LockControls(InputLockType.GameOnly, this);
    }

    private void DoUnpause()
    {
        LockPauseModule.UnlockControls(this);
        LockPauseModule.UnpauseGame(this);
    }

    private bool AllowMenu
    {
        get
        {
            var lockState = LockPauseModule.GetInputLockState();
            return (lockState == null || lockState.Value >= InputLockType.GameOnly);
            //TODO allow temporary locking with a session flag or something
        }
    }

    public void HandleResumeButtonClicked()
    {
        //close the menu

        if (MainPanel.activeSelf)
        {
            MainPanel.SetActive(false);

            DoUnpause();

        }
    }

    public void HandleExitButtonClicked()
    {
        //exit the game

        Time.timeScale = ConfigState.Instance.DefaultTimescale; //needed?
        //BaseSceneController.Current.("MainMenuScene");
        SharedUtils.EndGame();
    }


}
