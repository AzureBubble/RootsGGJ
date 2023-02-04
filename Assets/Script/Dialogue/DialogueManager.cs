using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox;//显示或隐藏panel
    public Text dialogueText, nameText;
    //保证句子不显示在一行
    [TextArea(1, 3)] public string[] dialogueLines;
    [SerializeField] private int currentLine;

    private bool isScrolling;//禁止没有滚完玩家就跳对话
    [SerializeField] private float scrollingSpeed;

    public Talkable talkable;//采用获取脚本的方法访问变量
    //设计成单例模式挂在Dialoguepanel下
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
    }

    //打开对话窗口，文字滚动
    public void ShowDialogue(string[] _newLines, bool _hasName)
    {
        dialogueLines = _newLines;
        currentLine = 0;

        CheckName();
        SetTextAlign(_hasName);

        StartCoroutine(ScrollingText());

        dialogueBox.SetActive(true);
        //因为单例模式如果没名字会沿用上一次调用的名字
        nameText.gameObject.SetActive(_hasName);

       // PlayerMovement.instance.isTalking = true; //todo
    }

    private void Update()
    {
        if (dialogueBox.activeInHierarchy)//只在激活panel时检测按下左键
        {
            //按下并松开左键
            if (Input.GetMouseButtonUp(0) && !isScrolling)//Input.GetMouseButtonUp(0)
            {
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    CheckName();
                    StartCoroutine(ScrollingText());//Letter by Letter Show
                }
            }
        }
    }

    //// 可对话的NPC对话【居左对齐】工具类游戏对象，比如路标【居中对齐】
    private void SetTextAlign(bool _hasName)
    {
        if (_hasName)
            dialogueText.alignment = (UnityEngine.TextAnchor)TextAlignment.Left;
        else
            dialogueText.alignment = (UnityEngine.TextAnchor)TextAlignment.Center;
    }

    //检查对话内容是否含有对话者的名字
    private void CheckName()
    {
        if (dialogueLines[currentLine].StartsWith("n-"))
        {
            nameText.text = dialogueLines[currentLine].Replace("n-", "");//在NameText处显示名字，并且去除标记n-
            currentLine++;//跳过显示名字的这一行
        }
    }

    //字母滚动效果
    private IEnumerator ScrollingText()
    {
        isScrolling = true;
        dialogueText.text = "";//清空

        foreach (char letter in dialogueLines[currentLine].ToCharArray())
        {
            dialogueText.text += letter;//HELLO => H->E->L->L->O//MARKER Letter by Letter Show
            yield return new WaitForSeconds(scrollingSpeed);
        }

        isScrolling = false;
    }

}
