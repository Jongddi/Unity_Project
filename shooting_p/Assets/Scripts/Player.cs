using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Bool
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
    public bool isHit;

    //Int
    public int life;
    public int score;

    //Float
    public float speed;
    public float power;
    public float curShotDelay;
    public float maxShotDelay;

    //Class,GetComponent
    public GameManager gameManager;
    Animator anim;

    //Prefab
    public GameObject bulletObjA;
    public GameObject bulletObjB;

    void Awake()        //프로그램 실행 전 다른 클래스에서 오는 컴포넌트 초기화
    {
        anim = GetComponent<Animator>();
    }

    void Update()       //초당 N회 실행
    {
        Move();
        Fire();
        Reload();
    }

    void Move()             //플레이어 이동 함수
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))               //경계값을 넘지 못하도록 고정
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;            //transform이동은 항상 델타타임을 곱해야함

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
	{
        if (curShotDelay < maxShotDelay)
            return;

        switch(power)               //플레이어의 강화단계에 따라 총알 오브젝트의 포지션,로테이션 변동
		{
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);                //Instantiate(복제할 오브젝트 ,생성될 위치 ,생성될 방향)
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                     //총알 오브젝트의 리지드바디호출
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          //AddForce(힘을받아 날아갈 방향*속도, 힘의 정도) impulse = 순간적인 힘 == 한번 받은 힘에 의해 일정한 속도로 고정
                break;
            case 2:
                GameObject bulletR= Instantiate(bulletObjA, transform.position+Vector3.right * 0.1f, transform.rotation);
                GameObject bulletL= Instantiate(bulletObjA, transform.position+ Vector3.left * 0.1f, transform.rotation);
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletB = Instantiate(bulletObjB, transform.position, transform.rotation);
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0;
	}

    void Reload()
	{
        curShotDelay += Time.deltaTime;
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "B_Top":
                    isTouchTop = true;
                    break;
                case "B_Bottom":
                    isTouchBottom = true;
                    break;
                case "B_Right":
                    isTouchRight = true;
                    break;
                case "B_Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);

            if (life == 0)
            {
                gameManager.GameOver();
            }
			else
			{
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "B_Top":
                    isTouchTop = false;
                    break;
                case "B_Bottom":
                    isTouchBottom = false;
                    break;
                case "B_Right":
                    isTouchRight = false;
                    break;
                case "B_Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}