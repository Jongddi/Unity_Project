using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        Think();

        Invoke("Think", 5);      //5초뒤에 실행
    }

    void FixedUpdate() //1초에 50-60번 움직임
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //PlatForm Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); //에디터 상에서만 Ray를 그려주는 함수

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("PlatForm")); //Ray에 닿은오브젝트

        if (rayHit.collider == null)
        {
            //Debug.Log("경고! 이 앞 낭떠러지.");
            Turn();
        }
    }

    //재귀함수 : 자기자신을 또 호출하는 함수, 보통 맨 아래에 작성
    void Think()
    {
        //set next active
        nextMove = Random.Range(-1, 2); //최댓값은 포함이 안되므로 원하는 범위 수보다 +1해야함

        //sprite animation
        anim.SetInteger("WalkSpeed", nextMove);

        //flip sprite
        if (nextMove != 0) {
            spriteRenderer.flipX = nextMove == 1;
        }

        //recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);         //5초뒤에 호출
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        //Collider Disable
        capsuleCollider.enabled = false;

        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
	{
        gameObject.SetActive(false);
	}
}
