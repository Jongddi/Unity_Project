﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() //단발적 입력
    {
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);

            //Sound
            PlaySound("JUMP");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            //rigid.velocity.nomalized //nomalized 벡터크기를 1로 만든 상태
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);

        else
            anim.SetBool("isWalking", true);

    }

    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //오른쪽 최대속도 조절
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxSpeed * (-1)) //왼쪽 최대속도 조절
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }
        
        
        //Landing Platform
        //RayCst 오브젝트 검색을 위해 Ray를 쏘는 방식
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //에디터 상에서만 Ray를 그려주는 함수

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("PlatForm")); //Ray에 닿은오브젝트

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
                // Debug.Log(rayHit.collider.name);
            }
        }
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "Enemy")
		{
            //Debug.Log("플레이어가 맞았습니다!");

            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);

                //Sound
                PlaySound("ATTACK");
            }
            else //Damaged
            {
                OnDamaged(collision.transform.position);

                //Sound
                PlaySound("DAMAGED");
            }
        }

        else if (collision.gameObject.tag == "Spikes") //가시 충돌이벤트 분리
        {
            OnDamaged(collision.transform.position);

            //Sound
            PlaySound("DAMAGED");
        }
    }

	 void OnTriggerEnter2D(Collider2D collision) //동전, 피니쉬라인
	{
        if (collision.gameObject.tag == "Item")                     //동전
		{
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
			{
                gameManager.stagePoint += 50;
            }
            else if(isSilver)
			{
                gameManager.stagePoint += 100;
            }
            else if(isGold)
			{
                gameManager.stagePoint += 300;
            }
            

            // Deactive Item
            collision.gameObject.SetActive(false);

            //Sound
            PlaySound("ITEM");
        }

        else if(collision.gameObject.tag == "Finish")
		{
            // Next Stage
            gameManager.NextStage();

            //Sound
            PlaySound("FINISH");
        }
	}

	void OnAttack(Transform enemy)
	{
        //Point
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);       //몬스터 잡을때 반발력(반동)

        //Enemy Die
        Enemy_Move enemyMove = enemy.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();
	}


    //피격 시 무적
    void OnDamaged(Vector2 targetPos)
	{
        // Health Down
        gameManager.HealthDown();

        //Chage layer (Immortal Active)
        gameObject.layer = 11;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);             //무적시간 설정
	}

    //무적해제
    void OffDamaged()
	{
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
	}

    //사망 이펙트
   public void OnDie()
	{
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        capsuleCollider.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Sounds
        PlaySound("DAMAGED");
    }

    //원위치
    public void VelocityZero()
	{
        rigid.velocity = Vector2.zero;
	}

    //Sound Effect
    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }
}