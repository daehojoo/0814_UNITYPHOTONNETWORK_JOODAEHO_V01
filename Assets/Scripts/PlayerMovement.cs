using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotSpeed = 180f;
    private PlayerInPut playerInPut;
    private Rigidbody playerRb;
    private Animator playerAnimator;   
    private Transform playerTr;


    private readonly int hashMove = Animator.StringToHash("move");
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();        
        playerTr = GetComponent<Transform>();
        playerInPut = GetComponent<PlayerInPut>();


    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;


        if (Input.GetKey(KeyCode.S))
        {
            moveSpeed = 5f;
        }
        else
            moveSpeed = 5f;

        Rotate();
        Move();

        playerAnimator.SetFloat(hashMove, playerInPut.move);
        
    }
    void Move()
    {
        Vector3 moveDistance = playerInPut.move * transform.forward * moveSpeed * Time.deltaTime;
        playerRb.MovePosition( playerRb.position + moveDistance);
    }
    void Rotate()
    {
        float turn = playerInPut.rotate * rotSpeed * Time.deltaTime;
        playerRb.rotation = playerRb.rotation * Quaternion.Euler(0f, turn, 0f);
    }
    
    
}
