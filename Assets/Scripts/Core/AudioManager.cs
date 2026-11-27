using UnityEngine;
using TopDownSurvivor.Enemies;
using TopDownSurvivor.Player;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// 音效管理（单例）：播放音效，订阅死亡事件自动播放。
    /// 当前用代码生成「哔」声占位，之后可替换为真实音效文件。
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioSource _audioSource;
        private AudioClip _enemyDieClip;
        private AudioClip _eliteDieClip;
        private AudioClip _bossDieClip;
        private AudioClip _playerHurtClip;
        private AudioClip _levelUpClip;
        private AudioClip _pickupClip;

        protected override void Awake()
        {
            base.Awake();

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // 程序化生成占位音效（不同频率的「哔」声）
            _enemyDieClip = CreateBeep(350f, 0.15f);
            _eliteDieClip = CreateBeep(500f, 0.2f);
            _bossDieClip = CreateBeep(700f, 0.3f);
            _playerHurtClip = CreateBeep(400f, 0.15f);
            _levelUpClip = CreateBeep(800f, 0.2f);
            _pickupClip = CreateBeep(600f, 0.05f);
        }

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
            EventBus<PlayerDiedEvent>.Subscribe(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
            EventBus<PlayerDiedEvent>.Unsubscribe(OnPlayerDied);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            if (e.IsBoss)
            {
                PlaySound(_bossDieClip);   // BOSS：低沉
            }
            else if (e.IsElite)
            {
                PlaySound(_eliteDieClip);  // 精英：中音
            }
            else
            {
                PlaySound(_enemyDieClip);  // 普通：高音
            }
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            PlaySound(_playerHurtClip);
        }

        /// <summary>播放「受伤」音效（供 PlayerHealth 直接调用）。</summary>
        public void PlayPlayerHurtSound()
        {
            PlaySound(_playerHurtClip);
        }

        /// <summary>播放「升级」音效（供 PlayerExperience 直接调用）。</summary>
        public void PlayLevelUpSound()
        {
            PlaySound(_levelUpClip);
        }

        /// <summary>播放「拾取经验」音效（供 ExperienceGem 直接调用）。</summary>
        public void PlayPickupSound()
        {
            PlaySound(_pickupClip);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// 生成一段正弦波「哔」声（占位音效）。
        /// </summary>
        private static AudioClip CreateBeep(float frequency, float duration)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate);
            }

            AudioClip clip = AudioClip.Create("Beep", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
