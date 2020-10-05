using UnityEngine;

namespace hatuxes.UnitBarrage
{
    [CreateAssetMenu(fileName = "BossSettings", menuName = "Settings/BossSetting", order = 1)]
    public class BossSettings : ScriptableObject
    {
        /// <summary>
        /// 初期のボスのHP
        /// </summary>
        public float InitialBossHP;
        /// <summary>
        /// 初期のボスのHPバーの色
        /// </summary>
        public Color NormalBossHPBarColor;
        /// <summary>
        /// 最後のボスのHP
        /// </summary>
        public float ThirdBossHP;
        /// <summary>
        /// 最後のボスのHPバーの色
        /// </summary>
        public Color AttackBossHPBarColor;

        /// <summary>
        /// 第一攻撃段階でのボスの弾のショットレート
        /// </summary>
        public float Phase1Rate;
        /// <summary>
        /// 第二攻撃段階でのボスの弾のショットレート
        /// </summary>
        public float Phase2Rate;
        /// <summary>
        /// 第三攻撃段階でのボスの弾のショットレート
        /// </summary>
        public float Phase3Rate;

        /// <summary>
        /// 最大ボス攻撃段階値
        /// </summary>
        public uint MaxBossPhaseCount;
    }
}