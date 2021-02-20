using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;
    public float speed;
    public float health;
    public float maxShotDelay;
    public float curShotDelay;

    public Sprite[] sprites;
    SpriteRenderer spriteRenderer;
    Animator anim;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemBoom;
    public GameObject itemPower;
    public GameObject player;
    public ObjectManager objectManager;

	void Awake()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(enemyName=="B")
		{
            anim = GetComponent<Animator>();
		}
    }

	void OnEnable()
	{
		switch(enemyName)
		{
            case "B":
                health = 3000;
                Invoke("Stop", 3);
                break;

            case "L":
                health = 40;
                break;

            case "M":
                health = 10;
                break;

            case "S":
                health = 5;
                break;
        }
	}

    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 3);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireForward()
    {
        if (health <= 0)
            return;
        //Fire 4 bullet Forward
        GameObject bulletR = objectManager.MakeObj("EnemyBulletC");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletR2 = objectManager.MakeObj("EnemyBulletC");
        bulletR2.transform.position = transform.position + Vector3.right * 0.55f;

        GameObject bulletL = objectManager.MakeObj("EnemyBulletC");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletL2 = objectManager.MakeObj("EnemyBulletC");
        bulletL2.transform.position = transform.position + Vector3.left * 0.55f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidR2 = bulletR2.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL2 = bulletL2.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidR2.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL2.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireForward", 3.5f);
        }
        else
            Invoke("Think", 2);
    }

    void FireShot()
    {
        //Fireshot Bullet
        if (health <= 0)
            return;
        for (int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("EnemyBulletB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        //Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireShot", 3.5f);
        }
        else
            Invoke("Think", 3);
    }

    void FireArc()
    {
        if (health <= 0)
            return;
        //FireArc Bullet
        GameObject bullet = objectManager.MakeObj("EnemyBulletA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Sin(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        //Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireArc", 0.15f);
        }
        else
            Invoke("Think", 3);
    }

    void FireAround()
    {
        if (health <= 0)
            return;
        //Fire Around bullet
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
        for (int index = 0; index < roundNumA; index++)
        {
            GameObject bullet = objectManager.MakeObj("EnemyBulletD");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));

            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        //Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireAround", 0.7f);
        }
        else
            Invoke("Think", 3);
    }


    void Update()       //초당 N회 실행
    {
        if (enemyName == "B")
            return;
        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S")
		{
            GameObject bullet = objectManager.MakeObj("EnemyBulletA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);
        }

        if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("EnemyBulletB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;

            GameObject bulletL = objectManager.MakeObj("EnemyBulletB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
	{
        if(health <= 0)
            return;

        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if (enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }

        if ( health <= 0)
		{
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);

            if (ran < 3)        //30%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6)     //Coin  30%
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 8)       //Power     20%
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)      //Boom      20%
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
	}

    void ReturnSprite()
	{
        spriteRenderer.sprite = sprites[0];
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Border_Bullet" && enemyName != "B")
		{
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if(collision.gameObject.tag == "Player_Bullet")
		{
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
		}
	}
}
