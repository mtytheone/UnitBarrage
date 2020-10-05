using UnityEngine;
using Unity.Entities;
using TMPro;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// ゲームを管理するためのクラス
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// GameManager自身を示す変数
        /// </summary>
        public static GameManager Instance { get; private set; }


        #region Public Variable
        /// <summary>
        /// プレイヤーを管理しているクラスの参照
        /// </summary>
        public PlayerManager PlayerManager { get; private set; }

        /// <summary>
        /// ボスを管理しているクラスの参照
        /// </summary>
        public BossManager BossManager { get; private set; }

        /// <summary>
        /// Sceneの移行などを管理するクラスの参照
        /// </summary>
        public SceneController SceneController { get; private set; }

        /// <summary>
        /// 時間等を管理するクラスの参照
        /// </summary>
        public TimeManager TimeManager { get; private set; }

        /// <summary>
        /// GoogleSpreadSheetに関する管理クラスの参照
        /// </summary>
        public GSSController GSSController { get; private set; }

        /// <summary>
        /// ゲームの状態を表す変数
        /// </summary>
        public Define.GameStatus GameMode { get; set; }

        /// <summary>
        /// GSSとのインターネット接続をするかどうか
        /// </summary>
        public bool IsOnlineMode { get; private set; } = true;

        /// <summary>
        /// スコアデータ
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// Playerの弾の音を再生するAudioSource
        /// </summary>
        public AudioSource PlayerSEAudioSource { get { return _playerSEAudioSource; } }

        /// <summary>
        /// ボスの弾の音を再生するAudioSource
        /// </summary>
        public AudioSource BossSEAudioSource { get { return _bossSEAudioSource; } }

        public TextMeshProUGUI[] RankingTexts { get { return _rankingTexts; } }
        #endregion


        #region Serialize Variable
        [Header("GeneralSerialize")]
        [SerializeField]
        private AudioSource _playerSEAudioSource;

        [SerializeField]
        private AudioSource _bossSEAudioSource;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private GSSConnectSettings _gSSSettings;

        [SerializeField]
        private GSSConstantString _gSSConstSettings;

        [SerializeField]
        private PlayerSettings _playerSettings;

        [SerializeField]
        private BossSettings _bossSettings;

        [Header("ScoreRanking")]
        [SerializeField]
        private TextMeshProUGUI[] _rankingTexts;

        [Header("Debug")]
        [SerializeField]
        private bool _isDeleteAllPlayerPref;

        [SerializeField]
        private bool _isSetAllPlayerPref;

        [SerializeField]
        private string _setUserID;

        [SerializeField]
        private int _setHiScore;
        #endregion



        #region Unity Function
        private void Awake()
        {
            // シーン間をGameManagerが移動できるようにするための処理
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;

            // 初期化
            GameMode = Define.GameStatus.Title;
            PlayerManager = new PlayerManager(_playerSettings);
            BossManager = new BossManager(_bossSettings);
            SceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
            TimeManager = new TimeManager(_gameSettings);
            GSSController = new GSSController(_gameSettings, _gSSSettings, _gSSConstSettings, RankingTexts);

            // デバックの設定
            if (_isDeleteAllPlayerPref)
            {
                DeletePlayerPref();
            }
            if (_isSetAllPlayerPref)
            {
                PlayerPrefs.SetString(_gSSConstSettings.PrefDefaultHiScoreKey, "user_" + _setUserID);
                PlayerPrefs.SetInt(_gameSettings.HiScorePrefKey, _setHiScore);
            }
        }

        private void Update()
        {
            if (GameMode != Define.GameStatus.Pausing)
            {
                // 待機状態 → プレイ状態の遷移
                if (TimeManager.CheckWaitTime())
                {
                    if (GameMode == Define.GameStatus.Waiting)
                    {
                        GameMode = Define.GameStatus.Playing;
                    }
                }

                // スコア減少の実装（待機状態かプレイ状態の時のみ）
                if (GameMode == Define.GameStatus.Waiting || GameMode == Define.GameStatus.Playing)
                {
                    var decreasedNumber = (int)(Time.deltaTime * _gameSettings.DecreaseScoreUnit) <= 0 ? 1 : (int)(Time.deltaTime * _gameSettings.DecreaseScoreUnit);
                    Score -= decreasedNumber;
                }
            }
        }
        #endregion



        #region Public Function
        /// <summary>
        /// ボスの攻撃段階を初期化するときに外部から呼ぶ関数
        /// </summary>
        public void InitializeGame()
        {
            // プレイヤーとボスの初期化
            PlayerManager.Initialize();
            BossManager.Initialize();

            // スコアの初期化
            Score = _gameSettings.InitialScore;

            // ゲームを待機状態にする
            GameMode = Define.GameStatus.Waiting;

            // キャッシュ更新
            TimeManager.UpdateCacheTime();

            // CameraのFarを更新（遠くまで見えるようにする）
            Camera.main.farClipPlane = Define.FARCLIP_GAME;
        }

        /// <summary>
        /// ボスの攻撃段階を一つ上げるときに外部から呼ぶ関数
        /// </summary>
        public void UpdateBossPhase()
        {
            // 敵の攻撃段階を上げる
            BossManager.UpdateBossCount();

            // キャッシュ更新
            TimeManager.UpdateCacheTime();

            // 攻撃段階が最大値を超えたらゲーム終了
            if (BossManager.CheckOverCount())
            {
                SceneController.EndGame();
            }
        }

        /// <summary>
        /// スコアデータからハイスコアを更新したかどうかを見るために外部から呼ぶ関数
        /// これを呼ぶと、ハイスコアが更新される
        /// </summary>
        /// <returns>ハイスコアが更新されたかどうか</returns>
        public void UpdateNewHiScore()
        {
            // 合計スコアを計算
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerHP = entityManager.Exists(EntityInformation.PlayerEntity) ? entityManager.GetComponentData<StatusData>(EntityInformation.PlayerEntity).HP : 0;
            int totalScore = (int)(Score * playerHP);

            // ハイスコアをロードし、更新していたらデータを更新して返す
            var hiscore = PlayerPrefs.GetInt(_gameSettings.HiScorePrefKey);
            if (hiscore < totalScore)
            {
                PlayerPrefs.SetInt(_gameSettings.HiScorePrefKey, totalScore);
            }
        }

        /// <summary>
        /// 合計スコアを取得するために外部から呼ぶ関数
        /// </summary>
        /// <returns>合計スコア</returns>
        public int GetTotalScore()
        {
            // 合計スコア = 残り残機 x スコア
            var playerHP = PlayerManager.GetPlayerHP();
            var totalScore = playerHP * Score;

            return totalScore;
        }

        /// <summary>
        /// オンラインにするかどうかを設定する際に外部から呼ぶ関数
        /// </summary>
        /// <param name="setmode">オンラインの可否</param>
        public void SetOnlineMode(bool setmode)
        {
            IsOnlineMode = setmode;
        }

        /// <summary>
        /// ローカルに保存しているデータを全部消すために呼ぶ関数（デバック用）
        /// </summary>
        private void DeletePlayerPref()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion
    }
}