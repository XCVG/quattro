using CommonCore;
using CommonCore.LockPause;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneExitScript : MonoBehaviour
{

    [SerializeField]
    private GameObject ContinueHintTextObject = null;


    private float HoldTime = 0;

    private void Update()
    {
        if (LockPauseModule.IsInputLocked())
            return;

        if(Input.GetButtonDown("Submit"))
        {
            ContinueHintTextObject.Ref()?.SetActive(true);
        }

        if(Input.GetButton("Submit"))
        {
            HoldTime += Time.deltaTime;
        }
        else
        {
            HoldTime = 0;
        }

        if(HoldTime > 3f)
        {
            SharedUtils.EndGame();
        }
    }
}
