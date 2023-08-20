using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBehavior : MonoBehaviour
{
    //该代码定义“碰撞物”的行为
    public bool ifAvoid;
    public GameObject thisAnim;
    private Transform thisTransform;

    void Start()
    {
        thisTransform = this.transform;
    }

    //与avatar进行碰撞后，进行相应的加/减分，然后destroy
    private void OnTriggerEnter(Collider other)
    {
        if(ifAvoid){
            GameManager.Instance.AddScore(-1, "Balance");
        }else{
            GameManager.Instance.AddScore(1, "Balance");
        }
        Instantiate(thisAnim, this.transform.position, Quaternion.identity);
        
        Destroy(this.gameObject);
    }

    //如果avatar没有碰撞，那么自行destroy
    void Update()
    {
        if(thisTransform.position.z - Camera.main.transform.position.z < 1)
            Destroy(this.gameObject);
    }

}
