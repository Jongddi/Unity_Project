using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public string[] enemyObjs;
	public Transform[] spawnPoints;

	public float maxSpawnDelay;
	public float curSpawnDelay;

	public GameObject player;
	public Text scoreText;
	public Image[] lifeImage;
	public Image[] boomImage;
	public GameObject gameOverSet;
	public ObjectManager objectManager;

	private void Awake()
	{
		enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
	}

	void Update()
	{
		curSpawnDelay += Time.deltaTime;

		if (curSpawnDelay > maxSpawnDelay)
		{
			SpawnEnemy();
			maxSpawnDelay = Random.Range(0.5f, 3f);
			curSpawnDelay = 0;
		}

		// UI score update
		Player playerLogic = player.GetComponent<Player>();
		scoreText.text = string.Format("{0:n0}", playerLogic.score);            //Format : 지정된 양식으로 문자열을 변환 , 0:n0 :세자리마다 쉼표를 나누는 숫자양식
	}

	void SpawnEnemy()
	{
		int ranEnemy = Random.Range(0, 3);
		int ranPoint = Random.Range(0, 9);
		GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
		enemy.transform.position = spawnPoints[ranPoint].position;

		Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
		Enemy enemyLogic = enemy.GetComponent<Enemy>();
		enemyLogic.player = player;
		enemyLogic.objectManager = objectManager;

		if (ranPoint == 5 || ranPoint == 6)             //Right Spawn
		{
			enemy.transform.Rotate(Vector3.back * 90);
			rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
		}
		else if (ranPoint == 7 || ranPoint == 8)            //Left Spawn
		{
			enemy.transform.Rotate(Vector3.forward * 90);
			rigid.velocity = new Vector2(enemyLogic.speed, -1);
		}
		else                                                     //Front Spawn
		{
			rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
		}
	}

	public void RespawnPlayer()
	{
		Invoke("RespawnPlayerExe", 2f);
	}
	void RespawnPlayerExe()
	{
		player.transform.position = Vector3.down * 3.5f;
		player.SetActive(true);

		Player playerLogic = player.GetComponent<Player>();
		playerLogic.isHit = false;
	}

	public void UpdateLifeIcon(int life)
	{
		//UI Life Init Disable
		for (int index = 0; index < 3; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 0);
		}

		//UI Life Init Active
		for (int index = 0; index < life; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 1);
		}
	}

	public void UpdateBoomIcon(int boom)
	{
		//UI Boom Init Disable
		for (int index = 0; index < 3; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 0);
		}

		//UI Boom Init Active
		for (int index = 0; index < boom; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 1);
		}
	}

	public void gameOver()
	{
		gameOverSet.SetActive(true);
	}

	public void GameTry()
	{
		SceneManager.LoadScene(0);
	}
}