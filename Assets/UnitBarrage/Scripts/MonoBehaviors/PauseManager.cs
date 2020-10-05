using UnityEngine;
using UnityEngine.UI;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// ポーズの実行を行うためのクラス
    /// </summary>
    public class PauseManager : MonoBehaviour
    {
        #region Serialize Variable
        /// <summary>
        /// ポーズ時に表示するキャンバス
        /// </summary>
        [SerializeField]
        private Canvas _pauseCanvas;

        [SerializeField]
        private Button _unpauseButton;
        #endregion


        #region Private Variable
        /// <summary>
        /// InputActionsのデータ
        /// </summary>
        private InputControlData _inputActions;
        #endregion



        #region Unity Function
        private void Awake()
        {
            // InputActionを作ってPause関数を登録
            _inputActions = new InputControlData();
            _inputActions.Player.Pause.performed += context => Pause();

            // 初期はポーズCanvasを非表示にする
            _pauseCanvas.gameObject.SetActive(false);

            // ポーズ解除ボタンにPause関数を登録
            _unpauseButton.onClick.RemoveAllListeners();
            _unpauseButton.onClick.AddListener(() => Pause());
        }

        private void OnApplicationFocus(bool focus)
        {
            // ゲームが待機状態もしくはプレイ状態の時で、アプリからフォーカスが外れた場合はPause関数を呼ぶ
            var gamemMode = GameManager.Instance.GameMode;
            if (focus == false && (gamemMode == Define.GameStatus.Waiting || gamemMode == Define.GameStatus.Playing))
            {
                Pause();
            }
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void OnDestroy()
        {
            _inputActions.Dispose();
        }
        #endregion


        
        #region Private Function
        /// <summary>
        /// ポーズもしくはポーズ解除をする際に呼ぶ関数
        /// </summary>
        private void Pause()
        {
            var gameManager = GameManager.Instance;
            var gameMode = gameManager.GameMode;

            // タイトルとゲーム終了時には何もしない
            if (gameMode != Define.GameStatus.Title && gameMode != Define.GameStatus.End)
            {
                // ポーズ → ポーズ解除
                if (gameMode == Define.GameStatus.Pausing)
                {
                    // ポーズ前がプレイ中だったら
                    if (gameManager.TimeManager.CheckWaitTime())
                    {
                        gameManager.GameMode = Define.GameStatus.Playing;
                    }
                    else // ポーズ前が待機状態だったら
                    {
                        gameManager.GameMode = Define.GameStatus.Waiting;
                    }

                    // 時間軸を戻してCanvasを非表示
                    Time.timeScale = 1;
                    _pauseCanvas.gameObject.SetActive(false);
                }
                else // 待機 or プレイ中 → ポーズ
                {
                    // ポーズ状態にして時間軸を止めてポーズCanvasを出す
                    gameManager.GameMode = Define.GameStatus.Pausing;
                    Time.timeScale = 0;
                    _pauseCanvas.gameObject.SetActive(true);
                }
            }
        }
        #endregion
    }
}