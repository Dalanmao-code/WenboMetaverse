using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace AI
{
    public class AIjobsmanage
    {
       
        public static String AIkeyZHIPU = "63f24bbc56724c909df25ef491eb77d7.1S0WRM9FhXz9NhXx";
        public static String AImodleZHIPU = "glm-4-air";

        public static void AIEmpowering(ChatGLM chatGLM, AIjob aIjob)
        {
            
            if(chatGLM == null)
            {
                return;
            }
            if(aIjob == AIjob.AIcontroller)
            {
                chatGLM.apiKey = AIjobsmanage.AIkeyZHIPU;
                chatGLM.model = AIjobsmanage.AImodleZHIPU;
                chatGLM.IsImportant = true;
            }
            if (aIjob == AIjob.AICreater)
            {
                
            }
        }
    }
    public class AIFunction
    {

        // Update is called once per frame

        // 创建 JSON 字符串
        public static string AIFunctionColor()
        {
            // 创建工具和参数数据
            Tool tool1 = new Tool
            {
                type = "function",
                function = new FunctionDetails
                {
                    name = "ChangeClothes",
                    description = "打开换装面板，如果用户有换你服装的意向，就调用这个函数，所以这个函数不需要任何参数",
                    parameters = new Parameters
                    {
                        type = "object",
                        properties = new Property
                        {
                            
                        },
                        required = new string[] {  }
                    }
                }
            };
            Tool tool2 = new Tool
            {
                type = "function",
                function = new FunctionDetails
                {
                    name = "GoToExhibits",
                    description = "前往参观对应的展品",
                    parameters = new Parameters
                    {
                        type = "object",
                        properties = new Property
                        {
                            number = new Parameter { description = "前往哪个展品参观，从1到10，总共十个展品", type = "string" },
                        },
                        required = new string[] { "number" }
                    }
                }
            };

            // 将对象序列化为JSON
            string json1 = JsonUtility.ToJson(tool1, true);
            string json2 = JsonUtility.ToJson(tool2, true);
            return json1 + "," + json2;
        }

        // 函数响应的结构
        [Serializable]
        public class Tool
        {
            public string type;
            public FunctionDetails function;  // 修改后的FunctionDetails类
        }

        [Serializable]
        public class FunctionDetails
        {
            public string name;
            public string description;
            public Parameters parameters;
        }

        [Serializable]
        public class Parameters
        {
            public string type;
            public Property properties;
            public string[] required;
        }

        [Serializable]
        public class Property
        {
            public Parameter number;
            public Parameter color;
            public Parameter size;
        }

        [Serializable]
        public class Parameter
        {
            public string type;
            public string description;
        }
        public static void AiParser(string jsonData, ChatGLM chatGLM)
        {

            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogError("Invalid JSON data: JSON string is null or empty.");
                return;  // 退出方法，避免继续解析
            }

            AIResponse response = null;

            try
            {
                // 尝试解析 JSON 数据
                response = JsonUtility.FromJson<AIResponse>(jsonData);

                if (response == null || response.choices == null || response.choices.Length == 0)
                {
                    Debug.LogError("Failed to parse AIResponse or missing 'choices' data.");
                    return;  // 如果解析失败或数据缺失，退出方法
                }

                // 获取第一个函数调用
                var toolCall = response.choices[0].message?.tool_calls?[0];

                if (toolCall == null || string.IsNullOrEmpty(toolCall.function?.name) || string.IsNullOrEmpty(toolCall.function?.arguments))
                {
                    return;  // 如果没有函数名或参数数据，退出方法
                }

                string functionName = toolCall.function.name;
                string functionArgs = toolCall.function.arguments;
                string toolCallId = toolCall.id;

                Debug.Log("Tool Call ID: " + toolCallId);

                // 调用函数
                CallFunction(functionName, functionArgs, toolCallId, chatGLM);
            }
            catch (Exception e)
            {
                // 捕获 JSON 解析异常
                Debug.LogError("Failed to parse JSON: " + e.Message);
                return;  // 如果发生异常，退出方法
            }
        }

        // 根据函数名调用不同的函数
        public static void CallFunction(string functionName, string args, string id, ChatGLM chatGLM)
        {
            // 解析参数
            FunctionArguments arguments = JsonUtility.FromJson<FunctionArguments>(args);

            if (functionName == "ChangeClothes")
            {
                ChangeClothes();
                chatGLM.AnswerFunc("_好的，这就打开换装面板。");
            }
            else if (functionName == "GoToExhibits")
            {
                GoToExhibits(arguments.number);
                chatGLM.AnswerFunc("_这就带您去参观第"+ arguments.number + "号展品：" + Functions.GetCultureBaseById(int.Parse(arguments.number)-1).Name);
            }
        }
        public static void ChangeClothes()
        {
            TrappingsIni.TrappingUI.SetActive(true);
        }

        public static void GoToExhibits(string number)
        {
            NpcRole.Instance.FindPathToTarget(int.Parse(number)-1);
        }



        // 函数响应的结构
        [Serializable]
        public class AIResponse
        {
            public Choice[] choices;
        }

        [Serializable]
        public class Choice
        {
            public Message message;
        }

        [Serializable]
        public class Message
        {
            public ToolCall[] tool_calls;
        }

        [Serializable]
        public class ToolCall
        {
            public string id; // 添加 id 字段
            public FunctionCall function; // 修改为 FunctionCall 类
        }

        [Serializable]
        public class FunctionCall
        {
            public string name;
            public string arguments;
        }

        [Serializable]
        public class FunctionArguments
        {
            public string number;
            public string color;
            public string size;
        }
    }
}
