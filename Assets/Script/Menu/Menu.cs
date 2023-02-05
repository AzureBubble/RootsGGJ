using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Sprite[] needImage;
    public Image nowImage;
    [Header("故事讲述进场")]
    public GameObject dialogueBox;//显示或隐藏panel
    public Text dialogueText;
    //保证句子不显示在一行
    [TextArea(1, 3)] public string[] dialogueLines;
    [SerializeField] private int currentLine;

    private bool isScrolling;//禁止没有滚完玩家就跳对话
    [SerializeField] private float scrollingSpeed;

    void Start()
    {

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

    }
    private void Update()
    {
        if (dialogueBox.activeInHierarchy)//只在激活panel时检测按下左键
        {
           
            //按下并松开左键
            if (Input.GetKeyDown(KeyCode.Space) && !isScrolling)
            {
                AudioManager.instance.PlaySound2D("ButtonTip");
                currentLine++;

                if (currentLine < dialogueLines.Length)
                {
                    nowImage.sprite = needImage[currentLine];
                    StartCoroutine(ScrollingText());//Letter by Letter Show
                }
                else
                {
                    SceneManager.LoadScene("sktest_urp");
                }
            }
        }
    }

    public void Play()
    {
        //这里转到一段剧情
        mainMenuHolder.SetActive(false);
        showStoryLine();
        
    }
    public void showStoryLine()
    {
         dialogueBox.SetActive(true);
        nowImage.sprite = needImage[currentLine];
        StartCoroutine(ScrollingText());//直接放在update会报错，why

    }
    public void Quit()
    {
        Application.Quit();
    }


    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }



    public void SetMasterVolume()//(float value)
    {
        // Debug.Log(volumeSliders[0].value);
        AudioManager.instance.SetVolume(volumeSliders[0].value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume()
    {
        AudioManager.instance.SetVolume(volumeSliders[1].value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume()
    {
        AudioManager.instance.SetVolume(volumeSliders[2].value, AudioManager.AudioChannel.Sfx);
    }
    //字母滚动效果
    private IEnumerator ScrollingText()
    {
        isScrolling = true;
        dialogueText.text = "";//清空

        foreach (char letter in dialogueLines[currentLine].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(scrollingSpeed);//一个字一个字显示
        }

        isScrolling = false;
    }

}