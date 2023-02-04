using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace Cinemachine.Examples
{ 

    public class TimeLine : MonoBehaviour
    {
    public PlayableDirector timeLine1;
    public PlayableDirector timeLine2;
    public CinemachineVirtualCameraBase vcam;
    // Update is called once per frame
    void Update()
    {
        //这里要改成死亡触发并且触发后人物无权移动
        if(Input.GetKeyDown(KeyCode.Space))
        {
            vcam.Priority = 8;
            timeLine1.Play();
        }
        if (Input.GetKeyDown(KeyCode.K))//这里要改成剧情播完
        {
            vcam.Priority = 10;
            timeLine2.Play();
        }
    }
    }
}
