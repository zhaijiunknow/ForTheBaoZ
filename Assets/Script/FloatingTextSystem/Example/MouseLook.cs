using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // 鼠标灵敏度
    public float mouseSensitivity = 1000f;
    // 玩家的身体Transform组件，用于旋转
    public Transform playerBody;
    // x轴的旋转角度
    float xRotation = 0f;
    void Start()
    {
        // 锁定光标到屏幕中心，并隐藏光标
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update在每一帧调用
    void Update()
    {
        // 执行自由视角查看功能
        FreeLook();
    }
    // 自由视角查看功能的实现
    void FreeLook()
    {
        // 获取鼠标X轴和Y轴的移动量，乘以灵敏度和时间，得到平滑的移动速率
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //限制旋转角度在-90到90度之间，防止过度翻转
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // 累计x轴上的旋转量
        xRotation -= mouseY;
        // 应用摄像头的x轴旋转
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 应用玩家身体的y轴旋转
        playerBody.Rotate(Vector3.up * mouseX);
    }
}