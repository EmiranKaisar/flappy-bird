using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    //TODO:
    //检查重名
    //打印所有数据的功能

    //本代码控制游戏的状态
    
    private static  GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
               Debug.Log("GameManager is null !");
            
            return _instance;
        }
    }

    private int playerScore;
    private int playerBalanceScore;
    private int playerCognitiveScore;

    [HideInInspector]
    public float playTime;

    [HideInInspector]
    public int cooperationMode; //0:single, 1:cooperation_1, 2:cooperation_2

    private int levelIndex;
    private int presentStateIndex;
    private int previousStateIndex;
    private bool ifPuase;
    private bool ifCognitiveWait;
    private int homeIndex;
    private int cognitiveCount;
    private string playerName;

    //一个场景的时间
    public float oneSceneLength = 100;
    //avatar的速度
    public float playerSpeed = 10;

    //在avatar临近“认知题”时所停顿的时间，单位：秒
    public float cognitiveTime = 1;

    [System.Serializable]
    public class ActionClass
    {
      public string actionName;
      public string stateName;
      public GameObject stateUIObject;

    }

    [SerializeField]
    public List<ActionClass> ActionList;

    private bool created = false;

    public ElementGenerator elementGenerator;

    public GameObject CanvasObj;
    public TutorialBehavior tutorialBehavior;

    public GameObject GamingScoreText;
    public TMP_Text ResultScoreText;
    public TMP_Text ResultCognitiveScoreText;
    public TMP_Text ResultPlayerName;
    public TMP_Text ResultPlayerRank;
    public TMP_InputField PlayerNameInputField;
    public TMP_Text MentionName;

    public TMP_Text GamingTimeText;

    public Image TimeFill;

    public GameObject StartBanner;
    public GameObject EndBanner;

    //定义哪些东西是不能在转换场景的时候被destroy的
    void Awake()
    {
        if (!created)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(CanvasObj);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
            Destroy(CanvasObj);
        }

        InitGame();
        InitUI();
    }

    //初始化游戏
    private void InitGame()
    {
        //connect BLE
        //init data
        playerScore = 0;
        playerBalanceScore = 0;
        playerCognitiveScore = 0;
        presentStateIndex = 0;
        previousStateIndex = 0;
        levelIndex = 0;
        playTime = 0;
        cognitiveCount = 0;
        ifPuase = false;
        ifCognitiveWait = false;

        PlayerNameInputField.text = playerName;
        GamingScoreText.GetComponent<TMP_Text>().text = "0";
        ResultScoreText.text = "0";
        ResultCognitiveScoreText.text = "0";
        StartBanner.SetActive(false);
        EndBanner.SetActive(false);
    }

    //初始化UI
    private void InitUI()
    {
      int i = 0;
      foreach (var item in ActionList)
      {
        item.stateUIObject.SetActive(false);
        if(item.stateName == "Home")
          homeIndex = i;
        
        i++;
      }

      ShowUI(homeIndex);
    }

    public void ChooseModeButtonAction(int _mode)
    {
        cooperationMode = _mode;
        StateButtonAction("GotoTutorial");
    }

    //根据玩家的输入跳转到对应的游戏状态
    public void StateButtonAction(string _actionName)
    {
        presentStateIndex = ReturnIndexByActionName(_actionName);

        if(presentStateIndex != previousStateIndex){
            if(_actionName == "GotoPlay"){
                CheckName();
            }else{
                UpdateState();
                previousStateIndex = presentStateIndex;
            } 
        }
    }

    //检查玩家是否输入名称
    private void CheckName()
    {
        playerName = PlayerNameInputField.text;
        if(playerName != ""){
            UpdateState();
            previousStateIndex = presentStateIndex;
            MentionName.text = "";
        }else{
            MentionName.text = "请填写队名";
        }
    }

    //更新状态
    private void UpdateState()
    {
        if(ActionList[presentStateIndex].actionName != "GotoResult"){
            HideUI(previousStateIndex);
            ShowUI(presentStateIndex);
        }

        if(ActionList[presentStateIndex].actionName == "ReturnHome")
            HideUI(ReturnIndexByActionName("GotoPlay"));
        
        if(ActionList[presentStateIndex].actionName == "GotoPlay")
           GotoTutorial();

        if(ActionList[presentStateIndex].actionName == "GotoPlay")
           GotoPlay();
        
        if(ActionList[presentStateIndex].actionName == "ReturnHome")
           ReturnHome();
        
        if(ActionList[presentStateIndex].actionName == "GotoPause")
           GotoPause();
        
        if(ActionList[presentStateIndex].actionName == "ReturnPlay")
           ReturnPlay();
        
        if(ActionList[presentStateIndex].actionName == "GotoResult")
           GotoResult();
    }

    //隐藏对应的UI
    private void HideUI(int _index)
    {
        ActionList[_index].stateUIObject.SetActive(false);
    }

    //显示对应的UI
    private void ShowUI(int _index)
    {
        ActionList[_index].stateUIObject.SetActive(true);
    }

    private void GotoTutorial()
    {
        tutorialBehavior.UpdateContent(cooperationMode);
    }

    //去“游玩”状态
    private void GotoPlay()
    {
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(playerSpeed);
        playTime = 0;

        GetLevelIndex("FlappyBird_1");
    }

    //回“首页”状态
    private void ReturnHome()
    {
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(0);
        elementGenerator.StopGenerator();

        InitGame();
    }

    //去“暂停”状态
    private void GotoPause()
    {
        Time.timeScale = 0;
    }

    //回“游玩”状态
    private void ReturnPlay()
    {
        Time.timeScale = 1;
    }

    //去“结果”状态
    private void GotoResult()
    {
        CreateData();
        StartCoroutine(EndSequence());
    }

    DateTime dt;
    //将玩家本次游玩的结果进行储存
    private void CreateData()
    {
        PlayerPrefs.SetInt(playerName, playerScore);

        dt = DateTime.Now;
        PlayerPrefs.SetString(playerName+"_"+"date", dt.ToString());

        PlayerPrefs.SetInt(playerName+"_"+"cognitive", (int)(((float)playerCognitiveScore/cognitiveCount)*100));

        PlayerPrefs.SetInt(playerName+"_"+"mode", cooperationMode);

        string allPlayers = PlayerPrefs.GetString("AllPlayers");
        string newAllPlayers = allPlayers + playerName + ";";
        PlayerPrefs.SetString("AllPlayers", newAllPlayers);
    }

    //返回本次游玩结果的排行
    private int ReturnThisScoreRank(int _score)
    {
        int _rank = 1;
        string allPlayers = PlayerPrefs.GetString("AllPlayers");
        if(allPlayers != null){
            string[] allPlayersList = allPlayers.Split(';');
            for (int i = 0; i < allPlayersList.Length - 1; i++)
            {
                int thisScore = PlayerPrefs.GetInt(allPlayersList[i]);
                if(thisScore > _score)
                   _rank++;
            }
        }

        return _rank;
    }

    //游戏结束时banner的短暂展示
    private IEnumerator EndSequence()
    {
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(0);
        elementGenerator.StopGenerator();

        yield return StartCoroutine(PutOnBannaer(EndBanner));

        HideUI(previousStateIndex);
        ShowUI(presentStateIndex);
        ResultPlayerName.text = playerName;
        ResultPlayerRank.text = ReturnThisScoreRank(playerScore).ToString();
        ResultScoreText.text = playerScore.ToString();
        ResultCognitiveScoreText.text = ((int)(((float)playerCognitiveScore/cognitiveCount)*100)).ToString();
    }

    //展示banner
    private IEnumerator PutOnBannaer(GameObject _obj)
    {
        _obj.SetActive(true);
        yield return new WaitForSeconds(1);
        _obj.SetActive(false);
    }

    void Update()
    {
        if((ActionList[presentStateIndex].actionName == "GotoPlay" || ActionList[presentStateIndex].actionName == "ReturnPlay") && !ifCognitiveWait){
            UpdateTime();
        }
        
    }

    //更新目前的时间
    private int _minutes;
    private int _seconds;
    private void UpdateTime()
    {
        playTime += Time.deltaTime;
        _seconds = ((int)playTime % 60);
        _minutes = ((int) playTime / 60);
        GamingTimeText.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
        TimeFill.fillAmount = (playTime%oneSceneLength)/oneSceneLength;
    }

    //跳转场景
    private bool loadFinish = false;
    public void GetLevelIndex(string _sceneName)
    {
        if(_sceneName == "FlappyBird_1"){
            levelIndex = 0;
        }else if(_sceneName == "FlappyBird_2"){
            levelIndex = 1;
        }else if(_sceneName == "FlappyBird_3"){
            levelIndex = 2;
        }

        if(_sceneName != "GotoResult"){
            loadFinish = false;
            SceneManager.LoadScene(_sceneName);

            SceneManager.sceneLoaded += OnSceneLoaded;

            elementGenerator.InitGenerator(_sceneName);
        }
        
        
    }

    //在进入一个场景时所要进行的一系列操作
    public Vector3 portalPos;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject _portal = GameObject.Find("Portal");
        
        Vector3 _camPos = Camera.main.transform.position;
        portalPos = _camPos + new Vector3(0, -14, playerSpeed*oneSceneLength + 12);
        _portal.SetActive(false);
        _portal.transform.position = portalPos;
        _portal.SetActive(true);
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(playerSpeed);
        
        if(levelIndex == 0)
           StartCoroutine(PutOnBannaer(StartBanner));

        loadFinish = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //在临近“认知题”时有一小段停顿
    public void CallCognitiveWait()
    {
        StartCoroutine(CognitiveWait());
    }

    //在临近“认知题”时有一小段停顿
    private IEnumerator CognitiveWait()
    {
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(0);
        ifCognitiveWait = true;
        yield return new WaitForSeconds(cognitiveTime);
        ifCognitiveWait = false;
        Camera.main.GetComponent<ModularTerrainCameraControl>().SetSpeed(playerSpeed);
    }

    //加/减分数
    public void AddScore(int _point, string _type)
    {
        playerScore += _point;

        if(_type == "Balance"){
            playerBalanceScore += _point;
        }else{
            //playerCognitiveScore用于统计个数，以计算准确率，所以这里只+1
            playerCognitiveScore += 1;
        }

        GamingScoreText.GetComponent<TMP_Text>().text = playerScore.ToString();
    }

    //记录“认知题”总数
    public void AddCognitiveCount()
    {
        cognitiveCount++;
    }

    public void InitAnchorButtonAction()
    {
        GameObject.FindGameObjectWithTag("Crow").GetComponent<PlayerController>().InitAnchor();
    }

    //接受“状态名称”，返回对应的"状态序号"
    private int ReturnIndexByActionName(string _actionName)
    {
        int i = 0;
        int returnIndex = 0;
        foreach (var item in ActionList)
        {
            if(item.actionName == _actionName)
               returnIndex = i;

           i++;     
        }

        return returnIndex;
    }

    private string oneData;
    public void PrintResultButtonAction()
    {
        oneData = "player: "+playerName+"; "+"score: "+playerScore.ToString()+"; "+"cognitive: "+ ((int)(((float)playerCognitiveScore/cognitiveCount)*100)).ToString() +"; "+"mode: "+cooperationMode.ToString()+"; "+"date: "+dt.ToString()+";";
        Debug.Log(oneData);
    }


    public void PrintAllResultButtonAction()
    {
        string allPlayers = PlayerPrefs.GetString("AllPlayers");
        string tempDateString = "";
        if(allPlayers != null){
            
            string[] allPlayersList = allPlayers.Split(';');
            for (int i = 0; i < allPlayersList.Length - 1; i++)
            {
                if(PlayerPrefs.GetString(allPlayersList[i]+"_date", "null") != "null"){
                    int thisScore = PlayerPrefs.GetInt(allPlayersList[i]);
                    int thisCognitive = PlayerPrefs.GetInt(allPlayersList[i]+"_cognitive");
                    int thisMode = PlayerPrefs.GetInt(allPlayersList[i]+"_mode");
                    tempDateString = PlayerPrefs.GetString(allPlayersList[i]+"_date");
                    oneData = "player: "+allPlayersList[i]+"; "+"score: "+thisScore.ToString()+"; "+"cognitive: "+ thisCognitive.ToString() +"; "+"mode: "+thisMode.ToString()+"; "+"date: "+tempDateString+";";
                    Debug.Log(oneData);
                }
            }
        }
    }

}
