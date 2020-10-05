using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自機弾を出す処理を行うシステム
    /// </summary>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(UpdateWorldTimeSystem))]
    public class Player_ShotBulletSystem : SystemBase
    {
        /// <summary>
        /// EntityCommandBuffer
        /// </summary>
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        /// <summary>
        /// 経過時間を保持する変数
        /// </summary>
        private float _interval = 0;
        /// <summary>
        /// 弾が出るレート時間を保持する変数
        /// </summary>
        private float _intervalThreshold;

        protected override void OnCreate()
        {
            // EntityCommandBufferの取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var gameManager = GameManager.Instance;
            var timeManager = GameManager.Instance.TimeManager;
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var prefabEntity = EntityInformation.PlayerBulletEntityPrefab;
            var playerBulletDistance = gameManager.PlayerManager.PlayerSettingdata.BulletDistance;

            if (gameManager.GameMode != Define.GameStatus.Pausing)
            {
                // 攻撃段階が変更してから5秒間は撃たない
                if (timeManager.CheckWaitTime())
                {
                    _interval += Time.DeltaTime;

                    // 一定のレートで弾の生成を行う
                    if (_interval > _intervalThreshold)
                    {
                        _interval -= _intervalThreshold;

                        // 自機弾の生成
                        // メインスレッド処理
                        Entities
                            .WithName("InstantiatePlayerBullet")
                            .WithoutBurst()
                            .WithAll<PlayerTag>()
                            .ForEach((in PlayerSettingData playerData, in Translation translation) =>
                            {
                            // ショットボタンが押されていたら
                            if (playerData.IsPressedShotKey)
                                {
                                // 左弾の生成
                                var leftEntity = commandBuffer.Instantiate(prefabEntity);
                                    var initialTranslation = translation.Value;
                                    initialTranslation.x -= playerBulletDistance;
                                    commandBuffer.SetComponent(leftEntity, new Translation
                                    {
                                        Value = initialTranslation
                                    });

                                // 右弾の生成
                                var rightEntity = commandBuffer.Instantiate(prefabEntity);
                                    initialTranslation = translation.Value;
                                    initialTranslation.x += playerBulletDistance;
                                    commandBuffer.SetComponent(rightEntity, new Translation
                                    {
                                        Value = initialTranslation
                                    });

                                // ショット音を再生
                                gameManager.PlayerSEAudioSource.Play();
                                }

                            }).Run();

                        // EntityCommandBuffer経由でJobを登録
                        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
                    }
                }
                else
                {
                    _interval = 0;
                }
            }
        }

        /// <summary>
        /// レート時間を登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="interval">プレイヤーの発射レート</param>
        public void SetInterval(float interval)
        {
            _intervalThreshold = interval;
        }
    }
}