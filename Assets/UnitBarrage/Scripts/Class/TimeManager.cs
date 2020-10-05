using UnityEngine;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 時間の管理を行うクラス
    /// </summary>
    public class TimeManager
    {
        #region Public Variable
        /// <summary>
        /// ゲームの設定データをあつめたデータ群
        /// </summary>
        public GameSettings GameSetting { get; private set; }
        /// <summary>
        /// シーンを読み込んでからの経過時間を記録しておくキャッシュ変数
        /// </summary>
        public float LastTimeSinceLevelLoad { get; private set; }
        #endregion


        #region Public Variable
        public TimeManager(GameSettings gameSetting)
        {
            GameSetting = gameSetting;
            LastTimeSinceLevelLoad = 0;
        }

        /// <summary>
        /// 待ち時間を終えたかどうかを確認するために外部から呼ぶ関数
        /// </summary>
        /// <returns>終わってるかどうか</returns>
        public bool CheckWaitTime()
        {
            return Time.timeSinceLevelLoad > (LastTimeSinceLevelLoad + GameSetting.WaitInterval);
        }

        /// <summary>
        /// 閾値のキャッシュを更新するために外部から呼ぶ関数
        /// </summary>
        public void UpdateCacheTime()
        {
            LastTimeSinceLevelLoad = Time.timeSinceLevelLoad;
        }
        #endregion
    }
}