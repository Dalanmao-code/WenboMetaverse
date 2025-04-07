using AI;
using UnityEngine;
using UnityEngine.UI;
using Wit.BaiduAip.Speech;

public class AsrDemo : MonoBehaviour
{
    public string APIKey = "";
    public string SecretKey = "";
    private Asr _asr;
    private ChatGLM simpleChatGPT;
    private TextToSpeech textToSpeech;

    // Microphone is not supported in Webgl
#if !UNITY_WEBGL

    void Start()
    {
        _asr = new Asr(APIKey, SecretKey);
        StartCoroutine(_asr.GetAccessToken());
        simpleChatGPT = GetComponent<ChatGLM>();
        textToSpeech = GetComponent<TextToSpeech>();
    }
    
    

    public void OnClickStopButton(AudioClip clip,float Current_data,float maxRecordingTime)
    {
        
        Debug.Log("[WitBaiduAip demo]end record");
        var data = Asr.ConvertAudioClipToPCM16(clip, Current_data, maxRecordingTime);
        //Debug.Log(data.Length)
        StartCoroutine(_asr.Recognize(data, s =>
        {
            Debug.Log(s.result[0]);
            // 处理转录的文本结果
            if (!string.IsNullOrEmpty(s.result[0]))
            {
                simpleChatGPT.Ai_response(s.result[0]);
                
            }
            else
            {
                Debug.LogError("Received empty response from the server.");
                textToSpeech.IsTalk = false;
            }
        }));
    }
#endif
}