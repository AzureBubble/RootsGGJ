using System.Collections;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [SerializeField] private bool isEntered;

    public bool hasName;//默认是没有名字的
    [TextArea(1, 5)] public string[] lines;
    [TextArea(1, 4)] public string[] congratsLines;
    [TextArea(1, 4)] public string[] completedLines;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = true;

            DialogueManager.instance.talkable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = false;
            DialogueManager.instance.talkable = null;
        }
    }

    private void Update()
    {
        //这里的限制
        if (Input.GetMouseButtonUp(0) && DialogueManager.instance.dialogueBox.activeInHierarchy == false)
        {   
            DialogueManager.instance.ShowDialogue(lines, hasName);
        }
    }
   
}
