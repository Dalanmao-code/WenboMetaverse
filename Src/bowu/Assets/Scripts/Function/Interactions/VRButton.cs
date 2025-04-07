using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class VRButton : MonoBehaviour
{
    public GameObject RayInteractor;
    private bool IsOn = true;
    private bool _previousButtonState = false; // ��һ֡��ť״̬
    private bool _currentButtonState = false;  // ��ǰ֡��ť״̬
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ��ȡ���ֱ��豸
        UnityEngine.XR.InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // ���B��ť��SecondaryButton���ĵ�ǰ״̬
        rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out _currentButtonState);

        // �����ǰ֡��������һ֡δ���£���ʾ��ť�ոհ���
        if (_currentButtonState && !_previousButtonState)
        {
            // ����B��ť�����¼�
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

        // ������һ֡��ť״̬
        _previousButtonState = _currentButtonState;
    }
}
