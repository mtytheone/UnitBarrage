using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// ボスに関する処理などをする管理クラス
    /// </summary>
    public class BossManager
    {
        #region Public Variable
        /// <summary>
        /// ボスの設定データをまとめたデータ群
        /// </summary>
        public BossSettings BossSettingData { get; private set; }
        /// <summary>
        /// ボスが何段階目の攻撃なのかを示す変数
        /// </summary>
        public uint BossPhaseCount { get; private set; }
        #endregion


        #region Private Variable
        /// <summary>
        /// 敵弾を生成するSystem
        /// </summary>
        private Boss_BulletShotSystem _bossShotBulletsystem;
        #endregion



        #region Public Function
        public BossManager(BossSettings bossSettings)
        {
            BossSettingData = bossSettings;

            var world = World.DefaultGameObjectInjectionWorld;
            _bossShotBulletsystem = world.GetOrCreateSystem<Boss_BulletShotSystem>();
        }

        /// <summary>
        /// ボスの初期化を行うために外部から呼ぶ関数
        /// </summary>
        public void Initialize()
        {
            BossPhaseCount = 0;
            _bossShotBulletsystem.SetInterval(BossSettingData.Phase1Rate, BossSettingData.Phase2Rate, BossSettingData.Phase3Rate);
        }

        /// <summary>
        /// ボスの攻撃段階が今どの段階かを確認するために外部から呼ぶ関数
        /// </summary>
        /// <param name="value">調べたい攻撃段階</param>
        /// <returns>正解かどうか</returns>
        public bool EqualBossPhaseCount(uint value)
        {
            return BossPhaseCount == value;
        }

        /// <summary>
        /// ボスの攻撃段階を上げるために外部から呼ぶ関数
        /// </summary>
        public void UpdateBossCount()
        {
            BossPhaseCount++;
        }

        /// <summary>
        /// ボスの攻撃段階が最大値を超えてしまっているかどうかを確認するために外部から呼ぶ関数
        /// </summary>
        /// <returns>超えてるかどうか</returns>
        public bool CheckOverCount()
        {
            return BossPhaseCount > BossSettingData.MaxBossPhaseCount;
        }
        #endregion
    }
}