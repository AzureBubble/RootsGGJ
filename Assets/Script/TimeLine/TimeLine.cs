using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class TimeLine : MonoBehaviour
{
    public PlayableDirector timeLine1;
    public PlayableDirector timeLine2;

    // Update is called once per frame
    void Update()
    {
        //����Ҫ�ĳ������������Ҵ�����������Ȩ�ƶ�
        if(Input.GetKeyDown(KeyCode.Space))
        {
            timeLine1.Play();
            
        }
        if (Input.GetKeyDown(KeyCode.K))//����Ҫ�ĳɾ��鲥��
        {
            timeLine2.Play();
        }
    }
}
