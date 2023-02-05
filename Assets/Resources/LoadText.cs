using UnityEngine;

public class LoadText : MonoBehaviour
{
    public static LoadText instance;
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
    private void Start()
    {
        TextAsset txt = Resources.Load("questions") as TextAsset;
       // Debug.Log(txt);
        string[] strs = txt.text.Split('\n');
    }
}