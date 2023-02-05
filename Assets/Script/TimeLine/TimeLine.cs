using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace Cinemachine.Examples
{ 

    public class TimeLine : MonoBehaviour
    {
        public static TimeLine instance;
        public PlayableDirector timeLine1;
        public PlayableDirector timeLine2;
        public CinemachineVirtualCameraBase vcam;
        public bool onCam2;
        void Awake()
        { 
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
        // Update is called once per frame
        void Update()
        {
            //����Ҫ�ĳ������������Ҵ�����������Ȩ�ƶ�
            if(Input.GetKeyDown(KeyCode.Space)&&onCam2 ==false)
            {
                vcam.Priority = 8;
                timeLine1.Play();
                onCam2 = true;
            }
            if (Input.GetKeyDown(KeyCode.K)&&onCam2 ==true)//����Ҫ�ĳɾ��鲥��
            {
                vcam.Priority = 10;
                onCam2 = false;
                timeLine2.Play();
            }
        }
    }
}
