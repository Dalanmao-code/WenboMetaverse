using UnityEngine;
using System.IO;
using System;

public class AudioRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private string microphoneDevice;
    private bool isRecording = false;
    private float[] audioSamples;
    private float volumeThreshold = 0.08f;  // 音量阈值，调整此值决定触发录音的灵敏度
    private float maxRecordingTime = 3f; // 最大录音时长（秒）
    private float currentRecordingTime = 0f;
    private float lastCheckTime = 0f;
    private float checkInterval = 0.05f; // 每 0.05 秒检查一次音量
    private int maxSamplesToCheck = 400; // 0.25秒内的样本数量 (44100 * 0.5 = 22050)

    [Header("计时器")]
    [SerializeField] private float Create_Timer = 0;
    [Header("录音数据")]
    [SerializeField] private float Current_data = 0; //默认为0
    [SerializeField] private float currentVolume =0;
    [Header("开关类")]
    [SerializeField] public bool isEnabled = false;
    [SerializeField] public bool IsOpen_Audio; //是否开启录音
    private bool IsAudio = false;

    private TextToSpeech textToSpeech;

    void Start()
    {
        textToSpeech = GetComponent<TextToSpeech>();
        // 获取麦克风设备
        microphoneDevice = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        Debug.Log(Microphone.devices[0]);
    }

    public void StartIni()
    {
        
        
        if (microphoneDevice != null)
        {
            // 为录音分配一个空的 AudioClip
            recordedClip = Microphone.Start(microphoneDevice, true, 1000, 16000); // 设定为1000秒的录音时长
            audioSamples = new float[recordedClip.samples];
            IsAudio = true;
        }
        else
        {
            Debug.LogError("No microphone found.");
        }
    }

    public void EndIni()
    {
        Create_Timer = 0;
        IsAudio = false;
        Microphone.End(microphoneDevice);
    }

    void Update()
    {
        if (IsAudio)
        {

            Create_Timer += Time.deltaTime;
            // 获取当前音频样本数据

            recordedClip.GetData(audioSamples, Microphone.GetPosition(microphoneDevice));
            lastCheckTime += Time.deltaTime;

            // 检测音量大小，决定是否开始录音
            if (IsOpen_Audio)
                currentVolume = GetCurrentVolume();
        }
          

        // 如果音量大于阈值并且当前未在录音，开始录音
        if (isEnabled && !textToSpeech.IsTalk && currentVolume > volumeThreshold && !isRecording)
        {
            StartRecording();
            Current_data = Create_Timer - 0.5f;

        }

        // 如果在录音中，且录音时间超过最大时间或音量低于阈值，停止录音
        if (isRecording)
        {
            currentRecordingTime += Time.deltaTime;

            if (currentRecordingTime >= maxRecordingTime )
            {
                SaveWav("recorded_audio.wav", recordedClip);
                isRecording = false;
            }
        }
    }

    // 计算当前音量大小
    float GetCurrentVolume()
    {
        float maxVolume = 0f;

        if (lastCheckTime >= checkInterval&& !isRecording)
        {
            // 获取过去 0.25 秒内的音频样本
            int startIdx = Mathf.Max(0, audioSamples.Length - maxSamplesToCheck);
            // 遍历过去 0.25 秒的音频样本，获取最大音量值
            for (int i = startIdx; i < audioSamples.Length; i++)
            {
                maxVolume = Mathf.Max(maxVolume, Mathf.Abs(audioSamples[i]));
            }

            lastCheckTime = 0f;  // 更新最后检查的时间
        }

        return maxVolume;
        
    }

    // 开始录音
    void StartRecording()
    {
        isRecording = true;
        currentRecordingTime = 0f;
        textToSpeech.IsTalk = true;
        Debug.Log("Recording started...");
    }

    // 停止录音
    void StopRecording()
    {
        if (microphoneDevice != null)
        {
            Microphone.End(microphoneDevice);
            isRecording = false;
            Debug.Log("Recording stopped...");
        }
    }

    // 保存音频为 WAV 文件
    void SaveWav(string filename, AudioClip clip)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        // 计算从录音开始后的 3 秒的数据
        int startSample = Mathf.FloorToInt((int)(16000 * Current_data)); 
        int endSample = Mathf.FloorToInt(startSample + 16000 * maxRecordingTime); // 录音3秒后的数据
        int sampleCount = endSample - startSample;

        // 确保不超出范围
        if (endSample > clip.samples)
            endSample = clip.samples;

        // 获取从10秒处开始的样本数据
        float[] samples = new float[sampleCount];
        clip.GetData(samples, startSample);

        

        // 转换为WAV格式并保存
        byte[] wavFile = ConvertAudioClipToWav(clip, samples);

        // 写入文件
        File.WriteAllBytes(filePath, wavFile);
        Debug.Log("WAV file saved at: " + filePath);
        this.gameObject.GetComponent<AsrDemo>().OnClickStopButton(clip, Current_data, maxRecordingTime);
        
        
        
    }

    

    // 将音频数据转换为WAV格式
    byte[] ConvertAudioClipToWav(AudioClip clip, float[] samples)
    {
        int hz = 16000;
        int channels = clip.channels;
        int sampleCount = samples.Length;
        int headerSize = 44;

        // 计算文件大小
        int dataSize = sampleCount * 2;
        int fileSize = headerSize + dataSize;

        // 创建WAV文件的字节数组
        byte[] wavFile = new byte[fileSize];

        // 写入RIFF头
        BitConverter.GetBytes(0x46464952).CopyTo(wavFile, 0); // "RIFF"
        BitConverter.GetBytes(fileSize - 8).CopyTo(wavFile, 4); // file size minus first 8 bytes
        BitConverter.GetBytes(0x45564157).CopyTo(wavFile, 8); // "WAVE"

        // 写入fmt块
        BitConverter.GetBytes(0x20746D66).CopyTo(wavFile, 12); // "fmt " (little-endian)
        BitConverter.GetBytes(16).CopyTo(wavFile, 16); // fmt chunk size
        BitConverter.GetBytes((short)1).CopyTo(wavFile, 20); // PCM format
        BitConverter.GetBytes((short)channels).CopyTo(wavFile, 22); // channels
        BitConverter.GetBytes(hz).CopyTo(wavFile, 24); // sample rate
        BitConverter.GetBytes(hz * channels * 2).CopyTo(wavFile, 28); // byte rate
        BitConverter.GetBytes((short)(channels * 2)).CopyTo(wavFile, 32); // block align
        BitConverter.GetBytes((short)16).CopyTo(wavFile, 34); // bits per sample

        // 写入data块
        BitConverter.GetBytes(0x61746164).CopyTo(wavFile, 36); // "data"
        BitConverter.GetBytes(dataSize).CopyTo(wavFile, 40); // data size

        // 转换音频数据为WAV格式
        int offset = headerSize;
        for (int i = 0; i < sampleCount; i++)
        {
            short sample = (short)(samples[i] * 32767); // 转换为16位音频样本
            BitConverter.GetBytes(sample).CopyTo(wavFile, offset);
            offset += 2;
        }

        return wavFile;
    }
}
