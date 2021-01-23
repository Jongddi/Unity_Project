using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Press_Button : MonoBehaviour
{
    public AudioClip audioClick;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
	{
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(transform.gameObject); // 이렇게 하면 다음 scene으로 넘어가도 오브젝트가 사라지지 않습니다.
    }
  
    // Update is called once per frame
    public void Start_Game()
    {
        SceneManager.LoadScene("SampleScene");
        audioSource.clip = audioClick;
        audioSource.Play();
    }
}
