using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BigscreenMainMenuController : MonoBehaviour
{

    [Header("Panel")]
    public GameObject CurrentPanel;
    public GameObject MessagePanel;
    public GameObject HelpPanel;

    [Header("Special")]    
    public Text SystemText;


    private void Start()
    {
        if (CoreParams.IsDebug)
        {
            SystemText.text = string.Format("{0}\n{1} {2}\nCommonCore {3} {4}\nUnity {5}",
                    Application.productName,
                    Application.version, CoreParams.GameVersionName,
                    CoreParams.VersionCode.ToString(), CoreParams.VersionName,
                    Application.unityVersion);
        }
        else
        {
            SystemText.gameObject.SetActive(false);
        }

    }

    public void OnClickNew()
    {
        if (CurrentPanel != null)
            CurrentPanel.SetActive(false);

        CurrentPanel = MessagePanel;

        CurrentPanel.SetActive(true);

        //select the continue/cancel buttons
        EventSystem.current.SetSelectedGameObject(CurrentPanel.transform.FindDeepChild("Button (1)").gameObject);
    }

    public void OnClickModalContinue()
    {
        StartGame();
    }

    public void OnClickModalCancel()
    {
        if(CurrentPanel == MessagePanel)
        {
            CurrentPanel.SetActive(false);
            CurrentPanel = null;
        }

        EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonNew"));
    }

    public void StartGame()
    {
        //start a new game the normal way
        SharedUtils.StartGame();
    }

    public void OnClickHelp()
    {
        if (CurrentPanel != null)
            CurrentPanel.SetActive(false);

        CurrentPanel = HelpPanel;

        CurrentPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(CurrentPanel.transform.FindDeepChild("Button").gameObject);
    }

    public void OnClickHelpClose()
    {
        if (CurrentPanel == HelpPanel)
        {
            CurrentPanel.SetActive(false);
            CurrentPanel = null;
        }

        EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonHelp"));
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

}
