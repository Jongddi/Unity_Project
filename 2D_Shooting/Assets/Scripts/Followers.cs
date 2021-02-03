using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followers : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

	private void Awake()
	{
        parentPos = new Queue<Vector3>();
	}

	void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }
    void Watch()
	{
        //Queue = FIFO(First Input First Output)
        if(!parentPos.Contains(parent.position))                 //부모위치가 멈추면 자식도 그자리에 멈추는 함수
                parentPos.Enqueue(parent.position);         // input

        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();             //output
        else if (parentPos.Count < followDelay)         //큐가 채워지기 전까진 부모 위치 적용
            followPos = parent.position;
	}

    void Follow()
    {
        transform.position = followPos;
    }
    
    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0;

    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
