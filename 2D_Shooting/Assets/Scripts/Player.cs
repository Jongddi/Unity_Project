using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;

    public float speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;
    public GameObject[] Followers;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public bool isRespawnTime;

    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	void OnEnable()
	{
        Unbeatable();
        Invoke("Unbeatable",3);
	}

    void Unbeatable()
	{
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)                      //무적 타임 이펙트(투명화)
		{
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

            for(int index = 0; index<Followers.Length; index++)
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

    void Update()
    {
        Move();
        Fire();
        Reload();
        Boom();
    }

    public void JoyPanel(int type)
	{
        for(int index=0; index < 9; index++)
		{
            joyControl[index] = index == type;
		}
	}

    public void Joydown()
    {
        isControl = true;
    }

    public void JoyUp()
	{
        isControl = false;
	}

    void Move()
    {
        //Keyboard
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //JoyControl
        if(joyControl[0]){    h = -1;    v = 1;  }
        if(joyControl[1]){    h = 0;    v = 1;  }
        if(joyControl[2]){    h = 1;    v = 1;  }
        if(joyControl[3]){    h = -1;    v = 0;  }
        if(joyControl[4]){    h = 0;    v = 0;  }
        if(joyControl[5]){    h = 1;    v = 0;  }
        if(joyControl[6]){    h = -1;    v = -1;  }
        if(joyControl[7]){    h = 0;    v = -1;  }
        if(joyControl[8]){    h = 1;    v = -1;  }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)               //경계값을 넘지 못하도록 고정
            h = 0;

        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
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
    public void ButtonAUP()
    {
        isButtonA = false;
    }
    public void ButtonBDown()
    {
        isButtonB = true;
    }

    void Fire()
    {
        //if (!Input.GetButton("Fire1"))
        //  return;

        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                // Power one
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
            case 3:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;

                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 4:
                GameObject bulletR2 = objectManager.MakeObj("BulletPlayerA");
                bulletR2.transform.position = transform.position + Vector3.right * 0.35f;

                GameObject bulletC2 = objectManager.MakeObj("BulletPlayerB");
                bulletC2.transform.position = transform.position;

                GameObject bulletL2 = objectManager.MakeObj("BulletPlayerA");
                bulletL2.transform.position = transform.position + Vector3.left * 0.35f;

                Rigidbody2D rigidR2 = bulletR2.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidC2 = bulletC2.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL2 = bulletL2.GetComponent<Rigidbody2D>();

                rigidR2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidC2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletR3 = objectManager.MakeObj("BulletPlayerB");
                bulletR3.transform.position = transform.position + Vector3.right * 0.3f;

                GameObject bulletL3 = objectManager.MakeObj("BulletPlayerB");
                bulletL3.transform.position = transform.position + Vector3.left * 0.3f;

                Rigidbody2D rigidR3 = bulletR3.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL3 = bulletL3.GetComponent<Rigidbody2D>();

                rigidR3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

        }
        curShotDelay = 0;
    }

    void Boom()
    {
        // if (!Input.GetButton("Fire2"))
        //     return;

        if (!isButtonB)
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isButtonB = false;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        //Effect Visiable
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        //Remove Enemy
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
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
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
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy_Bullet")
        {
            if (isRespawnTime)
                return;

            if (isHit)
                return;
            //주석
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life == 0)
            {
                gameManager.gameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);

            //플레이어와 보스가 충돌했을 때 보스가 사라지지 않도록 하는 함수
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
                    gameManager.UpdateBoomIcon(boom);
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower() 
    {
        if (power == 5)
            Followers[0].SetActive(true);
        else if (power == 6)
            Followers[1].SetActive(true);
        else if (power == 7)
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
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}