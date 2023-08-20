using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour
{
    //本代码控制“教学页面”行为

    public Sprite[] TutorialBG;

    public GameObject[] TutorialVideo;
    private Image thisImage;
    private int presentIndex;

    void Start()
    {
        presentIndex = 0;
        thisImage = GetComponent<Image>();
        UpdateSprite();
    }

    //向右切换
    public void RightButtonAction()
    {
        presentIndex++;
        if(presentIndex > 2)
           presentIndex = 0;
        
        UpdateSprite();
    }

    //向左切换
    public void LeftButtonAction()
    {
        presentIndex--;
        if(presentIndex < 0)
           presentIndex = 2;
        
        UpdateSprite();
    }

    //更改页面的背景
    private void UpdateSprite()
    {
        thisImage.sprite = TutorialBG[presentIndex];
        
        if(presentIndex == 0)
            HideShowVideo(false);
        else
            HideShowVideo(true);
    }

    //根据情况更改教学视频
    private void HideShowVideo(bool _show)
    {
        for (int i = 0; i < TutorialVideo.Length; i++)
        {
            TutorialVideo[i].SetActive(_show);
        }
    }
}
