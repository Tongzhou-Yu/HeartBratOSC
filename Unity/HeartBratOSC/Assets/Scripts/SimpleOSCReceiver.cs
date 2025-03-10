using UnityEngine;
using extOSC;

[RequireComponent(typeof(OSCReceiver))]
public class SimpleOSCReceiver : MonoBehaviour
{
    [Header("OSC设置")]
    [SerializeField] private string oscAddress = "/test";  // OSC地址

    [Header("对象缩放设置")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float smoothSpeed = 5f;  // 平滑过渡速度
    [SerializeField] private float defaultScale = 1f; // 默认缩放值

    private Vector3 targetScale;
    private OSCReceiver receiver;
    private OSCBind receiverBind;
    private bool isScalingDown = true; // 用于追踪当前是放大还是缩小

    private void Start()
    {
        if (targetObject == null)
            targetObject = transform;

        // 初始化目标缩放为默认值
        targetScale = Vector3.one * defaultScale;
        targetObject.localScale = targetScale;

        // 获取OSCReceiver组件
        receiver = GetComponent<OSCReceiver>();
        if (receiver == null)
        {
            Debug.LogError("未找到OSCReceiver组件！");
            return;
        }

        // 绑定消息处理
        receiverBind = receiver.Bind(oscAddress, OnOscMessageReceived);

        Debug.Log($"SimpleOSCReceiver已启动，监听地址: {oscAddress}");
    }

    private void Update()
    {
        // 平滑过渡到目标缩放值
        targetObject.localScale = Vector3.Lerp(
            targetObject.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    // OSC消息处理方法
    private void OnOscMessageReceived(OSCMessage message)
    {
        // 尝试获取整数值
        if (message.ToInt(out int value))
        {
            // 根据当前状态决定是放大还是缩小
            float targetValue;
            if (isScalingDown)
            {
                targetValue = minScale;
                Debug.Log($"收到OSC消息 {oscAddress}，执行缩小");
            }
            else
            {
                targetValue = maxScale;
                Debug.Log($"收到OSC消息 {oscAddress}，执行放大");
            }

            // 设置目标缩放
            targetScale = Vector3.one * targetValue;

            // 切换状态
            isScalingDown = !isScalingDown;
        }
    }

    private void OnDestroy()
    {
        if (receiver != null && receiverBind != null)
        {
            receiver.Unbind(receiverBind);
        }
    }
}