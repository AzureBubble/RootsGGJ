using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSentence : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject talkIcon;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            talkIcon.SetActive(true);//开启的时候其实透明度还是等于0
            StartCoroutine(FadeIn());//渐变效果的调用
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            //StartCoroutine(FadeOut());//ERROR
            talkIcon.GetComponent<CanvasGroup>().alpha = 0;
            talkIcon.SetActive(false);

        }
    }
    IEnumerator FadeIn()
    {
        talkIcon.GetComponent<CanvasGroup>().alpha = 0;
        while (talkIcon.GetComponent<CanvasGroup>().alpha < 1)
        {
            talkIcon.GetComponent<CanvasGroup>().alpha += 0.02f;
            yield return null;
        }
    }
}
