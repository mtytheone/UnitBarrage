using UnityEngine;

namespace hatuxes.UnitBarrage
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSetting", order = 1)]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// ゲーム開始時のスコア最大値
        /// </summary>
        public int InitialScore;
        /// <summary>
        /// 毎フレーム時でのスコア減少値
        /// </summary>
        public int DecreaseScoreUnit;

        /// <summary>
        /// 攻撃段階が上がる時の待機時間
        /// </summary>
        public float WaitInterval;

        /// <summary>
        /// PlayerPrefsに保存しているハイスコアを得るためのキー
        /// </summary>
        public string HiScorePrefKey;
    }
}