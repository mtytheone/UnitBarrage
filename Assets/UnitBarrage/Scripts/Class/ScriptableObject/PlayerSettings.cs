using UnityEngine;

namespace hatuxes.UnitBarrage
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/PlayerSetting", order = 0)]
    public class PlayerSettings : ScriptableObject
    {
        /// <summary>
        /// 初期のプレイヤーHP
        /// </summary>
        public uint InitialHP;

        /// <summary>
        /// X軸の左端（0～1）
        /// </summary>
        public float XMinBound;
        /// <summary>
        /// X軸の右端（0～1）
        /// </summary>
        public float XMaxBound;
        /// <summary>
        /// Y軸の下端（0～1）
        /// </summary>
        public float YMinBound;
        /// <summary>
        /// Y軸の上端（0～1）
        /// </summary>
        public float YMaxBound;

        /// <summary>
        /// プレイヤーのショットレート
        /// </summary>
        public float PlayerShotRate;

        /// <summary>
        /// 弾の横間隔
        /// </summary>
        public float BulletDistance;
    }
}