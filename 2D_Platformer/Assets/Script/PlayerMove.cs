using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update() //단발적 입력
    {
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            //rigid.velocity.nomalized //nomalized 벡터크기를 1로 만든 상태
            //Stop Speed
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
			}
            else //Damaged
                OnDamaged(collision.transform.position);
        }

        else if (collision.gameObject.tag == "Spikes") //가시 충돌이벤트 분리
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnAttack(Transform enemy)
	{
        //Point


        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);       //몬스터 잡을때 반발력(반동)

        //Enemy Die
        Enemy_Move enemyMove = enemy.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();
	}


    //피격 시 무적
    void OnDamaged(Vector2 targetPos)
	{
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
}