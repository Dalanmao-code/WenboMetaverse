using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.Rendering;

public class TextToSpeech : MonoBehaviour
{
    public string apiUrl = "http://127.0.0.1:23456/voice/vits?id=4&format=mp3&text=";  // 替换为你的 API 地址
    public AudioSource audioSource;
    public Animator animator;
    [Header("Value")]
    [SerializeField] public bool IsOpen_Ani;
    public GameObject text_;
    public bool IsTalk = false;
    private bool Speaked = false;

    void Start()
    {
        
    }
    
    private void Update()
    {
        if (IsOpen_Ani)
        {
            if (audioSource.isPlaying)
            {
                animator.SetBool("Isspeek", true);
                
            }
            else
            {
                animator.SetBool("Isspeek", false);
                
            }

            
        }


        if (audioSource.isPlaying)
        {
            IsTalk = true;
            Speaked = true;

        }else if (Speaked)
        {
            IsTalk = false;
            Speaked = false;
            if (text_ != null)
                text_.transform.parent.gameObject.SetActive(false);
        }


    }
    
    IEnumerator GetAndPlayMP3(string text)
    {
        if (string.IsNullOrEmpty(text))  // 检查 text 是否为空
        {
            Debug.LogError("Text is empty! Cannot send request.");
            yield break;  // 退出协程
        }

        string encodedText = Uri.EscapeDataString(text);
        // 构造完整的 API 请求 URL
        
        string apiUrl_text = apiUrl + encodedText;

        // 获取 MP3 数据并通过 Unity 自带的方式解码
        AudioClip audioClip = null;
        using (var request = UnityWebRequestMultimedia.GetAudioClip(apiUrl_text, AudioType.MPEG))
        {
         // 等待音频加载
         yield return request.SendWebRequest();

        // 检查请求是否成功
        if (request.result == UnityWebRequest.Result.Success)
        {
              audioClip = DownloadHandlerAudioClip.GetContent(request);
              // 播放音频
              audioSource.clip = audioClip;
              audioSource.Play();
        }
        else
        {
              Debug.LogError("Failed to load MP3: " + request.error);
                //StartCoroutine(TimeToSleep());
                IsTalk = false;
        }
            
    }
}

    public void Start_speek(string text)
    {
        StartCoroutine(GetAndPlayMP3(text));
    }

    public IEnumerator TimeToSleep()
    {
        yield return new WaitForSeconds(3.5f);
        if (text_ != null)
            text_.transform.parent.gameObject.SetActive(false);
    }
}
