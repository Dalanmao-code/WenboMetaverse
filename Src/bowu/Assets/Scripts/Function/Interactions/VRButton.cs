using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class VRButton : MonoBehaviour
{
    public GameObject RayInteractor;
    private bool IsOn = true;
    private bool _previousButtonState = false; // 上一帧按钮状态
    private bool _currentButtonState = false;  // 当前帧按钮状态
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 获取右手柄设备
        UnityEngine.XR.InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // 检测B按钮（SecondaryButton）的当前状态
        rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out _currentButtonState);

        // 如果当前帧按下且上一帧未按下，表示按钮刚刚按下
        if (_currentButtonState && !_previousButtonState)
        {
            // 触发B按钮按下事件
            if (IsOn)
            {
                RayInteractor.SetActive(false);
                IsOn = false;
            }
            else
            {
                RayInteractor.SetActive(true);
                IsOn = true;
            }
        }

        // 更新上一帧按钮状态
        _previousButtonState = _currentButtonState;
    }
}
