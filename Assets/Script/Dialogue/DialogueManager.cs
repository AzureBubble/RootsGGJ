using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine.Examples;
using Cinemachine;
using System;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox;//显示或隐藏panel
    public Text[] dialogueText;
    public int dialogueIndex;
    public Text nameText;

    //保证句子不显示在一行
    [TextArea(1, 3)] public string[] dialogueLines;

    [SerializeField] private int currentLine;

    private bool isScrolling;//禁止没有滚完玩家就跳对话
    [SerializeField] private float scrollingSpeed;

    public Talkable talkable;//采用获取脚本的方法访问变量
    public Talkable[] talkableEnum;

    //设计成单例模式挂在Dialoguepanel下
    public int talkabIndex = 0;

    public bool isTalking = false;
    public TimeLine timeLine;

    public GameObject playerAPrefab, playerBPrefab, playerCPrefab, playerDPrefab;
    public GameObject swordPrefab, jumpPrefab;
    public Transform playerTranform;
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public PlayerMovement player;

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
        //playerTranform = GameObject.Find("PlayerA").transform;
    }

    //打开对话窗口，文字滚动
    public void ShowDialogue(string[] _newLines, bool _hasName)
    {
        dialogueLines = _newLines;
        currentLine = 0;

        CheckName();
        dialogueText[1 - dialogueIndex].enabled = false;
        dialogueText[dialogueIndex].enabled = true;
        SetTextAlign(_hasName);
        AudioManager.instance.PlaySound2D("dialogue");
        StartCoroutine(ScrollingText(dialogueIndex));

        dialogueBox.SetActive(true);
        //因为单例模式如果没名字会沿用上一次调用的名字
        nameText.gameObject.SetActive(_hasName);

        // PlayerMovement.instance.isTalking = true; //todo
    }

    private void Update()
    {
        if (GameObject.FindObjectOfType<PlayerMovement>() != null)
        {
            player = GameObject.FindObjectOfType<PlayerMovement>();
        }
        if (timeLine.onCam2 == true && dialogueBox.activeInHierarchy == false && (!isTalking))
        {
            talkable = talkableEnum[talkabIndex];
            ShowDialogue(talkable.lines, talkable.hasName);
            talkabIndex++;
            isTalking = true;
        }
        if (dialogueBox.activeInHierarchy)//只在激活panel时检测按下左键
        {
            //按下并松开左键
            if (Input.GetMouseButtonUp(0) && !isScrolling)//Input.GetMouseButtonUp(0)
            {
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    AudioManager.instance.PlaySound2D("dialogue");
                    CheckName();
                    dialogueText[1 - dialogueIndex].enabled = false;
                    dialogueText[dialogueIndex].enabled = true;
                    StartCoroutine(ScrollingText(dialogueIndex));//Letter by Letter Show
                }
                else
                {
                    dialogueBox.SetActive(false);//Box HIDE MARKER END Dialogue
                    isTalking = false;
                    timeLine.changeToCam1 = true;
                    timeLine.SetIsDeadFalse();
                    Create();

                    //PlayerMovement.instance.isTalking = false;
                    //if(talkabIndex >=2&& player.bossIsDead == true)
                    if (talkabIndex >= 2 && player.bossIsDead == true)
                    {
                        Invoke("Change", 4.0f);
                    }
                    else if(talkabIndex >= 2){
                        talkabIndex = 2;
                    }
                }
            }
        }
    }

    private void Create()
    {
        switch (talkabIndex)
        {
            case 1:
                //GameObject player1 = Instantiate(playerAPrefab, playerTranform.position, Quaternion.identity);
                playerCPrefab.SetActive(true);
                swordPrefab.SetActive(true);
                vcam1.Follow = playerCPrefab.transform;
                vcam2.Follow = playerCPrefab.transform;
                break;

            case 2:
                GameObject player2 = Instantiate(playerBPrefab, playerTranform.position, Quaternion.identity);
                jumpPrefab.SetActive(true);
                player2.gameObject.GetComponent<PlayerMovement>().SetJump();
                vcam1.Follow = player2.transform;
                vcam2.Follow = player2.transform;
                break;

            case 3:
                GameObject player3 = Instantiate(playerBPrefab, playerTranform.position, Quaternion.identity);
                player3.gameObject.GetComponent<PlayerMovement>().SetJump();
                player3.gameObject.GetComponent<PlayerMovement>().maxHealth = 10000;
                vcam1.Follow = player3.transform;
                vcam2.Follow = player3.transform;
                break;
        }
    }

    //// 可对话的NPC对话【居左对齐】工具类游戏对象，比如路标【居中对齐】
    private void SetTextAlign(bool _hasName)
    {
        if (_hasName)
        {
            dialogueIndex = 1;
            dialogueText[dialogueIndex].alignment = (UnityEngine.TextAnchor)TextAlignment.Left;
        }
        else
        {
        }
    }

    //检查对话内容是否含有对话者的名字
    private void CheckName()
    {
        if (dialogueLines[currentLine].StartsWith("n-"))
        {
            dialogueLines[currentLine] = dialogueLines[currentLine].Replace("n-", "");//在NameText处显示名字，并且去除标记n-
            dialogueIndex = 1;
            //currentLine++;//跳过显示名字的这一行
        }
        else
        {
            dialogueIndex = 0;
        }
    }

    //字母滚动效果
    private IEnumerator ScrollingText(int index)
    {
        isScrolling = true;
        dialogueText[index].text = "";//清空

        foreach (char letter in dialogueLines[currentLine].ToCharArray())
        {
            dialogueText[index].text += letter;//HELLO => H->E->L->L->O//MARKER Letter by Letter Show
            yield return new WaitForSeconds(scrollingSpeed);
        }

        isScrolling = false;
    }

    public void SetPlayer(GameObject gameObject)
    {
        playerDPrefab = gameObject;
    }
    private void change()
    {
        SceneManager.LoadScene("end", LoadSceneMode.Single); ;
    }
}