using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Wit.BaiduAip.Speech
{
    [Serializable]
    public class AsrResponse
    {
        public int err_no;
        public string err_msg;
        public string sn;
        public string[] result;
    }

    public class Asr : Base
    {
        private const string UrlAsr = "https://vop.baidu.com/server_api";

        public Asr(string apiKey, string secretKey) : base(apiKey, secretKey)
        {
        }

        public IEnumerator Recognize(byte[] data, Action<AsrResponse> callback)
        {
			yield return PreAction ();

			if (tokenFetchStatus == Base.TokenFetchStatus.Failed) {
				Debug.LogError("Token fetched failed, please check your APIKey and SecretKey");
				yield break;
			}

            var uri = string.Format("{0}?dev_pid=1537&cuid={1}&token={2}", UrlAsr, SystemInfo.deviceUniqueIdentifier, Token);

            var form = new WWWForm();
            form.AddBinaryData("audio", data);
            var www = UnityWebRequest.Post(uri, form);
            www.SetRequestHeader("Content-Type", "audio/pcm;rate=16000");
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log("[WitBaiduAip]"+www.downloadHandler.text);
                callback(JsonUtility.FromJson<AsrResponse>(www.downloadHandler.text));
            }
            else
                Debug.LogError(www.error);

        }

        /// <summary>
        /// 将Unity的AudioClip数据转化为PCM格式16bit数据
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static byte[] ConvertAudioClipToPCM16(AudioClip clip, float Current_data, float maxRecordingTime)
        {

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
            var samples_int16 = new short[samples.Length];

            for (var index = 0; index < samples.Length; index++)
            {
                var f = samples[index];
                samples_int16[index] = (short) (f * short.MaxValue);
            }

            var byteArray = new byte[samples_int16.Length * 2];
            Buffer.BlockCopy(samples_int16, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }
    }
}