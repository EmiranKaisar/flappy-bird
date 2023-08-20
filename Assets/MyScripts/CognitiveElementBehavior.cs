using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CognitiveElementBehavior : MonoBehaviour
{
    //本代码定义“认知题”的行为
    private string Operator;
    private int firstOperand;
    private int secondOperand;
    private int rightAnswer;
    private Transform thisTransform;
    private GameObject thisObj;

    void Start()
    {
        thisObj = this.gameObject;
        thisTransform = this.transform;
    }
    
    //创建认知题，包括一个问题和两个选项
    public void SetCognitiveElement(string _operator, int[] _operandAnswer)
    {
        int _fakeAnswer = _operandAnswer[2] + (int)Random.Range(1, 9);
        float _sideChooser = Random.Range(0, 2);
        this.GetComponentsInChildren<TMP_Text>()[0].text = _operandAnswer[0].ToString() + " " + _operator + " " + _operandAnswer[1].ToString()  + " = ?";

        if(_sideChooser < 1){
            this.GetComponentsInChildren<TMP_Text>()[1].text = _fakeAnswer.ToString();
            this.GetComponentsInChildren<CognitiveAnswerBehavior>()[0].SetRightWrong(false);

            this.GetComponentsInChildren<TMP_Text>()[2].text = _operandAnswer[2].ToString();
            this.GetComponentsInChildren<CognitiveAnswerBehavior>()[1].SetRightWrong(true);
        }else{
            this.GetComponentsInChildren<TMP_Text>()[1].text = _operandAnswer[2].ToString();
            this.GetComponentsInChildren<CognitiveAnswerBehavior>()[0].SetRightWrong(true);

            this.GetComponentsInChildren<TMP_Text>()[2].text = _fakeAnswer.ToString();
            this.GetComponentsInChildren<CognitiveAnswerBehavior>()[1].SetRightWrong(false);
        }
        
    }

    //如果两个选项都没有选，那么该问题会自行destroy
    void Update()
    {
        if(thisTransform.position.z - Camera.main.transform.position.z < 1){
            Destroy(this);
        }else if(thisTransform.position.z - Camera.main.transform.position.z <= 30){
            GameManager.Instance.CallCognitiveWait();
        }
    }
}
