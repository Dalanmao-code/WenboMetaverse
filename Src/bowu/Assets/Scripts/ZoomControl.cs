using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ZoomControl : MonoBehaviour
{
    public float rotateSpeed;

    //原始大小
    private Vector3 InitialScale;
    private Vector3 InitialRotate;

    float scale = 1.0f;

    private float OffsetX = 0;
    private float OffsetY = 0;

    public Vector3 originPos;

    private void Awake()
    {
        //获取物体最原始大小
        InitialScale = this.transform.localScale;
        InitialRotate = transform.eulerAngles;
        //transform.position = originPos;
    }
    public void OnEnable()
    {
        if (InitialScale.x == 0)
        {
            InitialScale = transform.localScale;
        }
        transform.localScale = InitialScale;
        transform.eulerAngles = InitialRotate;
    }

    // Update is called once per frame
    private void Update()
    {
        scale = Input.GetAxis("Mouse ScrollWheel");

        //滚轮缩放模型
        if (scale != 0)//这个是鼠标滚轮响应函数
        {
            if (transform.localScale.x < transform.localScale.x - 0.25f && scale < 0)
                return;
            else if (transform.localScale.x > transform.localScale.x + 0.3f && scale > 0)
                return;

            transform.localScale = new Vector3(transform.localScale.x + scale, transform.localScale.x + scale, transform.localScale.x + scale);//改变物体大小
        }

        //按鼠标左键旋转
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("开始旋转");
            OffsetX = Input.GetAxis("Mouse X");//获取鼠标x轴的偏移量
            OffsetY = Input.GetAxis("Mouse Y");//获取鼠标y轴的偏移量

            transform.Rotate(new Vector3(OffsetY, -OffsetX, 0) * rotateSpeed, Space.World);
        }

    }
}
