using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーに関する処理などをする管理クラス
    /// </summary>
    public class PlayerManager
    {
        #region Public Variable
        /// <summary>
        /// プレイヤーの設定データをまとめたデータ群
        /// </summary>
        public PlayerSettings PlayerSettingdata { get; private set; }
        #endregion


        #region Private Variable
        /// <summary>
        /// プレイヤー弾を生成するSystem
        /// </summary>
        private Player_ShotBulletSystem _playerShotBulletSystem;
        /// <summary>
        /// プレイヤーの動きを行うSystem
        /// </summary>
        private Player_MovementSystem _playerMovementSystem;
        /// <summary>
        /// プレイヤー弾の消去を行うSystem
        /// </summary>
        private PlayerBullet_DestroySystem _playerbulletDestroySystem;
        #endregion


        #region Public Function
        public PlayerManager(PlayerSettings playerSettings)
        {
            PlayerSettingdata = playerSettings;
            var world = World.DefaultGameObjectInjectionWorld;
            _playerShotBulletSystem = world.GetOrCreateSystem<Player_ShotBulletSystem>();
            _playerMovementSystem = world.GetOrCreateSystem<Player_MovementSystem>();
            _playerbulletDestroySystem = world.GetOrCreateSystem<PlayerBullet_DestroySystem>();
        }

        /// <summary>
        /// プレイヤーの初期化を行うために外部から呼ぶ関数
        /// </summary>
        public void Initialize()
        {
            _playerShotBulletSystem.SetInterval(PlayerSettingdata.PlayerShotRate);
            _playerMovementSystem.SetBounds(PlayerSettingdata.XMinBound, PlayerSettingdata.XMaxBound, PlayerSettingdata.YMinBound, PlayerSettingdata.YMaxBound);
            _playerbulletDestroySystem.SetBounds(PlayerSettingdata.XMaxBound, PlayerSettingdata.YMaxBound + 0.1f);
        }

        /// <summary>
        /// プレイヤーの残りHPを得るために外部から呼ぶ関数
        /// </summary>
        /// <returns>プレイヤーの残りHP</returns>
        public int GetPlayerHP()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerHP = entityManager.Exists(EntityInformation.PlayerEntity) ? entityManager.GetComponentData<StatusData>(EntityInformation.PlayerEntity).HP : 0;

            return (int)playerHP;
        }
        #endregion
    }
}