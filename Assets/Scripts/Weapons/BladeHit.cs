using UnityEngine;
using TopDownSurvivor.Enemies;

namespace TopDownSurvivor.Weapons
{
    /// <summary>
    /// 环绕刀刃的单片刀：碰到敌人造成伤害。
    /// </summary>
    public class BladeHit : MonoBehaviour
    {
        private int _damage = 1;

        public void Init(int damage)
        {
            _damage = damage;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }
        }
    }
}
