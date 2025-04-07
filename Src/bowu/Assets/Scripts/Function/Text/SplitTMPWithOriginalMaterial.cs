using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Xml;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class SplitTMPWithOriginalMaterial : MonoBehaviour
{
    public TMP_Text tmpText; // Ŀ�� TMP �ı����
    public bool hideOriginalText = true; // �Ƿ�����ԭʼ�ı�
    public TMP_FontAsset fontAsset; // ���TMPro������Դ

    public Transform target;  // ��ת����
    public Transform player;  // ��Ҷ���
    public float rotationSpeed = 5f;  // ��ת�ٶ�

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
        // �������嵽��ҵķ���
        Vector3 directionToPlayer = player.position - target.position;
        directionToPlayer.y = 0;  // ����Y�ᣬʹ��תֻ������ˮƽ����

        // ����Ŀ����ת
        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer);

        // ƽ������ת���壬ʹ�����������
        target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    

    public void TextDownEffect()
    {
        fontAsset.ClearFontAssetData();
        if (tmpText == null)
        {
            Debug.LogError("δ���� TMP �ı������");
            return;
        }

        // ǿ�Ƹ��� TMP �����Ի�ȡ��������
        tmpText.ForceMeshUpdate(true);
        tmpText.UpdateGeometry(tmpText.mesh, 0);
        

        // ��ȡ TMP ���ı���Ϣ
        TMP_TextInfo textInfo = tmpText.textInfo;

        // ����ÿ���ַ�
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // �����հ��ַ�����ո�
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // ȷ���������ᳬ����Χ
            if (vertexIndex + 3 >= textInfo.meshInfo[materialIndex].vertices.Length)
            {
                Debug.LogError("��������������Χ");
                continue;
            }

            // ��ȡ�ַ��Ķ�������
            Vector3[] vertices = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                Vector3 vertex = textInfo.meshInfo[materialIndex].vertices[vertexIndex + j];
                vertices[j] = tmpText.transform.TransformPoint(vertex);
            }

            // �����µ�����
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            // ������������������ֹȱʧС������
            mesh.triangles = new int[] { 0, 1, 3, 0, 3, 2, 0, 1, 2, 2, 1, 3 };

            // ��ȡ UV ����
            Vector2[] uvs = new Vector2[4];
            for (int j = 0; j < 4; j++)
            {
                uvs[j] = textInfo.meshInfo[materialIndex].uvs0[vertexIndex + j];
            }
            mesh.uv = uvs;

            // ��ȡ��ɫ����
            Color32[] colors = new Color32[4];
            for (int j = 0; j < 4; j++)
            {
                colors[j] = textInfo.meshInfo[materialIndex].colors32[vertexIndex + j];
            }
            mesh.colors32 = colors;

            // �����µ� GameObject
            GameObject charObj = new GameObject("Char_" + i);
            charObj.transform.SetParent(tmpText.transform.parent.parent);
            //charObj.transform.localPosition = new Vector3(0, 0, 0);
            Destroy(charObj, 3f);

            // ��� MeshFilter �� MeshRenderer
            MeshFilter meshFilter = charObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = charObj.AddComponent<MeshRenderer>();
            meshRenderer.material = tmpText.fontMaterial;

            // ��� BoxCollider
            BoxCollider boxCollider = charObj.AddComponent<BoxCollider>();

            // ���� BoxCollider �ĳߴ�
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.02f);

            // ��� Rigidbody ���
            Rigidbody rb = charObj.AddComponent<Rigidbody>();

            float randcounty = 0;
            do
            {
                // ���� -0.8 �� 0.8 ֮��������
                randcounty = Random.Range(-0.8f, 0.8f);
            }
            while (randcounty >= -0.3f && randcounty <= 0.3f); // �ų� -0.2 �� 0.2 ֮���ֵ
            float randcountz = 0;
            do
            {
                // ���� -0.8 �� 0.8 ֮��������
                randcountz = Random.Range(-0.8f, 0.8f);
            }
            while (randcountz >= -0.3f && randcountz <= 0.3f); // �ų� -0.2 �� 0.2 ֮���ֵ

            // ����һ������������
            Vector3 randomForce = new Vector3(
                0,
                randcounty, // y�������
                randcountz  // z�������
            );

            // ʩ�������
            rb.AddForce(randomForce, ForceMode.Impulse);
            

        }
        tmpText.text = null;

    }
}
