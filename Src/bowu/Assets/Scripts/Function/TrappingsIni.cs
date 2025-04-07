using CC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrappingsIni : MonoBehaviour
{
    [Header("���Լ�")]
    public bool IsOnTest = false;
    public CharacterCustomization character;
    public int selection;
    public int topSlot = 0;  // ���²�λ
    public int topMaterialIndex = 0; // ���ʱ�������
    [Header("Static")]
    public static GameObject TrappingUI;
    [Header("Camera")]
    public  Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IniTrapping());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&& IsOnTest)
        {
            // ����1��ͨ��������װ����֪����װ���б��е�˳��
            character.setApparel(selection, topSlot, topMaterialIndex);

        }
    }
    /// <summary>
    /// ��ʼ�����ν���
    /// </summary>
    /// <returns></returns>
    public IEnumerator IniTrapping()
    {
        yield return new WaitForSeconds(0.1f);
        character.setApparel(7, 1, 1); //��ʼ���·�
        character.setApparel(8, 0, 0); //��ʼ������
        character.setApparel(2, 2, 2); //��ʼ��Ь��
        TrappingUI = GameObject.Find("UI_Parent");
        TrappingUI.SetActive(false);
        TrappingUI.GetComponentInChildren<Canvas>().worldCamera = camera;
    }

}
