using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MiniJSON;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// GoogleSpreadSheetからデータを受信する本体クラス
    /// </summary>
    public class GSSGetProcess
    {
        #region Private Variable
        /// <summary>
        /// GSSと接続する際に必要な文字列定数データ
        /// </summary>
        private GSSConstantString _gssConstSettings;
        /// <summary>
        /// GASのURL
        /// </summary>
        private string _appURL;
        #endregion


        #region Public Function
        public GSSGetProcess(string appURL, GSSConstantString gssConst)
        {
            _appURL = appURL;
            _gssConstSettings = gssConst;
        }

        /// <summary>
        /// GSSからデータを受信ための最終調整と実際にデータ受信要求をするために外部から呼ぶ関数
        /// </summary>
        /// <param name="monobehavior">何かしらのMonoBehavior</param>
        /// <param name="processName">何の処理をさせたいかを記した文字列定数</param>
        /// <param name="sendData">受信に必要な送信データ</param>
        /// <param name="responseHandler">受信完了時に行いたい処理</param>
        public CustomYieldInstruction GetDataProcessAsync(MonoBehaviour monobehavior, string processName, IDictionary<string, object> sendData, Action<object> responseHandler = null)
        {
            var stringJonData = Json.Serialize(sendData);

            var formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(_gssConstSettings.MethodTag, processName));
            formData.Add(new MultipartFormDataSection(_gssConstSettings.PayloadTag, stringJonData));

            var complete = false;
            monobehavior.StartCoroutine(GetDataToGSS(formData, status => complete = status, responseHandler));

            return new WaitUntil(() => complete);
        }
        #endregion



        #region Private Function
        /// <summary>
        /// 受信要求と関連する処理を実行する本体関数
        /// </summary>
        /// <param name="formData">最終形式にした送るデータ</param>
        /// <param name="statusHandler">一連の処理が終わったかどうかを反映するコールバック</param>
        /// <param name="responseHandler">受信完了時に行う処理</param>
        private IEnumerator GetDataToGSS(List<IMultipartFormSection> formData, Action<bool> statusHandler, Action<object> responseHandler = null)
        {
            statusHandler(false);

            var www = UnityWebRequest.Post(_appURL, formData);
            Debug.Log("[<color=blue>GSSDataService</color>]\n Start sending data to Google Sheets.");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError($"[<color=blue>GSSDataService</color>]\n Sending data to Google Sheets failed. Error: {www.error}");
            }
            else
            {
                Debug.Log("[<color=blue>GSSDataService</color>]\n Sending data to Google Sheets completed");

                try
                {
                    var response = Json.Deserialize(www.downloadHandler.text);
                    var message = response as string;
                    if (message != null && message.Contains("Error"))
                    {
                        Debug.LogError($"[<color=blue>GSSDataService</color>]\n Getting data from Google Sheets failed. {message}");
                    }
                    else
                    {
                        // 返事を引数に処理後に行う処理を発動
                        responseHandler?.Invoke(response);
                    }
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError($"[<color=blue>GSSDataService</color>]\n Parsing result from Google Sheets failed. Error: {e.Message}");
                }

                statusHandler(true);
            }
        }
        #endregion
    }
}