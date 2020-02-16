using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.Audio;

public class EndingSceneMusicStubScript : MonoBehaviour
{

    public string Music;


    void Start()
    {
        AudioPlayer.Instance.SetMusic(Music, MusicSlot.Ambient, 1.0f, true, false);
        AudioPlayer.Instance.StartMusic(MusicSlot.Ambient);
    }

}
