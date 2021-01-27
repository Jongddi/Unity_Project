using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public TalkManager talkManager;
    public GameObject talkPanel;
    public Image portraitImg;
    public Text talkText;
    public GameObject scanObject;
    public bool isAction;
    public int talkIndex;

    public void Action(GameObject scanObj)
	{
       
	
       isAction = true;
       scanObject = scanObj;
       ObjectData objData = scanObject.GetComponent<ObjectData>();
       Talk(objData.id, objData.isNpc);
       
        talkPanel.SetActive(isAction);
    }

    void Talk(int id, bool isNpc)
	{
        string talkData = talkManager.GetTalk(id, talkIndex);

        if(talkData == null)
		{
            isAction = false;
            talkIndex = 0;
            return;
		}

        if(isNpc)
		{
            talkText.text = talkData.Split(':')[0];    //Split 구분자를 통하여 배열로 나눠주는 문자열 함수

            portraitImg.sprite = talkManager.GetPortrait(id, int.Parse (talkData.Split(':')[1])); //parse 문자열을 해당 타입으로 변화해주는 함수
            portraitImg.color = new Color(1, 1, 1, 1);
		}
        else
        {
            talkText.text = talkData;
            portraitImg.color = new Color(1, 1, 1, 0);
        }
        isAction = true;
        talkIndex++;
    }
}
