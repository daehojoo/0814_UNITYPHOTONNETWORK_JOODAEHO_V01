using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//IK : 왼손 오른손이 무기에 정확하게 부착 되게
//총알 발사
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;//총 배치 기준점
    public Transform leftHandMound;//총의 왼쪽 손잡이
    public Transform rightHandMound;//총의 오른쪽 손잡이
    private PlayerInPut playerInPut;
    private Animator animator;

    [Header("Ui관련")]
    public Text ammoTxt;
    public Text remainAmmoTxt;


    private void OnEnable()
    {//슈터가 활성화 될 때, 총도 함께 활성화
        gun.gameObject.SetActive(true);


    }
    void Start()
    {
        
        animator = GetComponent<Animator>();
        playerInPut = GetComponent<PlayerInPut>();
        

    }
    void Update()
    {
        if (!photonView.IsMine) return;

        if (playerInPut.fire)
        {
            gun.Fire();
        }
        else if (playerInPut.reload)
        {
            if (gun.Reload())
            {
                animator.SetTrigger("reloadTrigger");
            }
        }
        UpdateUI();
    }

    void UpdateUI()//탄알 표시 UI 업데이트
    {
        if (gun != null && Uimanager.Instance != null)
        {
            //ammoTxt.text = $"{gun.magAmmo}/{gun.ammoRemain}";
            Uimanager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);


        }
    
    }
    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);//총의 기준점 gunpivot을 3D모델의 오른쪽 팔꿈치 위치로 이동
        //IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);//가중치
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);
        //IK를 사용하여 왼손의 위치와 회전을 총의 오른 손잡이에 맞춤
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);
    }
    private void OnDisable()
    {
        gun.gameObject.SetActive(false);

    }
}
