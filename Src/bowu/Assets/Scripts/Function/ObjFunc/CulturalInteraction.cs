using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using JetBrains.Annotations;

public class CulturalInteraction : MonoSingleton<CulturalInteraction>
{
    public enum Select
    {
        Ray,
        Button
    }
    [Header("����")]
    public GameObject TestGameObject; //��������
    public GameObject TestGameObject1; //��������
    public Select select;
    public GameObject LastTime;
    private Coroutine RayLocalcoroutine;
    private Coroutine ButtonLocalcoroutine;


    // Start is called before the first frame update
    void Start()
    {
        //GetIdToIniCultural(TestGameObject, 0, select);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    GetIdToIniCultural(TestGameObject, 0, Select.Ray);
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    GetIdToIniCultural(TestGameObject, 0, Select.Button);
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    GetIdToIniCultural(TestGameObject1, 1, Select.Ray);
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    GetIdToIniCultural(TestGameObject1, 1, Select.Button);
        //}

    }
    
    public void GetIdToIniCultural(GameObject Canves_,int Id, Select select)
    {

        //Ψһ��ȷ��
        if(LastTime == null)
        {
            LastTime = Canves_;
        }
        else if(LastTime!=Canves_)
        {
            LastTime.SetActive(false);
            LastTime.transform.GetChild(1).gameObject.SetActive(false);
            LastTime = Canves_;
        }

        
        
        //

        if (Id < 0|| Canves_ == null)
        {
            Debug.LogError("This is not cultural Id");
            return;
        }
        else
        {
            if (!Canves_.activeSelf)
            {
                Canves_.SetActive(true); //û�м����򼤻�����
            }

            if (select == Select.Ray)
            {
                if (RayLocalcoroutine != null)
                {
                    StopCoroutine(RayLocalcoroutine);
                }
                Canves_.transform.GetChild(0).gameObject.SetActive(true);
                TextMeshProUGUI SinpleDescription = Canves_.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                SinpleDescription.text = null; //�ȹ���
                string SinpleDesText = Functions.GetCultureBaseById(Id).SimpleDescription;
                TextSlowToDisplay(SinpleDescription, SinpleDesText, 0.05f,select); //�򵥽��ܷ���
            }
            else if (select == Select.Button)
            {
                if (ButtonLocalcoroutine != null)
                {
                    StopCoroutine(ButtonLocalcoroutine);
                }
                //
                Canves_.transform.GetChild(1).gameObject.SetActive(true);
                Canves_.transform.GetChild(0).gameObject.SetActive(false);
                TextMeshProUGUI AllDescription = Canves_.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                AllDescription.text = null;
                string AllDesText = "   " + Functions.GetCultureBaseById(Id).Description;
                TextSlowToDisplay(AllDescription, AllDesText, 0.03f, select); //ȫ�����ܷ���
                                                                      //

                UnityEngine.UI.Image image = Canves_.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Image>();
                Sprite sprite = Functions.GetCultureBaseById(Id).CulturalImage;
                SlowShowImage_To(image, sprite, 1);
            }
        }

    }

    /// <summary>
    /// ͨ����������ʾ����
    /// </summary>
    public void TextSlowToDisplay(TextMeshProUGUI Target, string message, float DisplaySpeed, Select select)
    {
        if (select == Select.Ray)
        {
            RayLocalcoroutine = StartCoroutine(ShowTextGradually(Target, message, DisplaySpeed,select)); //����Я��
        }
        else if (select == Select.Button)
        {
            ButtonLocalcoroutine = StartCoroutine(ShowTextGradually(Target, message, DisplaySpeed,select)); //����Я��
        }
    }

    IEnumerator ShowTextGradually(TextMeshProUGUI Target, string message, float DisplaySpeed, Select select)
    {
        // ��������
        //userInputField.text = "";
        Target.text = null;

        Target.text += "";  // ��AI�ش�ǰ���� "AI: "

        foreach (char c in message)
        {
            Target.text += c;  // ���һ���ַ�
            yield return new WaitForSeconds(DisplaySpeed);  // ÿ���ַ�֮����ӳ�ʱ�䣬0.05����Ե���
        }

        Target.text += "\n";  // ��ɺ���
        if (select == Select.Ray)
        {
            RayLocalcoroutine = null;
        }
        else if (select == Select.Button)
        {
            ButtonLocalcoroutine = null;
        }
    }

    /// <summary>
    /// ����ʾͼƬ��Я��
    /// </summary>
    /// <param name="TargerImage"></param>
    /// <param name="sprite"></param>
    /// <param name="speed"></param>
    public void SlowShowImage_To(UnityEngine.UI.Image TargerImage, Sprite sprite, float speed)
    {
        //������ʩ
        if(TargerImage == null||sprite == null)
        {
            Debug.LogError("Not Find image or sprite");
            return;
        }
        StartCoroutine(SlowShowImage(TargerImage, sprite,speed));
    }

    IEnumerator SlowShowImage(UnityEngine.UI.Image TargerImage,Sprite sprite,float speed)
    {
        TargerImage.sprite = sprite;
        TargerImage.color = new Color(1, 1, 1, 0);
        while (TargerImage.color.a<=1)
        {
            TargerImage.color += new Color(0,0,0,1)*0.02f* speed;
            yield return new WaitForSeconds(0.02f);
        }
        

    }


}
