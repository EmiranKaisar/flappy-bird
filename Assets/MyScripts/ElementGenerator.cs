using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElementGenerator : MonoBehaviour
{
    //本代码负责何时何地生产“碰撞物”/“认知题”

    public GameObject SnowFlake;
    public GameObject Rock;

    public GameObject IceCube;
    public GameObject Cloud;
    public GameObject Bulb;

    public GameObject Cognitive;

    private static bool created = false;

    private IEnumerator generateElement;
    private string sceneName;

    private int normalCount;
    private int cognitiveCount;

    //保证该代码不会因为跳转场景而被destroy
    void Awake()
    {
        // Ensure the script is not deleted while loading.
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    //停止生成
    public void StopGenerator()
    {
        if(generateElement != null)
            StopCoroutine(generateElement);
    }

    //初始化生成
    public void InitGenerator(string _sceneName)
    {
        if(generateElement != null)
           StopCoroutine(generateElement);
        generateElement = GenerateElement(_sceneName);
        StartCoroutine(generateElement);
    }

    //生成
    IEnumerator GenerateElement(string _sceneName)
    {
        sceneName = _sceneName;
        while(true)
        {
            Debug.Log(GameManager.Instance.portalPos);
            if(GameManager.Instance.portalPos.z > Camera.main.transform.position.z + 70){
                float _typeChooser = Random.Range(0, 9);
                if(_typeChooser > 2){
                    Vector3 _pos = new Vector3(Random.Range(-7, 7), Random.Range(-3, 3), 50) + Camera.main.transform.position;
                    float _collidorChooser = Random.Range(0, 3);
                    if(sceneName == "FlappyBird_1"){
                        Instantiate(Bulb, _pos, Quaternion.identity);
                    }else if(sceneName == "FlappyBird_2"){
                        if(_collidorChooser <= 1){
                            Instantiate(Rock, _pos, Quaternion.identity);
                        }else{
                            Instantiate(Cloud, _pos, Quaternion.identity);
                        }
                    }else{
                        if(_collidorChooser <= 1){
                            Instantiate(SnowFlake, _pos, Quaternion.identity);
                        }else{
                            Instantiate(IceCube, _pos, Quaternion.identity);
                        }
                    }
    
                    yield return new WaitForSeconds(Random.Range(3, 4));
    
                }else{
                    GenerateCognitive();
                    GameManager.Instance.AddCognitiveCount();
                    
                    yield return new WaitForSeconds(Random.Range(5, 6));
                }
    
            }else{
                yield return null;
            }
                
            
        }
    }

    //生成“认知题”
    public void GenerateCognitive()
    {
        Vector3 _pos = new Vector3(0, 0, 50) + Camera.main.transform.position;
        GameObject _cognitive = Instantiate(Cognitive, _pos, Quaternion.identity);
        string _operator = ReturnRandomOperator();
        int[] _operandAnswer = ReturnOperandAnswer(_operator);
        _cognitive.GetComponent<CognitiveElementBehavior>().SetCognitiveElement(_operator, _operandAnswer);
    }

    //随机生成“认知题”中的运算符
    private string ReturnRandomOperator()
    {
        float _upper = 1;

        if(sceneName == "FlappyBird_2"){
            _upper = 2;
        }else if(sceneName == "FlappyBird_3"){
            _upper = 3;
        }

        float _operateChooser = Random.Range(0, _upper);

        if(_operateChooser >= 0 && _operateChooser < 1){
            return "+";
        }else if(_operateChooser >= 1 && _operateChooser < 2){
            return "-";
        }else{
            return "*";
        }
    }

    //生成“认知题”的正确答案
    private int[] ReturnOperandAnswer(string _operator)
    {
        int[] _nums = new int[3];
        float _upper = 9;
        if(sceneName == "FlappyBird_2"){
            _upper = 20;
        }else if(sceneName == "FlappyBird_3"){
            _upper = 40;
        }

        if(_operator == "*")
           _upper = 9;
        

        int _first = (int) Random.Range(1, _upper);
        int _second = (int) Random.Range(1, _upper);
        _nums[0] = _first;
        _nums[1] = _second;
        if(_operator == "-"){
            if(_first < _second){
                _nums[0] = _second;
                _nums[1] = _first;
            }

            _nums[2] = _nums[0] - _nums[1];
        }else if(_operator == "+"){
            _nums[2] = _nums[0] + _nums[1];
        }else if(_operator == "*"){
            _nums[2] = _nums[0]*_nums[1];
        }
            
        return _nums;
    }
}
