using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 敵弾を出す処理を行うシステム
    /// </summary>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(UpdateWorldTimeSystem))]
    public class Boss_BulletShotSystem : SystemBase
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
        /// 第一段階での弾が出るレート時間を保持する変数
        /// </summary>
        private float _intervalThreshold_in_Phase1;
        /// <summary>
        /// 第二段階での弾が出るレート時間を保持する変数
        /// </summary>
        private float _intervalThreshold_in_Phase2;
        /// <summary>
        /// 第三段階での弾が出るレート時間を保持する変数
        /// </summary>
        private float _intervalThreshold_in_Phase3;

        protected override void OnCreate()
        {
            // EntityCommandBufferの取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var gameManager = GameManager.Instance;
            var bossManager = gameManager.BossManager;
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // ゲームシーンを開いている時のみ処理をする
            if (gameManager.GameMode == Define.GameStatus.Playing || gameManager.GameMode == Define.GameStatus.Waiting)
            {
                // 攻撃段階が変更してから5秒間は撃たない
                if (gameManager.TimeManager.CheckWaitTime())
                {
                    _interval += Time.DeltaTime;

                    // 攻撃第一段階
                    if (bossManager.EqualBossPhaseCount(0))
                    {
                        // 一定のレートで弾の生成を行う
                        if (_interval > _intervalThreshold_in_Phase1)
                        {
                            _interval -= _intervalThreshold_in_Phase1;

                            var prefabEntity1 = EntityInformation.BossBulletEntityPrefab[0];

                            // レート測定結果による敵の弾の生成
                            Entities
                                .WithName("InstantiateBossBullet1")
                                .WithAll<BossBulletPoint1Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                // 生成
                                var entity = commandBuffer.Instantiate(prefabEntity1);
                                    commandBuffer.SetComponent(entity, new Translation
                                    {
                                        Value = localToWorld.Position
                                    });

                                    commandBuffer.SetComponent(entity, new Rotation
                                    {
                                        Value = localToWorld.Rotation
                                    });

                                }).Schedule();

                            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

                            // SEの再生
                            gameManager.BossSEAudioSource.Play();
                        }
                    }
                    // 攻撃第二段階
                    else if (bossManager.EqualBossPhaseCount(1))
                    {
                        // 一定のレートで弾の生成を行う
                        if (_interval > _intervalThreshold_in_Phase2)
                        {
                            _interval -= _intervalThreshold_in_Phase2;

                            var playerEntity = EntityInformation.PlayerEntity;
                            var prefabEntity2 = EntityInformation.BossBulletEntityPrefab[1];

                            if (EntityManager.Exists(playerEntity))
                            {
                                // レート測定結果による敵の弾の生成
                                Entities
                                    .WithName("InstantiateBossBullet2")
                                    .WithAll<BossBulletPoint2Tag>()
                                    .ForEach((in Translation translation, in LocalToWorld localToWorld) =>
                                    {
                                    // 発射台からプレイヤーのベクトル
                                    var playerLocalToWorld = GetComponent<LocalToWorld>(playerEntity);
                                        var direction = math.normalizesafe(playerLocalToWorld.Position - localToWorld.Position);
                                        var velocity = GetComponent<PhysicsVelocity>(prefabEntity2);
                                        velocity.Linear = direction;

                                    // 生成
                                    var entity = commandBuffer.Instantiate(prefabEntity2);
                                    commandBuffer.SetComponent(entity, new Translation
                                    {
                                        Value = localToWorld.Position
                                    });

                                    commandBuffer.SetComponent(entity, new Rotation
                                    {
                                        Value = localToWorld.Rotation
                                    });

                                    commandBuffer.SetComponent(entity, velocity);

                                    }).Schedule();

                                // SE再生
                                gameManager.BossSEAudioSource.Play();

                                _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
                            }
                        }
                    }
                    // 攻撃第三段階
                    else if (bossManager.EqualBossPhaseCount(2))
                    {
                        // 一定のレートで弾の生成を行う
                        if (_interval > _intervalThreshold_in_Phase3)
                        {
                            _interval -= _intervalThreshold_in_Phase3;

                            var prefabEntity3 = EntityInformation.BossBulletEntityPrefab[2];

                            // レート測定結果による敵の弾の生成
                            Entities
                                .WithName("InstantiateBossBullet3_1")
                                .WithoutBurst()
                                .WithAll<BossBulletPoint3_1Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                    InstantiateBullet(1.0f, in localToWorld, commandBuffer, prefabEntity3);

                                }).Run();

                            Entities
                                .WithName("InstantiateBossBullet3_2")
                                .WithoutBurst()
                                .WithAll<BossBulletPoint3_2Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                    InstantiateBullet(2.0f, in localToWorld, commandBuffer, prefabEntity3);

                                }).Run();

                            Entities
                                .WithName("InstantiateBossBullet3_3")
                                .WithoutBurst()
                                .WithAll<BossBulletPoint3_3Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                    InstantiateBullet(3.0f, in localToWorld, commandBuffer, prefabEntity3);

                                }).Run();

                            // これ以上出したら重かった
                            /*Entities
                                .WithName("InstantiateBossBullet3_4")
                                .WithoutBurst()
                                .WithAll<BossBulletPoint3_4Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                    InstantiateBullet(4.0f, in localToWorld, commandBuffer, prefabEntity3);

                                }).Run();

                            Entities
                                .WithName("InstantiateBossBullet3_5")
                                .WithoutBurst()
                                .WithAll<BossBulletPoint3_5Tag>()
                                .ForEach((in LocalToWorld localToWorld) =>
                                {
                                    InstantiateBullet(5.0f, in localToWorld, commandBuffer, prefabEntity3);

                                }).Run();*/

                            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

                            // SE再生
                            gameManager.BossSEAudioSource.Play();
                        }
                    }
                }
                else
                {
                    _interval = 0;
                }
            }
        }

        /// <summary>
        /// 弾の生成
        /// </summary>
        /// <param name="multiplier">速度係数</param>
        /// <param name="localToWorld">LocalToWorld</param>
        /// <param name="commandBuffer">CommandBuffer</param>
        /// <param name="prefabEntity3">複製するEntity</param>
        private void InstantiateBullet(float multiplier, in LocalToWorld localToWorld, EntityCommandBuffer commandBuffer, Entity prefabEntity3)
        {
            var translation = localToWorld.Position;
            translation.z = 0;
            var bulletstatus = GetComponent<BulletStatusData>(prefabEntity3);
            bulletstatus.BulletSpeed *= (multiplier / 5.0f);

            // 生成
            var entity = commandBuffer.Instantiate(prefabEntity3);
            commandBuffer.SetComponent(entity, new Translation
            {
                Value = translation
            });

            commandBuffer.SetComponent(entity, new Rotation
            {
                Value = localToWorld.Rotation
            });

            commandBuffer.SetComponent(entity, bulletstatus);
        }

        /// <summary>
        /// レート時間を登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="phase1">第一段階のレート</param>
        /// <param name="phase2">第二段階のレート</param>
        /// <param name="phase3">第三段階のレート</param>
        public void SetInterval(float phase1, float phase2, float phase3)
        {
            _intervalThreshold_in_Phase1 = phase1;
            _intervalThreshold_in_Phase2 = phase2;
            _intervalThreshold_in_Phase3 = phase3;
        }
    }
}