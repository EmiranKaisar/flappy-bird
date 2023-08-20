using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //本代码用于控制avatar

    Transform _bird;
    private float _widthBound;
    private float _heightBound;
    public float _speed = 0.01f;
    private static bool created = false;

    private Animator anim;
	private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent <CharacterController>();
		anim = gameObject.GetComponentInChildren<Animator>();
		anim.SetInteger ("AnimationPar", 1);
        
        _bird = this.transform;
        CalculateBound();
    }

    //计算avatar在屏幕中的运动范围
    private void CalculateBound()
    {
        float _fov =  Camera.main.fieldOfView;
        float _length =Mathf.Abs((Camera.main.transform.position - _bird.localPosition).z);
        float _aspect = Camera.main.aspect;
        _widthBound = _length*Mathf.Tan(_fov*Mathf.PI/360);
        _heightBound = _widthBound/_aspect;
    }

    //通过键盘控制
    void Update()
    {
        if(Input.GetKey(KeyCode.A) && _bird.localPosition.x > - _widthBound){
            _bird.localPosition -= new Vector3(1, 0, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.D) && _bird.localPosition.x < _widthBound){
            _bird.localPosition += new Vector3(1, 0, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.W) && _bird.localPosition.y < _heightBound){
            _bird.localPosition += new Vector3(0, 1, 0)*_speed;
        }

        if(Input.GetKey(KeyCode.S) && _bird.localPosition.y > - _heightBound){
            _bird.localPosition -= new Vector3(0, 1, 0)*_speed;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private Quaternion youngDeviceAnchorQuat;
    private Quaternion oldDeviceAnchorQuat;

    //通过传感器控制横向移动
    private bool _horFirst = true;
    public void ControlPlayerHorMovement(Quaternion _deviceRot)
    {
        if(_horFirst){
            oldDeviceAnchorQuat = _deviceRot;
            _horFirst = false;
        }else{
            Quaternion _rot = _deviceRot*Quaternion.Inverse(oldDeviceAnchorQuat);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            _rot.ToAngleAxis(out angle, out axis);
            axis.Normalize();

            float _angle = angle;
            if(_angle >= 90)
                _angle = 90;
            if(Mathf.Abs(Vector3.Dot(axis, Vector3.forward)) >= 0.5){
                if(Vector3.Dot(axis, Vector3.forward) < 0){
                    _bird.localPosition = new Vector3(7.8f*(_angle/90), _bird.localPosition.y, _bird.localPosition.z);
                }else{
    
                    _bird.localPosition = new Vector3(-7.8f*(_angle/90), _bird.localPosition.y, _bird.localPosition.z);
                }
            }
        }
    }

    //通过传感器控制纵向移动
    private bool _verFirst = true;
    public void ControlPlayerVerMovement(Quaternion _deviceRot)
    {
        if(_verFirst){
            youngDeviceAnchorQuat = _deviceRot;
            _verFirst = false;
        }else{
            Quaternion _rot = _deviceRot*Quaternion.Inverse(youngDeviceAnchorQuat);
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            _rot.ToAngleAxis(out angle, out axis);
            axis.Normalize();
            float _angle = angle;
            if(_angle >= 90)
                _angle = 90;
            if(Mathf.Abs(Vector3.Dot(axis, Vector3.right)) >= 0.5){
                if(Vector3.Dot(axis, Vector3.right) < 0){
                    _bird.localPosition = new Vector3(_bird.localPosition.x, 3.7f*(_angle/90),  _bird.localPosition.z);
                }else{
                    _bird.localPosition = new Vector3(_bird.localPosition.x, -3.7f*(_angle/90),  _bird.localPosition.z);
                }
            }
        }
    }


}
