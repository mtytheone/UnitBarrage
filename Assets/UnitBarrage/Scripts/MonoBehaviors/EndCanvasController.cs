using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// スコア表示のCanvasたちの表示制御をするためのクラス
    /// </summary>
    public class EndCanvasController : MonoBehaviour
    {
        #region Serialize Variable
        /// <summary>
        /// EndスコアのメインCanvas
        /// </summary>
        [SerializeField]
        private Image _mainCanvas;

        /// <summary>
        /// Endスコアのオンラインスコア表示キャンバス
        /// </summary>
        [SerializeField]
        private Image _scoreRakingCanvas;

        /// <summary>
        /// オンラインスコアキャンバスからメインキャンバスに戻るためのボタン
        /// </summary>
        [SerializeField]
        private Button _backResultButton;

        /// <summary>
        /// メインキャンバスからオンラインスコアに進むためのボタン
        /// </summary>
        [SerializeField]
        private Button _goRankingButton;
        #endregion

        #region Private Variable
        private TextMeshProUGUI[] _rankingTexts;
        private readonly WaitForSeconds _waitOneSecond = new WaitForSeconds(1.0f);
        #endregion


        #region Unity Function
        // 非ActiveからActiveになるたびに呼ばれる
        void Start()
        {
            // パネルの初期化
            _mainCanvas.gameObject.SetActive(true);
            _scoreRakingCanvas.gameObject.SetActive(false);

            // テキストの参照を受け取る
            _rankingTexts = GameManager.Instance.RankingTexts;

            // タイトルに戻るボタンの設定
            _backResultButton.onClick.RemoveAllListeners();
            _backResultButton.onClick.AddListener(() =>
            {
                _mainCanvas.gameObject.SetActive(true);
                _scoreRakingCanvas.gameObject.SetActive(false);
            });

            // オンラインスコアランキング画面に移動するボタンの設定
            _goRankingButton.onClick.RemoveAllListeners();
            _goRankingButton.onClick.AddListener(() =>
            {
                _scoreRakingCanvas.gameObject.SetActive(true);
                _mainCanvas.gameObject.SetActive(false);
                StartCoroutine(DisplayScoreRanking());
            });
        }
        #endregion


        #region Private Function
        /// <summary>
        /// オンラインスコアランキングを表示するために呼ぶ関数
        /// </summary>
        private IEnumerator DisplayScoreRanking()
        {
            // 初期化
            _backResultButton.interactable = false;
            foreach (var text in _rankingTexts)
            {
                text.enabled = text.gameObject.name.Equals("YourScoreText");
            }

            // 一秒おきに一個ずつ下の順位から見せていく
            for (int i = _rankingTexts.Length - 2; i >= 0; i--)
            {
                yield return _waitOneSecond;

                _rankingTexts[i].enabled = true;
            }

            // 最後に戻るボタンを触れるようにする
            _backResultButton.interactable = true;
        }
        #endregion
    }
}