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

        // ���� JSON �ַ���
        public static string AIFunctionColor()
        {
            // �������ߺͲ�������
            Tool tool1 = new Tool
            {
                type = "function",
                function = new FunctionDetails
                {
                    name = "ChangeClothes",
                    description = "�򿪻�װ��壬����û��л����װ�����򣬾͵���������������������������Ҫ�κβ���",
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
                    description = "ǰ���ι۶�Ӧ��չƷ",
                    parameters = new Parameters
                    {
                        type = "object",
                        properties = new Property
                        {
                            number = new Parameter { description = "ǰ���ĸ�չƷ�ιۣ���1��10���ܹ�ʮ��չƷ", type = "string" },
                        },
                        required = new string[] { "number" }
                    }
                }
            };

            // ���������л�ΪJSON
            string json1 = JsonUtility.ToJson(tool1, true);
            string json2 = JsonUtility.ToJson(tool2, true);
            return json1 + "," + json2;
        }

        // ������Ӧ�Ľṹ
        [Serializable]
        public class Tool
        {
            public string type;
            public FunctionDetails function;  // �޸ĺ��FunctionDetails��
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
                return;  // �˳������������������
            }

            AIResponse response = null;

            try
            {
                // ���Խ��� JSON ����
                response = JsonUtility.FromJson<AIResponse>(jsonData);

                if (response == null || response.choices == null || response.choices.Length == 0)
                {
                    Debug.LogError("Failed to parse AIResponse or missing 'choices' data.");
                    return;  // �������ʧ�ܻ�����ȱʧ���˳�����
                }

                // ��ȡ��һ����������
                var toolCall = response.choices[0].message?.tool_calls?[0];

                if (toolCall == null || string.IsNullOrEmpty(toolCall.function?.name) || string.IsNullOrEmpty(toolCall.function?.arguments))
                {
                    return;  // ���û�к�������������ݣ��˳�����
                }

                string functionName = toolCall.function.name;
                string functionArgs = toolCall.function.arguments;
                string toolCallId = toolCall.id;

                Debug.Log("Tool Call ID: " + toolCallId);

                // ���ú���
                CallFunction(functionName, functionArgs, toolCallId, chatGLM);
            }
            catch (Exception e)
            {
                // ���� JSON �����쳣
                Debug.LogError("Failed to parse JSON: " + e.Message);
                return;  // ��������쳣���˳�����
            }
        }

        // ���ݺ��������ò�ͬ�ĺ���
        public static void CallFunction(string functionName, string args, string id, ChatGLM chatGLM)
        {
            // ��������
            FunctionArguments arguments = JsonUtility.FromJson<FunctionArguments>(args);

            if (functionName == "ChangeClothes")
            {
                ChangeClothes();
                chatGLM.AnswerFunc("_�õģ���ʹ򿪻�װ��塣");
            }
            else if (functionName == "GoToExhibits")
            {
                GoToExhibits(arguments.number);
                chatGLM.AnswerFunc("_��ʹ���ȥ�ι۵�"+ arguments.number + "��չƷ��" + Functions.GetCultureBaseById(int.Parse(arguments.number)-1).Name);
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



        // ������Ӧ�Ľṹ
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
            public string id; // ��� id �ֶ�
            public FunctionCall function; // �޸�Ϊ FunctionCall ��
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
