using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Man : MonoBehaviour
{
    public GameObject objSwordMan;
    Animator animator;
    // Start is called before the first frame update
    void Start()    //스크립트가 실행되고 업데이트가 실행되기전에 한번 실행
    {
        transform.position = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() //매 프레임마다 호출되는 메시지
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.localScale = new Vector3(-1, 1, 1);   //transform.rotation은 이미 지정되어 있기 때문에 localScale을 수정하여 방향 전환
            animator.SetBool("moving", true);
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("moving", true);
            transform.Translate(Vector3.left * Time.deltaTime);
        }
        else animator.SetBool("moving", false);
        //transform.Translate(new Vector3(h, 0, 0) * Time.deltaTime);         //Time.time은 1프레임당 걸리는 시간

        if (Input.GetKeyDown(KeyCode.A) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
            animator.SetTrigger("attack");
		}
    }
}
