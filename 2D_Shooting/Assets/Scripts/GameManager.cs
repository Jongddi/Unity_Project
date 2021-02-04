using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
	public int stage;
	public Animator stageAnim;
	public Animator clearAnim;
	public Animator fadeAnim;

	public Transform playerPos;

	public string[] enemyObjs;
	public Transform[] spawnPoints;

	public float nextSpawnDelay;
	public float curSpawnDelay;

	public GameObject player;
	public Text scoreText;
	public Image[] lifeImage;
	public Image[] boomImage;
	public GameObject gameOverSet;
	public ObjectManager objectManager;

	public List<Spawn> spawnList;
	public int spawnIndex;
	public bool spawnEnd;

	void Awake()
	{
		spawnList = new List<Spawn>();
		enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
		StageStart();
	}

	public void StageStart()
	{
		//Stage UI Load
		stageAnim.SetTrigger("On");
		stageAnim.GetComponent<Text>().text = "STAGE " + stage + "\nSTART";
		clearAnim.GetComponent<Text>().text = "STAGE " + stage + "\nCLEAR!";

		//Enemy Spawn File Read
		ReadSpawnFile();

		//Fade In
		fadeAnim.SetTrigger("In");
	}

	public void StageEnd()
	{
		//Clear UI
		clearAnim.SetTrigger("On");
		

		//Fade Out
		fadeAnim.SetTrigger("Out");

		//Player Repos
		player.transform.position = playerPos.position;

		//Stage Increament
		stage++;
		if(stage <2)
		{
			Invoke("gameOver", 6);
		}
		else
			Invoke("StageStart", 5);
	}

	void ReadSpawnFile()
	{
		//변수 초기화
		spawnList.Clear();
		spawnIndex = 0;
		spawnEnd = false;

		//리스폰 파일 열기
		TextAsset textFile = Resources.Load("Stage"+ stage) as TextAsset;
		StringReader stringReader = new StringReader(textFile.text);

		while(stringReader != null)
		{
			string line = stringReader.ReadLine();
			Debug.Log(line);
			if (line == null)
				break;

			//리스폰 데이터 생성
			Spawn spawnData = new Spawn();
			spawnData.delay = float.Parse(line.Split(',')[0]);
			spawnData.type = line.Split(',')[1];
			spawnData.point = int.Parse(line.Split(',')[2]);
			spawnList.Add(spawnData);
		}

		//텍스트 파일 닫기
		stringReader.Close();

		//첫번째 스폰 딜레이 적용
		nextSpawnDelay = spawnList[0].delay;
	}

	void Update()
	{
		curSpawnDelay += Time.deltaTime;

		if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
		{
			SpawnEnemy();
			curSpawnDelay = 0;
		}

		// UI score update
		Player playerLogic = player.GetComponent<Player>();
		scoreText.text = string.Format("{0:n0}", playerLogic.score);            //Format : 지정된 양식으로 문자열을 변환 , 0:n0 :세자리마다 쉼표를 나누는 숫자양식
	}

	void SpawnEnemy()
	{
		int enemyIndex = 0;
		switch(spawnList[spawnIndex].type)
		{
			case "S":
				enemyIndex = 0;
				break;
			case "M":
				enemyIndex = 1;
				break;
			case "L":
				enemyIndex = 2;
				break;
			case "B":
				enemyIndex = 3;
				break;
		}
		int enemyPoint = spawnList[spawnIndex].point;
		GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
		enemy.transform.position = spawnPoints[enemyPoint].position;

		Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
		Enemy enemyLogic = enemy.GetComponent<Enemy>();
		enemyLogic.player = player;
		enemyLogic.objectManager = objectManager;
		enemyLogic.gameManager = this;

		if (enemyPoint == 5 || enemyPoint == 6)             //Right Spawn
		{
			enemy.transform.Rotate(Vector3.back * 90);
			rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
		}
		else if (enemyPoint == 7 || enemyPoint == 8)            //Left Spawn
		{
			enemy.transform.Rotate(Vector3.forward * 90);
			rigid.velocity = new Vector2(enemyLogic.speed, -1);
		}
		else                                                     //Front Spawn
		{
			rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
		}

		//리스폰 인덱스 증가
		spawnIndex++;
		if(spawnIndex == spawnList.Count)
		{
			spawnEnd = true;
			return;
		}
		//다음 리스폰 딜레이 갱신
		nextSpawnDelay = spawnList[spawnIndex].delay;
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

	public void CallExplosion(Vector3 pos, string type)
	{
		GameObject explosion = objectManager.MakeObj("Explosion");
		Explosion explosionLogic = explosion.GetComponent<Explosion>();

		explosion.transform.position = pos;
		explosionLogic.StartExplosion(type);
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