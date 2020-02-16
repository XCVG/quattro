using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequence : MonoBehaviour
{

    public Image TargetImage;
    public Sprite Slide1;
    public Sprite Slide2;

    void Start()
    {
        StartCoroutine(CoIntroSequence());
    }

    private IEnumerator CoIntroSequence()
    {
        yield return null;

        yield return new WaitForSeconds(2f);

        TargetImage.sprite = Slide1;
        yield return new WaitForSeconds(10f);

        TargetImage.sprite = Slide2;
        yield return new WaitForSeconds(10f);


        SharedUtils.ChangeScene("QuattroScene");
    }
}
