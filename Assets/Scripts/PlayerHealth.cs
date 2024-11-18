using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider;//ü���� ǥ�� �� �����̴�
    public AudioClip hitClip;//�ǰ� �Ҹ�
    public AudioClip itemPickupClip;//������ ȹ��� �Ҹ�
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


    protected override void OnEnable()//�������̵带 �ٸ��� �� ����
    {
        base.OnEnable();
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = currentHealth;
        playerMovement.enabled = true;
        playerShooter.enabled = true;


    }
    //ü�� ȸ��
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
        //LivingEntity�� OnDamage �����ؼ� ������ ����
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
        Invoke("ReSpawn", 5f);//5�� �� ������
    }
    public void ReSpawn()//�÷��̾ ��� �� 5�� �Ŀ� ��Ȱ
    {
        if (photonView.IsMine)//���� �÷��̾ ������ġ ����
        {
            //�������� �ݰ� 5���� ������ ���� ��ġ ����
            Vector3 randomSpawnPoint = Random.insideUnitSphere * 5f;
            randomSpawnPoint.y = 0.3f;
            transform.position = randomSpawnPoint;
            //��ġ ����
            Uimanager.Instance.SetActiveGameOverUI(false);
            //Uimanager.Instance.GameRestart();
            GameManager.Instance.isGameOver =false;
        }
        gameObject.SetActive(false);//ondisable ��üó���� ���� ��
        gameObject.SetActive(true);//onenable ȣ��
    }
    private void OnTriggerEnter(Collider other)
    {


        //�����۰� �浹�Ѵ�
        if (!dead)
        {   //�浹�� �������� ���� Iitem ���۳�Ʈ�� ������ �´�
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {   //host�� ������ ��� ����
                //�� ȣ��Ʈ���� ������ ��� �� ���� ȿ���� ��� Ŭ���̾�Ʈ���� ����ȭ ��Ŵ
                if (PhotonNetwork.IsMasterClient)
                {   //Use �޼��带 �����Ͽ� ������ ���
                    item.Use(gameObject);
                
                }
                //������ ���� �Ҹ� ���
                playerAudioSource.PlayOneShot(itemPickupClip, 1.0f);
                
            }        
        }
    }
}

