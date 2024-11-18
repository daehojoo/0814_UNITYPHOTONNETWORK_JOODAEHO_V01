using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : LivingEntity
{

    public LivingEntity targetEntity;//추적 대상
    private NavMeshAgent pathFinder;//경로 계산 AI 에이전트

    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    public AudioClip deadSound;
    private AudioSource enemyAudioSource;
    private Animator enemyAnimator;
    private Renderer enemyRenderer;
    private Rigidbody enemyRg;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;//공격 주기
    private float lastAttackTime;//마지막 공격 시점
    private int whatIsTarget;
    private bool hasTarget
    {
        get
        {   //추적할 대상이 존재 하고 대상이 사망 하지 않았다면
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }
    private readonly int hashHasTarget = Animator.StringToHash("hasTarget");
    private readonly int hashDie = Animator.StringToHash("dieTrigger");
    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyRenderer = gameObject.GetComponentInChildren<Renderer>();
        enemyAudioSource = GetComponent<AudioSource>();
        whatIsTarget = LayerMask.NameToLayer("Player");
        enemyRg = GetComponent<Rigidbody>();
        
        
    }
    [PunRPC]
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {

        startingHealth = newHealth;
        currentHealth = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;


    }
   
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //StartCoroutine(UpdatePath());
        InvokeRepeating("UpdatePath", 0.01f, 0.25f);
        //게임오브젝트 활성화와 동시에 AI 추적 루틴이 시작
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        enemyAnimator.SetBool(hashHasTarget, hasTarget);


    }
    void UpdatePath()
    {
        if (!dead)
        {
            if (hasTarget)
            {
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                pathFinder.isStopped = true;
                // 플레이어를 찾고 targetEntity를 설정
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    LivingEntity playerEntity = playerObject.GetComponent<LivingEntity>();
                    if (playerEntity != null && !playerEntity.dead)
                    {
                        targetEntity = playerEntity;
                    }
                }
                else
                {
                    // 플레이어가 씬에 없을 경우
                    Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                        if (livingEntity != null && !livingEntity.dead)
                        {
                            targetEntity = livingEntity;
                            break;
                        }
                    }
                }
                // yield return new WaitForSeconds(0.25f);
            }
        }
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            //hitEffect.transform.position = hitPoint;
            //hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(hitNormal));
            hitEffect.Play();
            
            enemyAudioSource.PlayOneShot(hitSound, 1.0f);
           // Debug.Log(hitPoint);
            //Debug.Log(hitNormal);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void Die()
    {
        base.Die();
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;            
        }
        enemyRg.isKinematic = true;
        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        enemyAudioSource.PlayOneShot(deadSound);
        enemyAnimator.SetTrigger(hashDie);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;


        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null && attackTarget == targetEntity)
            {
                //상대방의 LivingEntity가 자신의 추적 대상이라면 공격 실행
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                //상대방의 피격위치와 피격 방향을 근사값으로 계산
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
