using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FishBaseData", menuName = "ScriptableObject/文化基类", order = 0)]
public class CultureData : SingleScriptableObject<CultureData>
{
    //储存鱼类基础
    //Culture
    [Header("古物数组")]
    public List<CultureBase> CultureDataList;
}

public class SingleScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static string filepath = "ScriptableObject/" + typeof(T).Name;
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<T>(filepath);
            }
            if (instance == null)
            {
                instance = CreateInstance<T>();
            }
            return instance;
        }
    }
}

[Serializable]
public class CultureBase
{
    public int Id;     //ID唯一属性
    public string Name;
    public string SimpleDescription; //简单描述
    public string Description; //描述
    public Sprite CulturalImage; //文物展示图
    public Vector3 Target; //预制体 
    public CultureBase(int id,string name,string simpleDescription, string description,Sprite culturalImage, Vector3 target)
    {
        this.Id = id;
        this.Name = name;
        this.SimpleDescription = simpleDescription;
        this.Description = description;
        this.CulturalImage = culturalImage;
        this.Target = target;
    }
    public CultureBase() { }
}

/// <summary>
/// 获取文物列表By Id
/// </summary>
public class Functions
{
    public static CultureBase GetCultureBaseById(int Id)
    {
        return CultureData.Instance.CultureDataList.Find(x => x.Id == Id);
    }
}