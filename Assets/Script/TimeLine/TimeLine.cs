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
        public bool changeToCam1;

        //public PlayerMovement playerMovement;
        private bool isDead;

        private void Awake()
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
        private void Update()
        {
            //����Ҫ�ĳ������������Ҵ�����������Ȩ�ƶ�
            if (isDead && onCam2 == false)
            {
                vcam.Priority = 8;
                changeToCam1 = false;
                timeLine1.Play();
                Invoke("GetOnCam2", 2.0f);
            }
            if (changeToCam1 && onCam2 == true)//����Ҫ�ĳɾ��鲥��
            {
                vcam.Priority = 10;
                onCam2 = false;
                timeLine2.Play();
            }
        }

        private void GetOnCam2()
        {
            onCam2 = true;
        }

        public void SetIsDead()
        {
            isDead = true;
        }

        public void SetIsDeadFalse()
        {
            isDead = false;
        }
       
    }
}