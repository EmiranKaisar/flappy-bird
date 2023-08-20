using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBehavior : MonoBehaviour
{
    //本代码控制特效的播放时间

    void Start()
    {
        StartCoroutine(CountDown());
    }

    //在播放0.5秒之后进行destroy
    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }


}
