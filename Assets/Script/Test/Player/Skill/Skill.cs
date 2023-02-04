using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Dictionary<string, bool> skillDict = new Dictionary<string, bool>();

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GetSkill()
    {
    }

    public void AddSkill(string name)
    {
        if (!skillDict.ContainsKey(name))
        {
            skillDict.Add(name, true);
        }
    }
}