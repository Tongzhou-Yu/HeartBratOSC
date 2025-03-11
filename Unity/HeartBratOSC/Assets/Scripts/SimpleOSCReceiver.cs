using UnityEngine;        // 告诉Unity我们要用它的功能
using extOSC;           // 告诉Unity我们要用OSC插件的功能

// 这行告诉Unity：这个脚本一定要和OSCReceiver组件一起使用，就像一个小朋友一定要有大人陪着一样
[RequireComponent(typeof(OSCReceiver))]
public class SimpleOSCReceiver : MonoBehaviour
{
    // 在Unity界面中显示的设置，就像填表格一样
    [Header("OSC设置")]
    [SerializeField] private string oscAddress = "/test";  // OSC的地址，就像每个小朋友都有自己的名字

    [Header("对象缩放设置")]
    [SerializeField] private Transform targetObject;       // 要控制的物体，就像遥控器要控制的玩具
    [SerializeField] private float minScale = 0.8f;       // 最小的尺寸，物体最小能缩到多小
    [SerializeField] private float maxScale = 1.5f;       // 最大的尺寸，物体最大能放到多大
    [SerializeField] private float smoothSpeed = 5f;      // 变化的速度，就像走路的速度
    [SerializeField] private float defaultScale = 1f;     // 默认的尺寸，就像玩具的原始大小

    // 这些是我们的"秘密小本本"，记录一些重要的信息
    private Vector3 targetScale;                          // 目标大小，就像我们想要变成多大
    private OSCReceiver receiver;                         // 接收器，就像收音机一样接收信号
    private OSCBind receiverBind;                        // 接收器的设置，就像收音机调到哪个频道
    private bool isScalingDown = true;                   // 记录现在是要变大还是要变小

    // 游戏开始时要做的事情，就像上学第一天要做的准备工作
    private void Start()
    {
        // 如果没有选择要控制的物体，就控制这个脚本所在的物体
        if (targetObject == null)
            targetObject = transform;

        // 设置初始大小，就像给玩具设置最开始的大小
        targetScale = Vector3.one * defaultScale;
        targetObject.localScale = targetScale;

        // 找到并设置接收器，就像设置收音机
        receiver = GetComponent<OSCReceiver>();
        if (receiver == null)
        {
            Debug.LogError("找不到接收器！就像找不到收音机一样，没法工作了！");
            return;
        }

        // 设置接收器要听哪个频道，就像调收音机的频道
        receiverBind = receiver.Bind(oscAddress, OnOscMessageReceived);

        Debug.Log($"准备好了！正在听{oscAddress}这个频道");
    }

    // 每一帧（每一小段时间）都要做的事情，就像一直看着玩具
    private void Update()
    {
        // 让物体慢慢变到目标大小，就像慢慢走到目标地方
        targetObject.localScale = Vector3.Lerp(
            targetObject.localScale,  // 现在的大小
            targetScale,             // 想要变成的大小
            Time.deltaTime * smoothSpeed  // 变化的速度
        );
    }

    // 当收到OSC消息时要做的事情，就像收到指令时要做什么
    private void OnOscMessageReceived(OSCMessage message)
    {
        // 试着从消息中获取数字，就像拆开一个信封看里面写了什么
        if (message.ToInt(out int value))
        {
            // 根据现在是要变大还是要变小来决定下一步
            float targetValue;
            if (isScalingDown)
            {
                targetValue = minScale;  // 如果现在是要变小，就变到最小
                Debug.Log($"收到消息，要变小了！");
            }
            else
            {
                targetValue = maxScale;  // 如果现在是要变大，就变到最大
                Debug.Log($"收到消息，要变大了！");
            }

            // 设置目标大小，就像告诉玩具要变多大
            targetScale = Vector3.one * targetValue;

            // 切换下一次是变大还是变小，就像玩跷跷板一样上下交替
            isScalingDown = !isScalingDown;
        }
    }

    // 当脚本被关闭时要做的事情，就像放学要收拾书包一样
    private void OnDestroy()
    {
        // 如果有接收器，就要把它关掉，就像关掉收音机
        if (receiver != null && receiverBind != null)
        {
            receiver.Unbind(receiverBind);
        }
    }
}