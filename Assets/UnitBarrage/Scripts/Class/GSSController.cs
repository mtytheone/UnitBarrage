using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using TMPro;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// GoogleSpreadSheetに送受信するために必要な準備とその実行を行うクラス
    /// </summary>
    public class GSSController
    {
        #region Public Variable
        /// <summary>
        /// プレイヤーネーム（GSSに登録する名前）
        /// </summary>
        public string PlayerName { get; private set; }
        #endregion


        #region Private Variable
        /// <summary>
        /// ゲーム全般の設定データ
        /// </summary>
        private GameSettings _gameSettings;
        /// <summary>
        /// GSSとの接続を行うために必要な設定データ
        /// </summary>
        private GSSConnectSettings _gssSettings;
        /// <summary>
        /// GSSと接続する際に必要な文字列定数データ
        /// </summary>
        private GSSConstantString _gssConstSettings;
        /// <summary>
        /// GSSからデータを受信するための本体クラス
        /// </summary>
        private GSSGetProcess _getDataHandler;
        /// <summary>
        /// GSSにデータを送信するための本体クラス
        /// </summary>
        private GSSSendProcess _sendDataHandler;
        /// <summary>
        /// ユーザーID（個人識別用）
        /// </summary>
        private string _userID;
        /// <summary>
        /// GSS上にあるユーザーIDたち
        /// </summary>
        private List<string> _getUserIDs = new List<string>();
        /// <summary>
        /// オンラインスコア表示キャンバスにある1位～5位までの反映用Text
        /// </summary>
        private TextMeshProUGUI[] _rankingTexts;
        /// <summary>
        /// 自分のスコアを強調するために変える色
        /// </summary>
        private readonly Color _yourNameColor = new Color(1.0f, 1.0f, 0.5235849f);
        #endregion


        #region Public Function
        public GSSController(GameSettings gameSettings, GSSConnectSettings gssSettings, GSSConstantString gssConst, TextMeshProUGUI[] texts)
        {
            _gameSettings = gameSettings;
            _gssSettings = gssSettings;
            _gssSettings = gssSettings;
            _gssConstSettings = gssConst;
            _rankingTexts = texts;
        }

        /// <summary>
        /// プレイヤーネームを登録しておくために外部から呼ぶ関数
        /// </summary>
        /// <param name="name">プレイヤーネーム</param>
        public void SetPlayerName(string name)
        {
            PlayerName = name;
        }

        /// <summary>
        /// データをGSSに送信するために外部から呼ぶ関数
        /// </summary>
        public IEnumerator SendData()
        {
            // ハイスコア更新
            GameManager.Instance.UpdateNewHiScore();

            if (PlayerName == null || PlayerName.Equals(""))
            {
                PlayerName = "NoName";
            }

            var rawData = new Dictionary<string, object>();
            int scoreData = PlayerPrefs.GetInt(_gameSettings.HiScorePrefKey);
            rawData.Add(_gssConstSettings.PlayerNameTag, PlayerName);
            rawData.Add(_gssConstSettings.ScoreTag, scoreData);

            yield return SendDataAsync(rawData, _gssSettings.SheetName);
        }

        /// <summary>
        /// オンラインスコアランキングをGSSから得るために外部から呼ぶ関数
        /// </summary>
        public IEnumerator GetData()
        {
            var keys = new string[] { _gssConstSettings.ScoreTag };

            return GetDataAsync(keys, PrintResults, _gssSettings.SheetName);
        }
        #endregion



        #region Private Function
        /// <summary>
        /// GSSにデータを送信するために必要な処理をしている関数
        /// </summary>
        /// <param name="rawdata">今回のプレイヤーネームとスコアデータ</param>
        /// <param name="sheetName">GSSのシート名</param>
        private IEnumerator SendDataAsync(Dictionary<string, object> rawdata, string sheetName = null)
        {
            var sendData = new Dictionary<string, object>();
            yield return GetUserID();
            sendData[_gssConstSettings.UserIDTag] = _userID;
            sendData[_gssConstSettings.SheetNameTag] = sheetName ?? "シート1";
            sendData[_gssConstSettings.DataTag] = rawdata;

            yield return SendRequestAsync(_gssConstSettings.SaveDataTag, sendData, null);
        }

        /// <summary>
        /// GSSに実際にデータを送信する関数
        /// </summary>
        /// <param name="processName">なんの処理をさせたいかをGASに認識してもらうための定数（今回はセーブであることを記した文字列）</param>
        /// <param name="sendData">生データを送信する形式にしたデータ群</param>
        /// <param name="responseHandler">送信完了時に行いたい処理</param>
        private CustomYieldInstruction SendRequestAsync(string processName, IDictionary<string, object> sendData, Action<object> responseHandler)
        {
            if (_sendDataHandler == null)
            {
                _sendDataHandler = new GSSSendProcess(_gssSettings.URL, _gssConstSettings);
            }

            return _sendDataHandler.SendDataProcessAsync(GameManager.Instance, processName, sendData, response => responseHandler?.Invoke(response));
        }

        /// <summary>
        /// GSSからオンラインスコアを得るために必要な処理を送信する準備をする関数
        /// </summary>
        /// <param name="keys">受信したいデータのヘッダー</param>
        /// <param name="responseHandler">受信完了時に行いたい処理</param>
        /// <param name="sheetName">GSSのシート名</param>
        private IEnumerator GetDataAsync(IList<string> keys, Action<IList<object>> responseHandler, string sheetName = null)
        {
            var storeData = new Dictionary<string, object>();
            yield return GetUserID();
            storeData[_gssConstSettings.UserIDTag] = _userID;
            storeData[_gssConstSettings.SheetNameTag] = sheetName ?? "シート1";
            storeData[_gssConstSettings.DataTag] = keys;

            yield return GetRequestAsync(_gssConstSettings.GetDataTag, storeData, response => responseHandler?.Invoke(ParseResponse(response)));
        }

        /// <summary>
        /// GSSに実際に受信要求をする関数
        /// </summary>
        /// <param name="processName">何の処理をしたいかをGASに認識してもらうための定数（今回はデータを受信することを記した文字列）</param>
        /// <param name="sendData">受信に必要なデータ</param>
        /// <param name="responseHandler">受信完了時に行いたい処理</param>
        private CustomYieldInstruction GetRequestAsync(string processName, IDictionary<string, object> sendData, Action<object> responseHandler)
        {
            if (_getDataHandler == null)
            {
                _getDataHandler = new GSSGetProcess(_gssSettings.URL, _gssConstSettings);
            }

            return _getDataHandler.GetDataProcessAsync(GameManager.Instance, processName, sendData, response => responseHandler?.Invoke(response));
        }

        /// <summary>
        /// 受信したデータを解析してそのデータ型に直す関数
        /// 今回は、受信したobject型の変数をList（配列）型に直す
        /// </summary>
        /// <param name="response">受信データ</param>
        /// <returns>List型に直した受信データ</returns>
        private IList<object> ParseResponse(object response)
        {
            var resultList = new List<object>();
            foreach (var result in (IList)response)
            {
                resultList.Add(result);
            }

            return resultList;
        }

        /// <summary>
        /// GSSからデータを受信した際に行う処理を記述した関数
        /// 今回は、ランキングテキストにデータを反映する処理
        /// </summary>
        /// <param name="results">受信したものを解析したもの</param>
        private void PrintResults(IList<object> results)
        {
            _rankingTexts[0].text = string.Format("{0} {1}pt", results[0] ?? "", results[1]);
            _rankingTexts[1].text = string.Format("{0} {1}pt", results[2] ?? "", results[3]);
            _rankingTexts[2].text = string.Format("{0} {1}pt", results[4] ?? "", results[5]);
            _rankingTexts[3].text = string.Format("{0} {1}pt", results[6] ?? "", results[7]);
            _rankingTexts[4].text = string.Format("{0} {1}pt", results[8] ?? "", results[9]);
            _rankingTexts[5].text = string.Format("{0} {1}pt", PlayerName ?? "", GameManager.Instance.GetTotalScore());

            for (int i = 0; i < results.Count / 2.0f; i++)
            {
                if (results[2 * i].ToString().Equals(PlayerName))
                {
                    _rankingTexts[i].color = _yourNameColor;
                }
                else
                {
                    _rankingTexts[i].color = Color.white;
                }
            }
        }

        /// <summary>
        /// ユーザーIDを得るために呼ぶ関数
        /// </summary>
        /// <param name="idKey">ローカルデータに保存されてるユーザーIDを取得するために使うキー</param>
        private IEnumerator GetUserID()
        {
            yield return GetUserIDAsync(StoreUserID, _gssSettings.SheetName);

            string idKey = _gssConstSettings.PrefDefaultHiScoreKey;

            _userID = PlayerPrefs.GetString(idKey);

            if (string.IsNullOrEmpty(_userID))
            {
                // 乱数でテキトーに作成して被らなくなったら保存する
                do
                {
                    _userID = "user_" + UnityEngine.Random.Range(1000000, 9999999).ToString();
                }
                while (_getUserIDs.Contains(_userID));
                PlayerPrefs.SetString(idKey, _userID);
            }
        }

        /// <summary>
        /// GSS上にあるユーザーIDたちを変数に格納するための関数
        /// </summary>
        /// <param name="results">解析済み受信データ</param>
        private void StoreUserID(IList<object> results)
        {
            _getUserIDs.Clear();

            foreach (var result in results)
            {
                _getUserIDs.Add(result.ToString());
            }
        }

        /// <summary>
        /// GSS上にあるユーザーIDたちを取得するために必要なデータを準備する関数
        /// </summary>
        /// <param name="responseHandler">受信完了時に行いたい処理</param>
        /// <param name="sheetName">GSSのシート名</param>
        private CustomYieldInstruction GetUserIDAsync(Action<IList<object>> responseHandler, string sheetName = null)
        {
            var storeData = new Dictionary<string, object>();
            storeData[_gssConstSettings.SheetNameTag] = sheetName ?? "シート1";

            return GetRequestAsync(_gssConstSettings.GetUserIDTag, storeData, response => responseHandler?.Invoke(ParseResponse(response)));
        }
        #endregion
    }
}