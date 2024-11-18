using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;//체력을 표시 할 슬라이더
    public AudioClip hitClip;//피격 소리
    public AudioClip itemPickupClip;//아이템 획득시 소리
    public AudioClip dieClip;

    private AudioSource playerAudioSource;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private readonly int hashDie = Animator.StringToHash("dieTrigger");



    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }


    protected override void OnEnable()//오버라이드를 다르게 쓸 예정
    {
        base.OnEnable();
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = currentHealth;
        playerMovement.enabled = true;
        playerShooter.enabled = true;


    }
    //체력 회복
    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        healthSlider.value = currentHealth;
        
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            playerAudioSource.PlayOneShot(hitClip, 1.0f);        
        }
        //LivingEntity의 OnDamage 실행해서 데미지 적용
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = currentHealth;

    }
    
    public override void Die()
    {
        base.Die();
        healthSlider.gameObject.SetActive(false);

        playerAudioSource.PlayOneShot(dieClip,1.0f);
        playerAnimator.SetTrigger(hashDie);
        playerMovement.enabled =false;
        playerShooter.enabled = false;
        Invoke("ReSpawn", 5f);//5초 후 리스폰
    }
    public void ReSpawn()//플레이어가 사망 후 5초 후에 부활
    {
        if (photonView.IsMine)//로컬 플레이어만 직접위치 변경
        {
            //원점에서 반경 5유닛 내부의 랜덤 위치 지정
            Vector3 randomSpawnPoint = Random.insideUnitSphere * 5f;
            randomSpawnPoint.y = 0.3f;
            transform.position = randomSpawnPoint;
            //위치 지정
            Uimanager.Instance.SetActiveGameOverUI(false);
            //Uimanager.Instance.GameRestart();
            GameManager.Instance.isGameOver =false;
        }
        gameObject.SetActive(false);//ondisable 자체처리라 보면 됨
        gameObject.SetActive(true);//onenable 호출
    }
    private void OnTriggerEnter(Collider other)
    {


        //아이템과 충돌한다
        if (!dead)
        {   //충돌한 상대방으로 부터 Iitem 컴퍼넌트를 가지고 온다
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {   //host만 아이템 사용 가능
                //즉 호스트에서 아이템 사용 후 사용된 효과를 모든 클라이언트에게 동기화 시킴
                if (PhotonNetwork.IsMasterClient)
                {   //Use 메서드를 실행하여 아이템 사용
                    item.Use(gameObject);
                
                }
                //아이템 습득 소리 재생
                playerAudioSource.PlayOneShot(itemPickupClip, 1.0f);
                
            }        
        }
    }
}

