using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;//유니티 용 포톤 컴퍼넌트
using Photon.Realtime;//포톤 서비스 관련라이브러리
using UnityEngine.UI;

//마스터 서버(리슨 서버)와 Mach Making 룸 접속 담당


public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";//게임 버전
    public Text connectionInfoText;//네트워크 정보 표시
    public Button joinButton;



    void Start()
    {   //접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        //설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "마스터 서버에 접속 중.........";
    }
    //마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "온라인 : 마스터 서버와 연결완료...";

    }
    //마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음...";

    }
    public void Connect()//룸 접속 시도 joinBtn을 눌렀을 때 실행
    {
        joinButton.interactable = false;//중복 접속시도를 막기위해
        if (PhotonNetwork.IsConnected)//마스터 서버에 접속 중 이라면
        {
            connectionInfoText.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom();//아무방에나 접속한다.//이것을 랜덤 매치 메이킹이라한다.
        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음...";
            PhotonNetwork.ConnectUsingSettings();//마스터 서버로의 재접속 시도
        }

    }
    //빈 룸이 없어서 랜덤 룸 참가에 실패한 경우 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈방이 없음, 새로운 방 생성...";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 });//방 이름과 방 최다 참가 인원수 설정
        //생성된 룸 목록을 확인 하는 기능은 만들지 않으므로 룸의 이름은
        //입력 하지 않고 null로 입력 했다.
        //참고로 생성된 룸은 리슨 서법 방식으로 동작하며
        //룸을 생성한 클라이언트가 호스트 역활을 맡는다/  호스트 = 마스터클라이언트 = (방장개념)

    }
    //룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel("Main");//모든 룸 참가자가 MainScene을 로드 하게 한다.
    }
}
