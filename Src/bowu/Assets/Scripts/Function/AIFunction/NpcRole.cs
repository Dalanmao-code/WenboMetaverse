using AI;
using JetBrains.Annotations;
using Pathfinding;

//using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NpcRole : MonoBehaviour
{
    private static NpcRole _instance;

    // 私有构造函数防止外部实例化
    private NpcRole() { }

    public static NpcRole Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NpcRole();
            }
            return _instance;
        }
    }
    [Header("IK类")]
    [SerializeField]public bool IsEnabled = true;
    [SerializeField]public Animator animator;
    [SerializeField]public Transform lookat;
    [Header("Player")]
    [SerializeField]public Transform Player;
    
    [Header("脚本类")]
    [SerializeField] public AudioRecorder audioRecorder;
    [SerializeField] public ChatGLM chat;
    [Header("A*寻路类")]
    public Seeker seeker;
    public List<Vector3> aimPoint;
    public Transform ThisObj;
    public Vector3 Target;
    [SerializeField] public float Speed = 1f;

    public string Text_; 
    [Header("搜索范围")]
    [SerializeField]public float radius = 2f; // 圆的半径
    [SerializeField]public int segments = 32; // 圆的线段数量（越多越平滑）
    private bool PlayerEnter = true;
    private bool IsOne = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        if (TryGetComponent<Animator>(out Animator animator_))
        {
            animator = animator_;
        }
        seeker.pathCallback += OnPathComplete;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(this.transform.position, Player.position)<= radius&& PlayerEnter)
        {
            PlayerEnter = false;
            audioRecorder.StartIni();

            if (audioRecorder != null)
            {
                audioRecorder.isEnabled = true;
                if (IsOne)
                {
                    StartCoroutine(TimeToSleep(Text_, 6f));
                    IsOne = false;
                }
            }
        }
        if(Vector3.Distance(this.transform.position, Player.position) > radius&&!PlayerEnter)
        {
            PlayerEnter = true;
            audioRecorder.EndIni();

            if (audioRecorder != null)
            {
                audioRecorder.isEnabled = false;
                
            }
        }

        
        
    }
    float Timer = 0;
    private void FixedUpdate()
    {
        if (aimPoint != null && aimPoint.Count != 0)
        {
            animator.SetBool("Walk",true);
            Vector3 dir = (aimPoint[0]- ThisObj.transform.position).normalized;
            
            //ThisObj.transform.LookAt(ThisObj.transform.position + dir);
            Quaternion qua = Quaternion.LookRotation(aimPoint[0] - ThisObj.transform.position);
            ThisObj.transform.rotation = Quaternion.Lerp(ThisObj.transform.rotation, qua, 0.1f);
            ThisObj.transform.position += dir * Time.fixedDeltaTime * Speed;
            if (Vector3.Distance(aimPoint[0], ThisObj.transform.position) <= 0.1f)
            {
                aimPoint.RemoveAt(0);
                
            }

            Vector2 currentXZ = new Vector2(ThisObj.transform.position.x, ThisObj.transform.position.z);
            Vector2 targetXZ = new Vector2(Target.x, Target.z);
            float xzDistance = Vector2.Distance(currentXZ, targetXZ);
            if (xzDistance <= 0.2f)
            {
                Timer = 0.1f;
            }



        }
        if (Timer>0&&Timer<=3f)
        {
            animator.SetBool("Walk", false);
            Timer += Time.fixedDeltaTime;
            if (Timer >= 3)
            {
                Timer = 0;
                Target = new Vector3(0,0,0);
            }
            Debug.Log(Timer);
            // 计算水平方向（忽略高度差）
            Vector3 toPlayer = Player.position - ThisObj.transform.position;
            toPlayer.y = 0; // 关键：只考虑XZ平面

            // 确保方向有效（长度大于0）
            if (toPlayer.magnitude > 0.01f)
            {
                // 计算目标旋转
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer);

                // 平滑旋转（使用Slerp更自然）
                ThisObj.transform.rotation = Quaternion.Slerp(
                    ThisObj.transform.rotation,
                    targetRotation,
                    5f * Time.fixedDeltaTime // 调整这个值改变旋转速度
                );
            }
        }

    }

    void OnPathComplete(Path path)
    {
        aimPoint = new List<Vector3>(path.vectorPath);
    }

    public void FindPathToTarget(int Id)
    {
        Target = Functions.GetCultureBaseById(Id).Target;
        if (Target != new Vector3(0, 0, 0))
        {
            seeker.StartPath(ThisObj.transform.position, Target);
        }
        ChatGLM.OtherSystem = Functions.GetCultureBaseById(Id).Description;
    }

    /// <summary>
    /// 让一段文本Timer时间后消失
    /// </summary>
    /// <param name="text"></param>
    /// <param name="Timer"></param>
    /// <returns></returns>
    public IEnumerator TimeToSleep(string text, float Timer)
    {
        
        chat.chatOutput.transform.parent.gameObject.SetActive(true);
        chat.ShowText_(text);
        yield return new WaitForSeconds(Timer);
        chat.chatOutput.text = null;
        chat.chatOutput.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// npc的Ik动作
    /// </summary>
    /// <param name="layerIndex"></param>
    void OnAnimatorIK(int layerIndex)
    {
        if (IsEnabled)
        {
            animator.SetLookAtWeight(0.5f);
            animator.SetLookAtPosition(lookat.position);
        }
    }
    
    private void OnDrawGizmos()
    {
        // 设置 Gizmos 颜色
        Gizmos.color = Color.blue;

        // 获取物体的位置
        Vector3 center = transform.position;

        // 绘制圆形
        DrawCircle(center, radius, segments);
    }
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        // 计算每个线段的角度
        float angleStep = 360f / segments;

        // 绘制线段
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            // 计算线段的起点和终点
            Vector3 point1 = center + Quaternion.Euler(0, angle1, 0) * Vector3.forward * radius;
            Vector3 point2 = center + Quaternion.Euler(0, angle2, 0) * Vector3.forward * radius;

            // 绘制线段
            Gizmos.DrawLine(point1, point2);
        }
    }
}
