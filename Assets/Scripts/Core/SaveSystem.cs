using UnityEngine;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// 存档系统：用 PlayerPrefs 保存最高存活时间。
    /// </summary>
    public static class SaveSystem
    {
        private const string BestTimeKey = "BestSurviveTime";

        /// <summary>
        /// 读取最高存活时间（没有记录时返回 0）。
        /// </summary>
        public static float LoadBestTime()
        {
            return PlayerPrefs.GetFloat(BestTimeKey, 0f);
        }

        /// <summary>
        /// 保存最高存活时间（只在新纪录更高时才保存）。
        /// </summary>
        public static void SaveBestTime(float time)
        {
            if (time > LoadBestTime())
            {
                PlayerPrefs.SetFloat(BestTimeKey, time);
                PlayerPrefs.Save();
            }
        }
    }
}
