using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalBehavior : MonoBehaviour
{
    //本代码定义传送门的行为
    public string nextScene = "";

    private void OnTriggerEnter(Collider other)
    {
        if(nextScene == "GotoResult" && GameManager.Instance.playTime >= GameManager.Instance.oneSceneLength*3 - 10)
            GameManager.Instance.StateButtonAction("GotoResult");
        else
            GameManager.Instance.GetLevelIndex(nextScene);
    }
}
