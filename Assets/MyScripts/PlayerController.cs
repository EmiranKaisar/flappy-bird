using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //本代码用于控制avatar

    Transform _bird;
    public float widthBound = 7.8f;
    public float heightBound = 3.7f;
    public float _speed = 0.01f;

    private Animator anim;
	private CharacterController controller;
    private Vector3 initPos;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent <CharacterController>();
		anim = gameObject.GetComponentInChildren<Animator>();
		anim.SetInteger ("AnimationPar", 1);
        
        _bird = this.transform;
        initPos = _bird.localPosition;

        
    }


    //通过键盘控制
    void Update()
    {
        if(Input.GetKey(KeyCode.A) && _bird.localPosition.x > - widthBound){
            _bird.localPosition -= new Vector3(1, 0, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.D) && _bird.localPosition.x < widthBound){
            _bird.localPosition += new Vector3(1, 0, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.W) && _bird.localPosition.y < heightBound){
            _bird.localPosition += new Vector3(0, 1, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.S) && _bird.localPosition.y > - heightBound){
            _bird.localPosition -= new Vector3(0, 1, 0)*_speed;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    //重新校准传感器
    public void InitAnchor()
    {
        _horFirst = true;
        _verFirst = true;
        _first = true;
        _bird.localPosition = initPos;
    }

    private Quaternion verAnchorQuat;
    private Quaternion horAnchorQuat;
    private Quaternion anchorQuat;

    //通过传感器控制横向移动
    private bool _horFirst = true;
    public void ControlPlayerHorMovement(Quaternion _deviceRot)
    {
        if(_horFirst){
            horAnchorQuat = _deviceRot;
            _horFirst = false;
        }else{
            Quaternion _rot = _deviceRot*Quaternion.Inverse(horAnchorQuat);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            _rot.ToAngleAxis(out angle, out axis);
            axis.Normalize();

            float _angle = angle;
            if(_angle >= 90)
                _angle = 90;
            if(Mathf.Abs(Vector3.Dot(axis, Vector3.forward)) >= 0.5){
                if(Vector3.Dot(axis, Vector3.forward) < 0){
                    _bird.localPosition = new Vector3(-widthBound*(_angle/90), _bird.localPosition.y, _bird.localPosition.z);
                }else{
                    _bird.localPosition = new Vector3(widthBound*(_angle/90), _bird.localPosition.y, _bird.localPosition.z);
                }
            }
        }
    }

    //通过传感器控制纵向移动
    private bool _verFirst = true;
    private float handMotionFactor = 1.0f;
    public void ControlPlayerVerMovement(Quaternion _deviceRot, float _range)
    {
        if(_range > 30)
            handMotionFactor = -1.0f;
        else
            handMotionFactor = 1.0f;
          
        if(_verFirst){
            verAnchorQuat = _deviceRot;
            _verFirst = false;
        }else{
            Quaternion _rot = _deviceRot*Quaternion.Inverse(verAnchorQuat);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            _rot.ToAngleAxis(out angle, out axis);
            axis.Normalize();
            float _angle = angle;
            if(_angle >= _range)
                _angle = _range;
            if(Mathf.Abs(Vector3.Dot(axis, Vector3.right)) >= 0.5){
                if(Vector3.Dot(axis, Vector3.right) < 0){
                    _bird.localPosition = new Vector3(_bird.localPosition.x, heightBound*(_angle/_range)*handMotionFactor,  _bird.localPosition.z);
                }else{
                    _bird.localPosition = new Vector3(_bird.localPosition.x, -heightBound*(_angle/_range)*handMotionFactor,  _bird.localPosition.z);
                }
            }
        }
    }

    private bool _first = true;
    public void ControlPlayerMovement(Quaternion _deviceRot)
    {
        if(_first){
            anchorQuat = _deviceRot;
            _first = false;
        }else{
            Quaternion _rot = _deviceRot*Quaternion.Inverse(anchorQuat);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            _rot.ToAngleAxis(out angle, out axis);
            axis.Normalize();

            float x_factor = Vector3.Dot(axis, Vector3.right);
            float y_factor = Vector3.Dot(axis, Vector3.forward);

            Vector3 moveDir = new Vector3(y_factor, -x_factor, 0).normalized;
            float moveRange = widthBound;
            if(moveDir.y >= 0.3f)
                moveRange = Mathf.Abs(heightBound/moveDir.y);
            
            if(angle >= 90)
                angle = 90;

            _bird.localPosition = new Vector3(0, 0, _bird.localPosition.z) + moveRange*(angle/90)*moveDir;
        }
    }


}
