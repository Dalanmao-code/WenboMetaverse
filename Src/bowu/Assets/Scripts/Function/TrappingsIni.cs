using CC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrappingsIni : MonoBehaviour
{
    [Header("测试集")]
    public bool IsOnTest = false;
    public CharacterCustomization character;
    public int selection;
    public int topSlot = 0;  // 上衣槽位
    public int topMaterialIndex = 0; // 材质变体索引
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
            // 方法1：通过索引换装（需知道服装在列表中的顺序）
            character.setApparel(selection, topSlot, topMaterialIndex);

        }
    }
    /// <summary>
    /// 初始化服饰进程
    /// </summary>
    /// <returns></returns>
    public IEnumerator IniTrapping()
    {
        yield return new WaitForSeconds(0.1f);
        character.setApparel(7, 1, 1); //初始化衣服
        character.setApparel(8, 0, 0); //初始化裤子
        character.setApparel(2, 2, 2); //初始化鞋子
        TrappingUI = GameObject.Find("UI_Parent");
        TrappingUI.SetActive(false);
        TrappingUI.GetComponentInChildren<Canvas>().worldCamera = camera;
    }

}
