using UnityEngine;
using TMPro;  // 引入 TextMesh Pro 命名空间
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace AI
{
    public enum AIjob
    {
        AIcontroller,
        AICreater,
        AIWriter,
        AIDrawer
    }
    public class ChatGLM : MonoBehaviour
    {
        public string apiKey = "YOUR_API_KEY_HERE";  // 请替换为你的API密钥
        public string model;
        public TMP_InputField userInputField;  // 输入框，使用 TextMeshPro的 InputField
        public TMP_Text chatOutput;  // 显示对话输出，使用 TextMeshPro的 Text
                                     //public TMP_Text Test1;
        private string url = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
        private List<ChatMessage> conversationHistory = new List<ChatMessage>();  // 用于存储对话历史
        public SplitTMPWithOriginalMaterial splitTMPWithOriginalMaterial;
        [Header("Values")]
        [SerializeField] public string system_text = ""; //催眠语句
        [Header("private")]
        [SerializeField] private string messagesJson;
        [Header("是否为重要")]
        [SerializeField] public bool IsImportant;
        public static string OtherSystem;
        public string[] messages;
        public AIjob aIjob;

        // 发送消息
        public void SendMessageToGPT()
        {
            string userMessage = userInputField.text;
            if (!string.IsNullOrEmpty(userMessage))
            {
                StartCoroutine(SendRequest(userMessage));
                userInputField.text = "";  // 清空输入框
            }
        }
        public void Ai_response(string userInput)
        {
            // 调用AI接口进行对话
            StartCoroutine(SendRequest(userInput));
        }
        private void Start()
        {
            OtherSystem = "";
            AIjobsmanage.AIEmpowering(this, aIjob);
            if (IsImportant)
            {
                messages = new string[51];
            }
            else
            {
                messages = new string[11];
            }
            messages[0] = "{\"role\":\"system\",\"content\":\"" + system_text + "\"},";

        }
        private void Update()
        {

        }

        public void AnswerFunc(string Text)
        {
            for (int i = 1; i < messages.Length; i++)
            {
                messages[i] = null;
            }
            StartCoroutine(ShowTextGradually(Text));
            GetComponent<TextToSpeech>().Start_speek(Text);
        }

        // 发送API请求
        IEnumerator SendRequest(string prompt)
        {
            messages[0] = "{\"role\":\"system\",\"content\":\"" + system_text+ OtherSystem + "\"},";
            messagesJson = null;
            if (prompt != "@!")
            {
                messages[OutPut_Max()] = "{\"role\":\"user\",\"content\":\"" + prompt + "\"}";
            }
            for (int i = 0; i < messages.Length; i++)
            {
                messagesJson += messages[i];
            }
            // 创建消息结构

            string jsonBody = "{\"model\":\"" + model + "\",\"messages\":" + "[" + messagesJson + "]" + ",\"tools\":" + "[" + AIFunction.AIFunctionColor() + "]" + ",\"tool_choice\":\"" + "auto" + "\"}";
            Debug.Log(jsonBody);
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + apiKey);

                // 等待请求完成
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    if (www.responseCode == 429)  // 如果收到 429 错误
                    {
                        chatOutput.text = "Error: Too many requests. Retrying in 5 seconds...";
                        yield return new WaitForSeconds(5f);  // 等待5秒后重试
                        yield return SendRequest(prompt);  // 重试请求
                    }
                    else
                    {
                        chatOutput.text = "Error: " + www.error;  // 错误处理
                        GetComponent<TextToSpeech>().IsTalk = false;
                    }
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                    string Text = ParseGPTResponse(responseText);
                    Debug.Log("AI:" + responseText);

                    AIFunction.AiParser(responseText, this);


                    chatOutput.transform.parent.gameObject.SetActive(true);

                    //string speekMessage = RemoveTextInBrackets(Text);//去除括号
                    //string speekMessage_last = RemoveTextAfterDash(speekMessage);//去除括号
                    
                    if (splitTMPWithOriginalMaterial != null)
                    {
                        Debug.Log("字体掉落");
                        splitTMPWithOriginalMaterial.TextDownEffect();
                        
                    }
                    GetComponent<TextToSpeech>().Start_speek(Text);
                    // 逐字显示AI的回复
                    StartCoroutine(ShowTextGradually(Text));

                    messages[OutPut_Max()] = ",{\"role\":\"assistant\",\"content\":\"" + RemoveTextInBrackets(Text) + "\"},";
                    Check_ToExpend();


                }

                // 添加延迟，减少请求频率，避免触发速率限制
                yield return new WaitForSeconds(1f);  // 等待1秒再发送下一个请求
            }
        }
        // 解析API返回的JSON字符串，提取对话内容
        public string ParseGPTResponse(string jsonResponse)
        {
            try
            {
                // 解析JSON字符串为GPTResponse对象
                GPTResponse gptResponse = JsonUtility.FromJson<GPTResponse>(jsonResponse);

                // 检查是否存在有效的回复
                if (gptResponse != null && gptResponse.choices != null && gptResponse.choices.Length > 0)
                {
                    // 返回GPT的回复内容
                    return gptResponse.choices[0].message.content;
                }
                else
                {

                    // 如果没有有效的回复，返回错误信息
                    return "Error: No valid response from GPT.";

                }
            }
            catch (System.Exception ex)
            {
                // 如果解析失败，返回错误信息
                return "Error parsing response: " + ex.Message;
            }
        }
        private int OutPut_Max() //输出最大长度
        {
            int element_count = 0;
            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i] != null)
                {
                    element_count++;
                }
            }
            return element_count;
        }
        private void Check_ToExpend()
        {
            if (OutPut_Max() == messages.Length)
            {
                for (int i = 1; i < messages.Length; i += 2)
                {
                    if (i == messages.Length - 2)
                    {
                        messages[messages.Length - 1] = null;
                        messages[messages.Length - 2] = null;
                    }
                    else
                    {
                        messages[i] = messages[i + 2];
                        messages[i + 1] = messages[i + 3];
                    }
                }
            }
        }

        public void ShowText_(string text)
        {
            StartCoroutine(ShowTextGradually(text));
        }
        // 协程逐字显示文本
        IEnumerator ShowTextGradually(string message)
        {
            // 清空输入框
            //userInputField.text = "";
            chatOutput.text = null;

            chatOutput.text += "";  // 在AI回答前加上 "AI: "

            foreach (char c in message)
            {
                chatOutput.text += c;  // 添加一个字符
                yield return new WaitForSeconds(0.05f);  // 每个字符之间的延迟时间，0.05秒可以调整
            }

            chatOutput.text += "\n";  // 完成后换行
        }

        // 方法：去除中括号内的内容
        string RemoveTextInBrackets(string text)
        {
            // 用来存放处理后的文本
            string result = text;
            int startIndex = result.IndexOf('{'); // 查找第一个中括号的位置

            while (startIndex != -1)
            {
                int endIndex = result.IndexOf('}', startIndex); // 查找闭合中括号的位置

                if (endIndex != -1)
                {
                    // 截取中括号内的内容并删除
                    result = result.Remove(startIndex, endIndex - startIndex + 1);
                    startIndex = result.IndexOf('{', startIndex); // 继续查找下一个中括号
                }
                else
                {
                    break; // 如果没有找到闭合的中括号，跳出循环
                }
            }

            return result;
        }

        // 方法：删除每行中的"--"及其后面的内容
        string RemoveTextAfterDash(string text)
        {
            int dashIndex = text.IndexOf("--");  // 查找"--"的位置

            if (dashIndex != -1)
            {
                // 截取"--"前面的文本
                return text.Substring(0, dashIndex).Trim();
            }

            return text;  // 如果没有"--"，返回原文本
        }


        public void GenerateText(string text)
        {
            StartCoroutine(ShowTextGradually(text));
        }

        [System.Serializable]
        public class ChatMessage
        {
            public string role;
            public string content;

            public ChatMessage(string role, string content)
            {
                this.role = role;
                this.content = content;
            }
        }
        [System.Serializable]
        public class GPTResponse
        {
            public Choice[] choices;

            [System.Serializable]
            public class Choice
            {
                public Message message;

                [System.Serializable]
                public class Message
                {
                    public string role;
                    public string content;
                }
            }
        }


    }
}

