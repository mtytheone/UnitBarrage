using UnityEngine;

namespace hatuxes.UnitBarrage
{
    [CreateAssetMenu(fileName = "GSSConst", menuName = "Settings/GSSConst", order = 4)]
    public class GSSConstantString : ScriptableObject
    {
        /// <summary>
        /// 次に来るのはメソッド名だよという意味の定数
        /// </summary>
        public string MethodTag;
        /// <summary>
        /// 次に来るのは処理データのJsonだよという意味の定数
        /// </summary>
        public string PayloadTag;
        /// <summary>
        /// プレイヤー名欄のGSSのヘッダー名
        /// </summary>
        public string PlayerNameTag;
        /// <summary>
        /// スコア欄のGSSのヘッダー名
        /// </summary>
        public string ScoreTag;
        /// <summary>
        /// 次に来るのはユーザーIDだよという意味の定数
        /// </summary>
        public string UserIDTag;
        /// <summary>
        /// 次に来るのはシート名だよという意味の定数
        /// </summary>
        public string SheetNameTag;
        /// <summary>
        /// 次に来るのはデータだよという意味の定数
        /// </summary>
        public string DataTag;
        /// <summary>
        /// GASに書かれているセーブ処理の関数名
        /// </summary>
        public string SaveDataTag;
        /// <summary>
        /// GASに書かれているスコアロード処理の関数名
        /// </summary>
        public string GetDataTag;
        /// <summary>
        /// GASに書かれているユーザーID取得処理の関数名
        /// </summary>
        public string GetUserIDTag;

        /// <summary>
        /// PlayerPrefで取得する際に使うキーの名前
        /// </summary>
        public string PrefDefaultHiScoreKey;
    }
}