using System.Collections;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [SerializeField] private bool isEntered;

    public bool hasName;//默认是没有名字的
    [TextArea(1, 5)] public string[] lines;

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        isEntered = true;

    //        DialogueManager.instance.talkable = this;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        isEntered = false;
    //        DialogueManager.instance.talkable = null;
    //    }
    //}

    private void Update()
    {
        //这里的限制
        //if (DialogueManager.instance.isTalking == true)
        //{   
        //    DialogueManager.instance.ShowDialogue(lines, hasName);
        //}
    }
   
}
