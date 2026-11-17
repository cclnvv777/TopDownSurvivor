using UnityEngine;

namespace TopDownSurvivor.Player
{
    /// <summary>
    /// 玩家八方向移动：WASD / 方向键，基于 Rigidbody2D 速度驱动。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // 归一化：保证斜向移动与直线移动速度一致（真正的八方向）
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }
           
            _rb.velocity = input * _moveSpeed;
        }

        /// <summary>
        /// 提升移动速度（升级强化用）。
        /// </summary>
        public void IncreaseMoveSpeed(float amount)
        {
            _moveSpeed += amount;
        }
    }
}
