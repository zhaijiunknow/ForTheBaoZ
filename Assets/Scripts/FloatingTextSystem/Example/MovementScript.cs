using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [Tooltip("角色控制器")] public CharacterController characterController;
    private float horizontal;
    private float vertical;

    [Header("移动")]
    [Tooltip("角色行走的速度")] public float walkSpeed = 6f;
    [Tooltip("当前速度")] private float speed;
    [Tooltip("角色移动的方向")] private Vector3 moveDirection;

    void Start()
    {
        speed = walkSpeed;
    }
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        moveDirection = transform.right * horizontal + transform.forward * vertical; // 计算移动方向
        //将该向量从局部坐标系转换为世界坐标系，得到最终的移动方向，效果和上面的一样
        // moveDirection = transform.TransformDirection(new Vector3(h, 0, v));
        moveDirection = moveDirection.normalized; // 归一化移动方向，避免斜向移动速度过快
        characterController.Move(moveDirection * Time.deltaTime * speed);
    }
}