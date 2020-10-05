using Unity.Mathematics;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 色んな定数や定義を保持しておくための静的クラス
    /// </summary>
    public static class Define
    {
        /// <summary>
        /// Z方向単位ベクトル
        /// </summary>
        public static readonly float3 ZAXIS = new float3(0, 0, 1);

        /// <summary>
        /// ゲームの状態に関するEnumの定義
        /// </summary>
        public enum GameStatus
        {
            Title,
            Playing,
            Waiting,
            Pausing,
            End
        }

        /// <summary>
        /// タイトル時のMainCameraのfarclipの値
        /// </summary>
        public const float FARCLIP_TITLE = 10.0f;
        /// <summary>
        /// ゲーム時のMainCameraのfarclipの値
        /// </summary>
        public const float FARCLIP_GAME = 100.0f;
    }
}