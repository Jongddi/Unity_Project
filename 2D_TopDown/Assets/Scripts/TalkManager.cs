using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData; //Key+Value
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }


    void GenerateData()
    {
        talkData.Add(1000, new string[] { "어서와:0", "여긴처음이지....?:1" });
        talkData.Add(2000, new string[] { "들어올 땐 마음대로지만...:0", "나갈땐 아니란다..:1" });
        talkData.Add(100, new string[] {"평범한 나무상자네...."});
        talkData.Add(200, new string[] { "누군가 사용한 듯한 책상이네.." });

        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(1000 + 1, portraitArr[1]);
        portraitData.Add(1000 + 2, portraitArr[2]);
        portraitData.Add(1000 + 3, portraitArr[3]);
        portraitData.Add(2000 + 0, portraitArr[4]);
        portraitData.Add(2000 + 1, portraitArr[5]);
        portraitData.Add(2000 + 2, portraitArr[6]);
        portraitData.Add(2000 + 3, portraitArr[7]);
    }

    public string GetTalk(int id, int talkIndex)
	{
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
	}

    public Sprite GetPortrait(int id, int portaraitIndex)
	{
        return portraitData[id + portaraitIndex];
	}
}
