using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip mainTheme;
    public AudioClip shopTheme;
    public AudioClip menuTheme;
    public AudioClip happyTheme;

    string sceneName;
    public static MusicManager instance;
    Scene sscene;
    void Awake()
    {
        sscene = SceneManager.GetActiveScene();
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {

        OnSceneLoaded(sscene, LoadSceneMode.Single);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = SceneManager.GetActiveScene().name;

        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }
    /*
    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;

        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }
    */
    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu1")
        {
            clipToPlay = menuTheme;
        }
        else if(sceneName == "ShopScene")
        {
            clipToPlay = shopTheme;
        }
        else  if (sceneName == "sktest_urp")
        {
            clipToPlay = mainTheme;
        }
        else
        {
            clipToPlay = happyTheme;
        }
        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }

    }

}