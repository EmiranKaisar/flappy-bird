using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CognitiveAnswerBehavior : MonoBehaviour
{
    //TODO:
    //add variety in vertical position

    //本代码定义“认知题”的选项的行为
    public GameObject rightAnim;
    public GameObject wrongAnim;
    public float verticalOffsetBound = 3;
    private bool isRightAnswer = false;

    //在创建该选项时，就确定该答案是否是正确答案
    public void SetRightWrong(bool _right)
    {
        isRightAnswer = _right;
        Vector3 _pos = this.GetComponent<RectTransform>().position;
        this.GetComponent<RectTransform>().position = _pos + new Vector3(0, Random.Range(-verticalOffsetBound, verticalOffsetBound), 0);
    }

    //当Avatar碰撞时，进行对应的加/减分，然后进行destroy
    private void OnTriggerEnter(Collider other)
    {
        if(isRightAnswer){
            GameManager.Instance.AddScore(1, "Cognitive");
            Instantiate(rightAnim, this.transform.position , Quaternion.identity);
        }else{
            Instantiate(wrongAnim, this.transform.position, Quaternion.identity);
        }
            
        Destroy(this.transform.parent.gameObject);
    }
}
