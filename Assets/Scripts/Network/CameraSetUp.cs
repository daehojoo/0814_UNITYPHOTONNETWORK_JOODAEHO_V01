using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  //포톤뷰와 시네머신 가상카메라와 동기화 하기 위해

public class CameraSetUp : MonoBehaviourPun//MonoBehaviour 기능에서 포톤 뷰를 추가한 프로퍼티이다.
{       


    void Start()
    {
        //만약 자신이 로컬 플레이어라면 ==
        if (photonView.IsMine)//포톤 네트워크 상의 포톤 뷰가 자기자신의 것이라면
        {
            //씬에 있는 시네머신 가상카메라를 찾고
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            //가상카메라의 추적대상을 자신의 트랜스 폼으로 변경
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }
}
