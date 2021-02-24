using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	//Prefab
	public GameObject enemyLPrefab;
	public GameObject enemyMPrefab;
	public GameObject enemySPrefab;
	public GameObject enemyBPrefab;
	public GameObject enemyBulletAPrefab;
	public GameObject enemyBulletBPrefab;
	public GameObject enemyBulletCPrefab;
	public GameObject enemyBulletDPrefab;
	public GameObject playerBulletAPrefab;
	public GameObject playerBulletBPrefab;
	public GameObject itemCoinPrefab;
	public GameObject itemPowerPrefab;
	public GameObject itemBoomPrefab;
	public GameObject followerBulletPrefab;
	public GameObject explosionPrefab;

	//Array
	GameObject[] enemyL;		//Enemy
	GameObject[] enemyM;
	GameObject[] enemyS;
	GameObject[] enemyB;

	GameObject[] itemCoin;		//Item
	GameObject[] itemPower;
	GameObject[] itemBoom;

	GameObject[] playerBulletA;		//Player
	GameObject[] playerBulletB;
	GameObject[] enemyBulletA;
	GameObject[] enemyBulletB;
	GameObject[] enemyBulletC;
	GameObject[] enemyBulletD;
	GameObject[] followerBullet;
	GameObject[] explosion;

	GameObject[] targetPool;        //Pooling


	void Awake()
	{
		enemyL = new GameObject[10];
		enemyM = new GameObject[10];
		enemyS = new GameObject[10];
		enemyB = new GameObject[1];

		itemCoin = new GameObject[20];
		itemPower = new GameObject[10];
		itemBoom = new GameObject[10];

		playerBulletA = new GameObject[100];
		playerBulletB = new GameObject[100];
		enemyBulletA = new GameObject[100];
		enemyBulletB = new GameObject[100];
		enemyBulletC = new GameObject[50];
		enemyBulletD = new GameObject[1000];
		followerBullet = new GameObject[100];
		explosion = new GameObject[20];

		Generate();
	}

	void Generate()
	{
		//Enemy
		for(int index = 0; index < enemyL.Length; index++)
		{
			enemyL[index] = Instantiate(enemyLPrefab);              //Instantiate(복제할 오브젝트 ,생성될 위치 ,생성될 방향)
			enemyL[index].SetActive(false);
		}

		for (int index = 0; index < enemyM.Length; index++)
		{
			enemyM[index] = Instantiate(enemyMPrefab);
			enemyM[index].SetActive(false);
		}

		for (int index = 0; index < enemyS.Length; index++)
		{
			enemyS[index] = Instantiate(enemySPrefab);
			enemyS[index].SetActive(false);
		}
		for (int index = 0; index < enemyB.Length; index++)
		{
			enemyB[index] = Instantiate(enemyBPrefab);
			enemyB[index].SetActive(false);
		}

		//Item
		for (int index = 0; index < itemCoin.Length; index++)
		{
			itemCoin[index] = Instantiate(itemCoinPrefab);
			itemCoin[index].SetActive(false);
		}

		for(int index = 0; index < itemPower.Length; index++)
		{
			itemPower[index] = Instantiate(itemPowerPrefab);
			itemPower[index].SetActive(false);
		}

		for(int index = 0; index < itemBoom.Length; index++)
		{
			itemBoom[index] = Instantiate(itemBoomPrefab);
			itemBoom[index].SetActive(false);
		}

		//Bullet
		for(int index = 0; index<playerBulletA.Length; index++)
		{
			playerBulletA[index] = Instantiate(playerBulletAPrefab);
			playerBulletA[index].SetActive(false);
		}

		for (int index = 0; index < playerBulletB.Length; index++)
		{
			playerBulletB[index] = Instantiate(playerBulletBPrefab);
			playerBulletB[index].SetActive(false);
		}

		for (int index = 0; index < enemyBulletA.Length; index++)
		{
			enemyBulletA[index] = Instantiate(enemyBulletAPrefab);
			enemyBulletA[index].SetActive(false);
		}

		for (int index = 0; index < enemyBulletB.Length; index++)
		{
			enemyBulletB[index] = Instantiate(enemyBulletBPrefab);
			enemyBulletB[index].SetActive(false);
		}

		for (int index = 0; index < enemyBulletC.Length; index++)
		{
			enemyBulletC[index] = Instantiate(enemyBulletCPrefab);
			enemyBulletC[index].SetActive(false);
		}

		for (int index = 0; index < enemyBulletD.Length; index++)
		{
			enemyBulletD[index] = Instantiate(enemyBulletDPrefab);
			enemyBulletD[index].SetActive(false);
		}

		for (int index = 0; index < followerBullet.Length; index++)
		{
			followerBullet[index] = Instantiate(followerBulletPrefab);
			followerBullet[index].SetActive(false);
		}

		//폭발애니메이션
		for (int index = 0; index < explosion.Length; index++)
		{
			explosion[index] = Instantiate(explosionPrefab);
			explosion[index].SetActive(false);
		}
	}

	public GameObject MakeObj(string type)
	{
		switch(type)
		{
			case "EnemyL":
				targetPool = enemyL;
				break;

			case "EnemyM":
				targetPool = enemyM;
				break;

			case "EnemyS":
				targetPool = enemyS;
				break;

			case "EnemyB":
				targetPool = enemyB;
				break;

			case "ItemCoin":
				targetPool = itemCoin;
				break;

			case "ItemPower":
				targetPool = itemPower;
				break;

			case "ItemBoom":
				targetPool = itemBoom;
				break;

			case "PlayerBulletA":
				targetPool = playerBulletA;
				break;

			case "PlayerBulletB":
				targetPool = playerBulletB;
				break;

			case "EnemyBulletA":
				targetPool = enemyBulletA;
				break;

			case "EnemyBulletB":
				targetPool = enemyBulletB;
				break;

			case "EnemyBulletC":
				targetPool = enemyBulletC;
				break;

			case "EnemyBulletD":
				targetPool = enemyBulletD;
				break;

			case "FollowerBullet":
				targetPool = followerBullet;
				break;

			case "Explosion":
				targetPool = explosion;
				break;
		}

		for(int index = 0; index < targetPool.Length; index++)
		{
			if(!targetPool[index].activeSelf)
			{
				targetPool[index].SetActive(true);
				return targetPool[index];
			}
		}
		return null;
	}
	public GameObject[] GetPool(string type)
	{
		switch (type)
		{
			case "EnemyL":
				targetPool = enemyL;
				break;

			case "EnemyM":
				targetPool = enemyM;
				break;

			case "EnemyS":
				targetPool = enemyS;
				break;

			case "EnemyB":
				targetPool = enemyB;
				break;

			case "ItemCoin":
				targetPool = itemCoin;
				break;

			case "ItemPower":
				targetPool = itemPower;
				break;

			case "ItemBoom":
				targetPool = itemBoom;
				break;

			case "PlayerBulletA":
				targetPool = playerBulletA;
				break;

			case "PlayerBulletB":
				targetPool = playerBulletB;
				break;

			case "EnemyBulletA":
				targetPool = enemyBulletA;
				break;

			case "EnemyBulletB":
				targetPool = enemyBulletB;
				break;

			case "EnemyBulletC":
				targetPool = enemyBulletC;
				break;

			case "EnemyBulletD":
				targetPool = enemyBulletD;
				break;

			case "FollowerBullet":
				targetPool = followerBullet;
				break;

			case "Explosion":
				targetPool = explosion;
				break;
		}
		return targetPool;
	}
}
