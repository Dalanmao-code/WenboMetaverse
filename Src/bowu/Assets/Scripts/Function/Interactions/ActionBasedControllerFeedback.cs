using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Diagnostics;
using UnityEngine.XR;

public class ActionBasedControllerFeedback : MonoBehaviour
{
    public ActionBasedController controller; // ActionBasedController 组件
    public XRRayInteractor rayInteractor; // 射线交互器
    public float hapticAmplitude = 0.5f; // 震动强度
    public float hapticDuration = 0.1f; // 震动时长
    public Material NewMaterial;

    private XRBaseInteractable currentInteractable; // 当前交互的物体

    

    void OnEnable()
    {
        // 注册射线悬停事件
        rayInteractor.hoverEntered.AddListener(OnHoverEntered);
        rayInteractor.hoverExited.AddListener(OnHoverExited);

        // 注册 Trigger 输入事件
        controller.activateAction.action.performed += OnTriggerPerformed;
    }

    private void Update()
    {
        
    }

    void OnDisable()
    {
        // 取消注册射线悬停事件
        rayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
        rayInteractor.hoverExited.RemoveListener(OnHoverExited);

        // 取消注册 Trigger 输入事件
        controller.activateAction.action.performed -= OnTriggerPerformed;
    }

    void OnTriggerPerformed(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            // 触发手柄震动
            
            controller.SendHapticImpulse(hapticAmplitude, hapticDuration);
            GameObject currentparent = currentInteractable.transform.parent.parent.gameObject;
            CulturalInteraction.Instance.GetIdToIniCultural(currentparent.transform.GetChild(0).gameObject, int.Parse(currentparent.name), CulturalInteraction.Select.Button);
        }
    }



    void OnHoverEntered(HoverEnterEventArgs args)
    {
        // 获取悬停的物体

        currentInteractable = args.interactable;
        if (currentInteractable.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            Interactiveeffects.AddEffect(meshRenderer, NewMaterial);
            GameObject currentparent = currentInteractable.transform.parent.parent.gameObject;
            CulturalInteraction.Instance.GetIdToIniCultural(currentparent.transform.GetChild(0).gameObject, int.Parse(currentparent.name), CulturalInteraction.Select.Ray);

        }
        else
        {
            currentInteractable = null;
        }

        
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (currentInteractable != null)
        {

            Interactiveeffects.RemoveEffect(currentInteractable.GetComponent<MeshRenderer>());
            // 清空当前交互物体
            currentInteractable = null;
        }
    }
}