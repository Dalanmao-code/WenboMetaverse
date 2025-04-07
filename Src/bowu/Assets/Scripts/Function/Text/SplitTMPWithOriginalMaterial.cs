using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Xml;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class SplitTMPWithOriginalMaterial : MonoBehaviour
{
    public TMP_Text tmpText; // 目标 TMP 文本组件
    public bool hideOriginalText = true; // 是否隐藏原始文本
    public TMP_FontAsset fontAsset; // 你的TMPro字体资源

    public Transform target;  // 旋转对象
    public Transform player;  // 玩家对象
    public float rotationSpeed = 5f;  // 旋转速度

    void Start()
    {
       
        
    }

    private void Update()
    {
        RotationToPlayer();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TextDownEffect();
        }
    }

    
    public void RotationToPlayer()
    {
        // 计算物体到玩家的方向
        Vector3 directionToPlayer = player.position - target.position;
        directionToPlayer.y = 0;  // 忽略Y轴，使旋转只发生在水平面内

        // 计算目标旋转
        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);

        // 平滑地旋转物体，使其逐渐面向玩家
        target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    

    public void TextDownEffect()
    {
        fontAsset.ClearFontAssetData();
        if (tmpText == null)
        {
            Debug.LogError("未分配 TMP 文本组件！");
            return;
        }

        // 强制更新 TMP 网格以获取最新数据
        tmpText.ForceMeshUpdate(true);
        tmpText.UpdateGeometry(tmpText.mesh, 0);
        

        // 获取 TMP 的文本信息
        TMP_TextInfo textInfo = tmpText.textInfo;

        // 遍历每个字符
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // 跳过空白字符（如空格）
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // 确保索引不会超出范围
            if (vertexIndex + 3 >= textInfo.meshInfo[materialIndex].vertices.Length)
            {
                Debug.LogError("顶点索引超出范围");
                continue;
            }

            // 提取字符的顶点数据
            Vector3[] vertices = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                Vector3 vertex = textInfo.meshInfo[materialIndex].vertices[vertexIndex + j];
                vertices[j] = tmpText.transform.TransformPoint(vertex);
            }

            // 创建新的网格
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            // 修正三角形索引，防止缺失小三角形
            mesh.triangles = new int[] { 0, 1, 3, 0, 3, 2, 0, 1, 2, 2, 1, 3 };

            // 提取 UV 数据
            Vector2[] uvs = new Vector2[4];
            for (int j = 0; j < 4; j++)
            {
                uvs[j] = textInfo.meshInfo[materialIndex].uvs0[vertexIndex + j];
            }
            mesh.uv = uvs;

            // 提取颜色数据
            Color32[] colors = new Color32[4];
            for (int j = 0; j < 4; j++)
            {
                colors[j] = textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
            }
            mesh.colors32 = colors;

            // 创建新的 GameObject
            GameObject charObj = new GameObject("Char_" + i);
            charObj.transform.SetParent(tmpText.transform.parent.parent);
            //charObj.transform.localPosition = new Vector3(0, 0, 0);
            Destroy(charObj, 3f);

            // 添加 MeshFilter 和 MeshRenderer
            MeshFilter meshFilter = charObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = charObj.AddComponent<MeshRenderer>();
            meshRenderer.material = tmpText.fontMaterial;

            // 添加 BoxCollider
            BoxCollider boxCollider = charObj.AddComponent<BoxCollider>();

            // 设置 BoxCollider 的尺寸
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.02f);

            // 添加 Rigidbody 组件
            Rigidbody rb = charObj.AddComponent<Rigidbody>();

            float randcounty = 0;
            do
            {
                // 生成 -0.8 到 0.8 之间的随机数
                randcounty = Random.Range(-0.8f, 0.8f);
            }
            while (randcounty >= -0.3f && randcounty <= 0.3f); // 排除 -0.2 到 0.2 之间的值
            float randcountz = 0;
            do
            {
                // 生成 -0.8 到 0.8 之间的随机数
                randcountz = Random.Range(-0.8f, 0.8f);
            }
            while (randcountz >= -0.3f && randcountz <= 0.3f); // 排除 -0.2 到 0.2 之间的值

            // 生成一个随机方向的力
            Vector3 randomForce = new Vector3(
                0,
                randcounty, // y轴随机力
                randcountz  // z轴随机力
            );

            // 施加随机力
            rb.AddForce(randomForce, ForceMode.Impulse);
            

        }
        tmpText.text = null;

    }
}
