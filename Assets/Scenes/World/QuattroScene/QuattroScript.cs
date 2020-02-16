using CommonCore;
using CommonCore.Input;
using CommonCore.RpgGame.Rpg;
using CommonCore.RpgGame.World;
using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuattroScript : MonoBehaviour
{
    public Transform TrackedNpc;

    private bool IsEnding = false;
    private float Elapsed;
    private string NextScene;

    void Start()
    {
        
    }

    void Update()
    {

        if (IsEnding)
        {
            Elapsed += Time.deltaTime;

            if(Elapsed > 5)
            {
                SharedUtils.ChangeScene(NextScene);
            }
        }

        else
        {
            if (!RpgWorldUtils.TargetIsAlive(TrackedNpc))
            {
                Elapsed = 0;
                IsEnding = true;
                NextScene = "BadEndingScene";
                GameState.Instance.PlayerFlags.Add(PlayerFlags.TotallyFrozen);
            }
            else
            {
                if(MappedInput.GetButton(DefaultControls.Reload))
                {
                    Elapsed += Time.deltaTime;
                    

                    if(Elapsed > 3f)
                    {
                        GameState.Instance.PlayerRpgState.UnequipItem(GameState.Instance.PlayerRpgState.Inventory.FindItem("demopistol")[0]);
                        IsEnding = true;
                        Elapsed = 0;
                        NextScene = "GoodEndingScene";
                        GameState.Instance.PlayerFlags.Add(PlayerFlags.TotallyFrozen);
                    }
                }

                if (MappedInput.GetButtonUp(DefaultControls.Reload))
                    Elapsed = 0;


            }
        }
    }
}
