using CommonCore;
using CommonCore.Input;
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

        yield return WaitOrSkip(2f);
        yield return null;

        TargetImage.sprite = Slide1;
        //yield return new WaitForSeconds(10f);
        yield return WaitOrSkip(14f);

        yield return null; //debouncing

        TargetImage.sprite = Slide2;
        //yield return new WaitForSeconds(10f);
        yield return WaitOrSkip(5f);

        yield return new WaitForSeconds(0.1f);

        SharedUtils.ChangeScene("QuattroScene");
    }

    private IEnumerator WaitOrSkip(float time)
    {
        for(float t = 0; t < time; t += Time.deltaTime)
        {
            if(MappedInput.GetButtonDown(CommonCore.Input.DefaultControls.Use) || MappedInput.GetButtonDown(CommonCore.Input.DefaultControls.Fire) || Input.GetKeyDown(KeyCode.Space))
            {
                yield break;
            }

            yield return null;
        }

    }
}
