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
    public bool isBoomTime;
    public bool isRespawnTime;
    public bool isButtonA;

    //Int
    public int life;
    public int score;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;

    //Float
    public float speed;
    public float curShotDelay;
    public float maxShotDelay;

    //Class,GetComponent
    public GameManager gameManager;
    public ObjectManager objectManager;
    Animator anim;
    SpriteRenderer spriteRenderer;

    //Prefab
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;
    public GameObject[] Followers;

    void Awake()        //프로그램 실행 전 다른 클래스에서 오는 컴포넌트 초기화
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)                      //무적 타임 이펙트(투명화)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

            for (int index = 0; index < Followers.Length; index++)
            {
                Followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);                       //무적시간 종료

            for (int index = 0; index < Followers.Length; index++)
            {
                Followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()       //초당 N회 실행
    {
        Move();
        Fire();
        Reload();
        Boom();
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

    public void ButtonADown()
    {
        isButtonA = true;
    }

    void Fire()
	{
        if (curShotDelay < maxShotDelay)
            return;

        switch(power)               //플레이어의 강화단계에 따라 총알 오브젝트의 포지션,로테이션 변동
		{
            case 1:
                GameObject bullet = objectManager.MakeObj("PlayerBulletA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                     //총알 오브젝트의 리지드바디호출
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          //AddForce(힘을받아 날아갈 방향*속도, 힘의 정도) impulse = 순간적인 힘 == 한번 받은 힘에 의해 일정한 속도로 고정
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("PlayerBulletA");
                bulletR.transform.position= transform.position + Vector3.right * 0.1f;
                GameObject bulletL= objectManager.MakeObj("PlayerBulletA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletB = objectManager.MakeObj("PlayerBulletB");
                bulletB.transform.position = transform.position;
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 4:
            case 5:
            case 6:
                GameObject bulletR2 = objectManager.MakeObj("PlayerBulletB");
                GameObject bulletL2 = objectManager.MakeObj("PlayerBulletB");
                bulletR2.transform.position = transform.position + Vector3.right * 0.25f;
                bulletL2.transform.position = transform.position + Vector3.left * 0.25f;
                Rigidbody2D rigidR2 = bulletR2.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL2 = bulletL2.GetComponent<Rigidbody2D>();
                rigidR2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0;
	}

    void Boom()
	{
        // if (!Input.GetButton("Fire2"))
        if (!isButtonA)
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isButtonA = false;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        //필살기 효과발동
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        //필살기발동시 적 제거
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        //Remove Enemy Bullet
        GameObject[] bulletsA = objectManager.GetPool("EnemyBulletA");
        GameObject[] bulletsB = objectManager.GetPool("EnemyBulletB");
        GameObject[] bulletsC = objectManager.GetPool("EnemyBulletC");
        GameObject[] bulletsD = objectManager.GetPool("EnemyBulletD");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
			{
                bulletsA[index].SetActive(false);
            }
        }

        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }
        }

        for (int index = 0; index < bulletsC.Length; index++)
        {
            if (bulletsC[index].activeSelf)
            {
                bulletsC[index].SetActive(false);
            }
        }

        for (int index = 0; index < bulletsD.Length; index++)
        {
            if (bulletsD[index].activeSelf)
            {
                bulletsD[index].SetActive(false);
            }
        }
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
            if (isRespawnTime)
                return;

            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life == 0)
            {
                gameManager.GameOver();
            }
			else
			{
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);

            if (collision.gameObject.tag == "Enemy")
            {
                GameObject bossEnemy = collision.gameObject;
                Enemy enemyBoss = bossEnemy.GetComponent<Enemy>();
                if (enemyBoss.enemyName == "B")
                {
                    return;
                }
                else
                {
                    collision.gameObject.SetActive(false);
                }
            }
            collision.gameObject.SetActive(false);
        }

        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
					else
					{
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                        boom++;
                    gameManager.UpdateBoomIcon(boom);                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 4)
            Followers[0].SetActive(true);
        else if (power == 5)
            Followers[1].SetActive(true);
        else if (power == 6)
            Followers[2].SetActive(true);
    }

    void OffBoomEffect()
	{
        boomEffect.SetActive(false);
        isBoomTime = false;
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