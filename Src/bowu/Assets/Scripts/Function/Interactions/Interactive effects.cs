using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactiveeffects : MonoBehaviour
{
    [Header("储存类")]
    [SerializeField]public MeshRenderer meshRenderer;
    [SerializeField]public Material newMaterial;
    [SerializeField] public Transform LeftHand;
    [SerializeField] public Transform RightHand;
    [Header("是否启用")]
    [SerializeField] public bool IsOpen = false;
    [Header("检测距离")]
    [SerializeField] public float DetectDistance = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        LeftHand = GameObject.FindGameObjectsWithTag("Hand")[1].transform;
        RightHand = GameObject.FindGameObjectsWithTag("Hand")[0].transform;

    }

    // Update is called once per frame
    void Update()
    {

        if (LeftHand!=null&& (Vector3.Distance(RightHand.position, this.transform.position) <= DetectDistance || Vector3.Distance(LeftHand.position, this.transform.position) <= DetectDistance) &&!IsOpen)
        {
            AddEffect(meshRenderer, newMaterial);
            IsOpen = true;
        }
        if ((LeftHand != null && Vector3.Distance(RightHand.position, this.transform.position) >= DetectDistance && Vector3.Distance(LeftHand.position, this.transform.position) >= DetectDistance) &&IsOpen)
        {
            RemoveEffect(meshRenderer);
            IsOpen = false;
        }
        
    }
    public static void AddEffect(MeshRenderer meshRenderer, Material newMaterial)
    {
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.material;
        materials[1] = newMaterial;
        meshRenderer.materials = materials;
    }

    public static void RemoveEffect(MeshRenderer meshRenderer)
    {
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.materials[0];
        materials[1] = null;
        meshRenderer.materials = materials;
    }
}
