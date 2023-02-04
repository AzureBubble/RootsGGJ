using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Dictionary<string, bool> skillDict = new Dictionary<string, bool>();

    private static Skill instance;   // µ¥Àý

    public Skill Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
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