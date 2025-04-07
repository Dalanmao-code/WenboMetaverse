using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamInteractive : MonoBehaviour
{
    private Camera _cam;

    public Transform model_Parent;
    public Transform ui_Parent;

    public MediaPlayer mediaPlayer;

    public GameObject disPlayer_UGUI;

    // Start is called before the first frame update
    void Start()
    {
        _cam = this.GetComponent<Camera>();

        Model_Con(-1);

        disPlayer_UGUI.SetActive(false);

        disPlayer_UGUI.transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            mediaPlayer.Control.SetVolume(0);
            disPlayer_UGUI.SetActive(false);

            Main_Con.Instance.bg_AudioSource.Play();
        });
    }


    public void Model_Con(int index)
    {
        for (int i = 0; i < model_Parent.childCount; i++)
        {
            
            model_Parent.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < ui_Parent.childCount - 2; i++)
        {
            
            ui_Parent.GetChild(i).gameObject.SetActive(false);
        }
        
        if (index >= 0)
        {
            model_Parent.GetChild(index).gameObject.SetActive(true);
            ui_Parent.GetChild(index).gameObject.SetActive(true);
            ui_Parent.GetChild(ui_Parent.childCount - 1).gameObject.SetActive(true);
            ui_Parent.gameObject.SetActive(true);
        }
        else
        {
            ui_Parent.GetChild(ui_Parent.childCount - 1).gameObject.SetActive(false);
            ui_Parent.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnMouseClickEvent();
    }

    void OnMouseClickEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {//判断是否是点击事件
            if (disPlayer_UGUI.activeSelf||ui_Parent.gameObject.activeSelf)
                return;
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("CiQi"))
                {
                    Debug.Log(hitInfo.collider.gameObject.name);

                    Model_Con(int.Parse(hitInfo.collider.gameObject.name));
                }
                else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Video"))
                {
                    Debug.Log(hitInfo.collider.gameObject.name);

                    Main_Con.Instance.bg_AudioSource.Pause();
                    mediaPlayer.Control.Rewind();
                    mediaPlayer.Control.SetVolume(1);
                    disPlayer_UGUI.SetActive(true);
                }

            }
        }
    }
}
