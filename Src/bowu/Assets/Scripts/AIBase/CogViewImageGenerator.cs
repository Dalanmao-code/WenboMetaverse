using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CogViewImageGenerator : MonoBehaviour
{
    public string apiKey = "your_api_key_here"; // 替换为你的API密钥
    public string model = "cogview-4"; // 替换为你的模型名称
    public RawImage rawImage; // 用于显示生成的图片
    public TMP_InputField userInputField;

    void Start()
    {
        
    }

    public void CreateImage()
    {
        StartCoroutine(GenerateImage(userInputField.text));
    }

    IEnumerator GenerateImage(string prompt)
    {
        userInputField.text = null;
        rawImage.color = new Color(1, 1, 1, 0);
        // API端点
        string url = "https://open.bigmodel.cn/api/paas/v4/images/generations";

        // 请求体
        string jsonData = "{\"model\": \"" + model + "\", \"prompt\": \"" + prompt + "\"}";

        // 创建UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 发送请求
        yield return request.SendWebRequest();

        // 处理响应
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 解析响应
            string responseJson = request.downloadHandler.text;
            Debug.Log("Response: " + responseJson);

            // 解析JSON
            var response = JsonUtility.FromJson<CogViewResponse>(responseJson);
            if (response.data != null && response.data.Length > 0)
            {
                string imageUrl = response.data[0].url;
                Debug.Log("Image URL: " + imageUrl);

                // 下载图片并显示
                yield return StartCoroutine(DownloadImage(imageUrl));
            }
            else
            {
                Debug.LogError("No image data found in response.");
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
            rawImage.texture = texture;
            while (rawImage.color.a <= 1)
            {
                yield return new WaitForSeconds(0.01f);
                rawImage.color += new Color(0, 0, 0, 0.01f);
            }
        }
        else
        {
            Debug.LogError("Error downloading image: " + imageRequest.error);
        }
    }

    [System.Serializable]
    private class CogViewResponse
    {
        public int created;
        public CogViewImageData[] data;
    }

    [System.Serializable]
    private class CogViewImageData
    {
        public string url;
    }
}