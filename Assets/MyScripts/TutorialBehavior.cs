using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour
{
    //本代码控制“教学页面”行为

    public Sprite[] TutorialBG;

    private Image thisImage;

    void Start()
    {
        
    }

    public void UpdateContent(int _mode)
    {
        thisImage = GetComponent<Image>();
        thisImage.sprite = TutorialBG[_mode];
    }

}
