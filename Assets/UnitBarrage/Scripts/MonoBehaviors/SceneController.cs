using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;
using TMPro;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// シーンの変更を管理するクラス
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        #region Serialize Variable
        [SerializeField]
        private Canvas _loadCanvas;

        [SerializeField]
        private Slider _loadSlider;

        [SerializeField]
        private List<Vector2> _windowResolutionXY;

        [SerializeField]
        private Animator _loadingIconAnimator;

        [SerializeField]
        private Canvas _endDisplayScoreCanvas;

        [SerializeField]
        private TextMeshProUGUI _playerHPText;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private TextMeshProUGUI _totalScoreText;

        [SerializeField]
        private TextMeshProUGUI _totalScoreFormulaText;

        [SerializeField]
        private Button _returnTitleButton;

        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private Button _scoreRankingButton;

        [SerializeField]
        private Button _tweetButton;
        #endregion

        #region Private Variable
        private Toggle _fullscreenToggleUI;
        private Dropdown _windowsizeDropdownUI;
        private Button _okButton;

        private readonly WaitForSeconds _waitPointFive = new WaitForSeconds(0.5f);
        private readonly WaitForSeconds _waitOne = new WaitForSeconds(1.0f);
        private readonly WaitForSeconds _waitSecond = new WaitForSeconds(2.0f);
        #endregion



        #region Unity Function
        private void Awake()
        {
            // Titleシーン読み込み時に変数を登録する処理を登録
            SceneManager.sceneLoaded += RegisterTitleSceneObject;

            // ボタンの登録
            _returnTitleButton.onClick.RemoveAllListeners();
            _returnTitleButton.onClick.AddListener(() => StartCoroutine(LoadTitleScene()));
            _restartButton.onClick.RemoveAllListeners();
            _restartButton.onClick.AddListener(() => StartCoroutine(ReloadGameScene()));

            #if UNITY_EDITOR
            #elif UNITY_STANDALONE
            // アプリだと起動時には一つのシーンしか読み込まれないため、追加で読み込む
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Additive);
            // 最初は全画面にはしないでおく
            Screen.fullScreen = false;
            #endif
        }
        #endregion



        #region Public Function
        /// <summary>
        /// シーンを最初の設定に戻すときに外部から呼ぶ関数
        /// </summary>
        public void EndGame()
        {
            GameManager.Instance.GameMode = Define.GameStatus.End;
            StartCoroutine(DisplayScoreCanvas());
        }
        #endregion



        #region Private Function
        /// <summary>
        /// FullScreenのトグルのチェックに合わせてDropdownを触らせたり触らせないようにする際に呼ぶ関数
        /// </summary>
        /// <param name="toggleIsOn">Toggleのbool</param>
        private void OnToggleValueChanged(bool toggleIsOn)
        {
            // フルスクリーンを選択中はDropdownは触らせないようにする
            _windowsizeDropdownUI.interactable = !toggleIsOn;
        }

        /// <summary>
        /// ウィンドウ設定シーンでのOKボタンを押した際に呼ぶ関数
        /// </summary>
        private void OnPlayButtonClicked()
        {
            // フルスクリーンを選択したなら
            if (_fullscreenToggleUI.isOn)
            {
                // 画面をフルスクリーンにする
                #if UNITY_EDITOR
                var gameview = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
                gameview.maximized = true;
                #elif UNITY_STANDALONE
                Screen.SetResolution(1440, 810, false);
                 Screen.fullScreen = true;
                #endif
            }
            else //フルスクリーンを選択してないなら
            {
                // Dropdownで選択したウィンドウサイズにする
                #if UNITY_STANDALONE
                Screen.SetResolution((int)_windowResolutionXY[_windowsizeDropdownUI.value].x, (int)_windowResolutionXY[_windowsizeDropdownUI.value].y, false);
                #endif
            }

            // 非同期でメインのシーンをロード
            StartCoroutine(LoadGameScene());
        }

        /// <summary>
        /// 自分のスコア結果を出すために呼ぶ関数
        /// </summary>
        private IEnumerator DisplayScoreCanvas()
        {
            // Entitymanager
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // 0.5秒待つ
            yield return _waitPointFive;

            // 初期化
            _playerHPText.enabled = false;
            _scoreText.enabled = false;
            _totalScoreText.enabled = false;
            _totalScoreFormulaText.enabled = false;
            _returnTitleButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
            _scoreRankingButton.gameObject.SetActive(false);
            _tweetButton.gameObject.SetActive(false);
            _endDisplayScoreCanvas.gameObject.SetActive(true);

            // 一秒待つ
            yield return _waitOne;

            // HPの残機を表示
            var playerHP = GameManager.Instance.PlayerManager.GetPlayerHP();
            _playerHPText.text = string.Format("PlayerHP : {0}", playerHP);
            _playerHPText.enabled = true;

            // 一秒待つ
            yield return _waitOne;

            // 単体スコアを表示
            _scoreText.text = string.Format("Score : {0}pt", GameManager.Instance.Score);
            _scoreText.enabled = true;

            // 一秒待つ
            yield return _waitOne;

            // 合計スコアを表示
            var totalScore = GameManager.Instance.GetTotalScore();
            _totalScoreText.text = string.Format("TotalScore : {0}", totalScore);
            _totalScoreText.enabled = true;
            _totalScoreFormulaText.text = string.Format("{0}pt x {1} = {2}pt", GameManager.Instance.Score, playerHP, totalScore);
            _totalScoreFormulaText.enabled = true;

            // 一秒待つ
            yield return _waitSecond;

            // ボタンを表示（触れない）
            _returnTitleButton.interactable = false;
            _returnTitleButton.gameObject.SetActive(true);
            _restartButton.interactable = false;
            _restartButton.gameObject.SetActive(true);
            _scoreRankingButton.interactable = false;
            _scoreRankingButton.gameObject.SetActive(true);
            _tweetButton.interactable = true;
            _tweetButton.gameObject.SetActive(true);

            if (GameManager.Instance.IsOnlineMode)
            {
                // データ送信＆受信
                _loadingIconAnimator.SetBool("IsLoading", true);
                yield return GameManager.Instance.GSSController.SendData();
                yield return GameManager.Instance.GSSController.GetData();
                _loadingIconAnimator.SetBool("IsLoading", false);
            }

            // 触れるようにする
            _returnTitleButton.interactable = true;
            _restartButton.interactable = true;
            _scoreRankingButton.interactable = GameManager.Instance.IsOnlineMode;
            _tweetButton.interactable = true;
        }

        /// <summary>
        /// 非同期でシーンを切り替える際に呼ぶ関数
        /// </summary>
        private IEnumerator LoadGameScene()
        {
            // 少し待つ
            yield return _waitPointFive;

            // 非同期でロード開始
            var asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            // ロード時に行う処理をイベントに登録
            SceneManager.sceneLoaded += GameSceneInitialize;

            // 初期化
            asyncLoad.allowSceneActivation = false;
            _loadSlider.value = _loadSlider.minValue;
            _loadCanvas.gameObject.SetActive(true);
            float progress = 0;

            // ロードが9割完了するまで無限ループ
            do
            {
                yield return null;

                // ロードの進行具合を反映
                progress = asyncLoad.progress;
                _loadSlider.value = progress;

            } while (progress < 0.9f);

            // スライダーを最大値にしてロードを完了させたことを表示
            _loadSlider.value = _loadSlider.maxValue;
            // 早くロードしすぎた場合にもロードしている風を出すためにわざと待たせている
            yield return _waitSecond;

            // ゲームを初期化
            GameManager.Instance.InitializeGame();

            // シーン切り替えを許可
            asyncLoad.allowSceneActivation = true;

            // 元のシーンをアンロード
            yield return SceneManager.UnloadSceneAsync("TitleScene");

            // スコア画面を非表示にする
            _endDisplayScoreCanvas.gameObject.SetActive(false);

            // ロード画面を非表示にする
            _loadCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// 非同期でもう一度ゲームシーンをロード際に呼ぶ関数
        /// </summary>
        private IEnumerator ReloadGameScene()
        {
            // 元のシーンをアンロード
            yield return SceneManager.UnloadSceneAsync("GameScene");

            // Entityを全部消して、失敗したらエラー表示
            if (!DestroyAllEntities())
            {
                Debug.LogError("EndEntityError");
            }

            // 非同期でロード開始
            var asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            // ロード時に行う処理をイベントに登録
            SceneManager.sceneLoaded += GameSceneInitialize;

            // 初期化
            asyncLoad.allowSceneActivation = false;
            _loadSlider.value = _loadSlider.minValue;
            _loadCanvas.gameObject.SetActive(true);
            float progress = 0;

            // ロードが9割完了するまで無限ループ
            do
            {
                yield return null;

                // ロードの進行具合を反映
                progress = asyncLoad.progress;
                _loadSlider.value = progress;

            } while (progress < 0.9f);

            // スライダーを最大値にしてロードを完了させたことを表示
            _loadSlider.value = _loadSlider.maxValue;
            // 早くロードしすぎた場合にもロードしている風を出すためにわざと待たせている
            yield return _waitSecond;

            // ゲームを初期化
            GameManager.Instance.InitializeGame();

            // シーン切り替えを許可
            asyncLoad.allowSceneActivation = true;

            // スコア画面を非表示にする
            _endDisplayScoreCanvas.gameObject.SetActive(false);

            // ロード画面を非表示にする
            _loadCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// 初期シーンに戻るときに呼ぶ関数（Entityを消す処理もしている）
        /// </summary>
        private IEnumerator LoadTitleScene()
        {
            // Entityを全部消して、失敗したらエラー表示
            if (!DestroyAllEntities())
            {
                Debug.LogError("EndEntityError");
            }

            // ゲームをタイトルの状態にする
            GameManager.Instance.GameMode = Define.GameStatus.Title;

            // シーンを同期的にロード
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Additive);

            // CameraのFarを更新（Entityが見えない距離まで縮める）
            Camera.main.farClipPlane = Define.FARCLIP_TITLE;

            // ゲームシーンをアンロード
            yield return SceneManager.UnloadSceneAsync("GameScene");
        }

        /// <summary>
        /// 次にロードするシーンからHPバーを探してきてSystemにセットするために呼ぶ関数
        /// </summary>
        private void GameSceneInitialize(Scene next, LoadSceneMode mode)
        {
            // 各Systemを取得
            var world = World.DefaultGameObjectInjectionWorld;
            var playerHPDisplay = world.GetOrCreateSystem<Player_HPDisplaySystem>();
            var bossHPDisplay = world.GetOrCreateSystem<Boss_HPDisplaySystem>();

            // シーンからルートオブジェクトを取得
            foreach (var rootGameObject in next.GetRootGameObjects())
            {
                // Canvasなら
                if (rootGameObject.name.Equals("StatusCanvas"))
                {
                    // HPバーを取得
                    var playerSliders = rootGameObject.transform.Find("PlayerHPSlider").GetComponent<Slider>();
                    if (playerSliders != null)
                    {
                        // 登録
                        playerHPDisplay.SetSlider(playerSliders);
                    }

                    // HPバーを取得
                    var bossSliders = rootGameObject.transform.Find("BossHPSlider").GetComponent<Slider>();
                    if (bossSliders != null)
                    {
                        // 各種登録
                        var bossSettingdata = GameManager.Instance.BossManager.BossSettingData;
                        bossHPDisplay.SetSlider(bossSliders);
                        bossHPDisplay.SetMaxValue(bossSettingdata.InitialBossHP);
                        bossHPDisplay.SetColor(bossSettingdata.NormalBossHPBarColor);
                        bossHPDisplay.Iniialize();
                    }
                }
            }

            // ローディングアイコンをオンラインモードの時のみ表示
            _loadingIconAnimator.gameObject.SetActive(GameManager.Instance.IsOnlineMode);

            // イベント削除
            SceneManager.sceneLoaded -= GameSceneInitialize;
        }

        /// <summary>
        /// Titleのシーンが読み込まれる際に毎回変数を登録する処理
        /// </summary>
        private void RegisterTitleSceneObject(Scene next, LoadSceneMode mode)
        {
            // タイトルシーンだったら
            if (next.name.Equals("TitleScene"))
            {
                #if UNITY_EDITOR
                var isFullScreen =  EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor")).maximized;
                #elif UNITY_STANDALONE
                var isFullScreen = Screen.fullScreen;
                #endif
                var currentResolution = Screen.currentResolution;

                // シーン起動時、最小ウィンドウに設定
                #if UNITY_EDITOR
                var gameview = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
                gameview.maximized = false;
                #elif UNITY_STANDALONE
                Screen.SetResolution(300, 450, false);
                #endif

                foreach (var rootObject in next.GetRootGameObjects())
                {
                    // SettingCanvasにのみ処理をする
                    if (rootObject.name.Equals("SettingCanvas"))
                    {
                        // プレイ前に記入した内容を再度反映
                        var playernameInputfield = rootObject.transform.Find("MainPanel/PlayerNameInputField").GetComponent<InputField>();
                        playernameInputfield.text = GameManager.Instance.GSSController.PlayerName ?? "";
                        // プレイヤーネームを登録するリスナーを再設定
                        playernameInputfield.onEndEdit.RemoveAllListeners();
                        playernameInputfield.onEndEdit.AddListener((content) => GameManager.Instance.GSSController.SetPlayerName(content));

                        // プレイ前に押した内容を再度反映
                        _fullscreenToggleUI = rootObject.transform.Find("MainPanel/FullScreenToggle").GetComponent<Toggle>();
                        _fullscreenToggleUI.isOn = isFullScreen;
                        // FullScreenにするかどうかを設定するToggleにつけるリスナーを再設定
                        _fullscreenToggleUI.onValueChanged.RemoveAllListeners();
                        _fullscreenToggleUI.onValueChanged.AddListener(OnToggleValueChanged);

                        // WindowSizeを設定するDropdownにつけるOptionを再設定
                        _windowsizeDropdownUI = rootObject.transform.Find("MainPanel/WindowSizeDropdown").GetComponent<Dropdown>();
                        _windowsizeDropdownUI.options.Clear();
                        foreach (var resolution in _windowResolutionXY)
                        {
                            _windowsizeDropdownUI.options.Add(new Dropdown.OptionData(string.Format("{0} x {1}", resolution.x, resolution.y)));

                            // プレイ前に選んだやつを再度設定
                            #if UNITY_STANDALONE
                            if (resolution.x == currentResolution.width && resolution.y == currentResolution.height)
                            {
                                _windowsizeDropdownUI.value = _windowResolutionXY.IndexOf(resolution);
                            }
                            #endif
                        }

                        // プレイ前に押した内容を再度反映
                        var offlineToggle = rootObject.transform.Find("MainPanel/OfflineModeToggle").GetComponent<Toggle>();
                        offlineToggle.isOn = !GameManager.Instance.IsOnlineMode;
                        // 押した際にオンラインモードを切り替えるリスナーを設定
                        offlineToggle.onValueChanged.RemoveAllListeners();
                        offlineToggle.onValueChanged.AddListener((value) => {
                            GameManager.Instance.SetOnlineMode(!value);
                            playernameInputfield.interactable = !value;
                            });

                        // OKボタンのリスナーを再設定
                        _okButton = rootObject.transform.Find("MainPanel/PlayButton").GetComponent<Button>();
                        _okButton.onClick.RemoveAllListeners();
                        _okButton.onClick.AddListener(OnPlayButtonClicked);
                    }
                }

                // スコア表示画面は初期では非表示にしておく
                _endDisplayScoreCanvas.gameObject.SetActive(false);

                // ロード画面は初期では非表示にしておく
                _loadCanvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Entityを全部消す時に呼ぶ関数
        /// </summary>
        /// <returns>正しく動作していたらtrueが返る</returns>
        private bool DestroyAllEntities()
        {
            // 全Entityを消す
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            foreach (var entity in entityManager.GetAllEntities())
            {
                entityManager.DestroyEntity(entity);
            }

            return true;
        }
        #endregion
    }
}