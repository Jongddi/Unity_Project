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

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
        Boom();
    }

    void Move()
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
        if (!Input.GetButton("Fire1"))
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
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;

                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
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
            case 4:
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
        if (!Input.GetButton("Fire2"))
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        //Effect Visiable
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        //Remove Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int index = 0; index < enemies.Length; index++)
        {
            Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }

        //Remove Enemy Bullet
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Enemy_Bullet");
        for (int index = 0; index < bullets.Length; index++)
        {
            bullets[index].SetActive(false);
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
            if (isHit)
                return;
            //주석
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);

            if (life == 0)
            {
                gameManager.gameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);
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
                        power++;
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