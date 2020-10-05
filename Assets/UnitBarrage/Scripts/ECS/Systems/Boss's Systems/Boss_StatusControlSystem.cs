using Unity.Entities;
using Unity.Jobs;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 敵のHPが0になったら消すシステム
    /// </summary>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Boss_TriggerBulletSystem))]
    public class Boss_StatusControlSystem : SystemBase
    {
        /// <summary>
        /// EntityCommandBuffer
        /// </summary>
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            // EntiryCommandBufferを取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var gameManager = GameManager.Instance;
            var bossManager = gameManager.BossManager;
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // HPに応じた攻撃段階の制御
            // メインスレッド処理
            Entities
                .WithName("DestroyBossForBossHP")
                .WithoutBurst()
                .WithAll<BossTag>()
                .ForEach((Entity entity, ref StatusData statusData) =>
                {
                    if (gameManager.GameMode == Define.GameStatus.Playing)
                    {
                        if (bossManager.EqualBossPhaseCount(0)) //第一段階なら
                        {
                            MovePhase1to2(ref statusData, gameManager);
                        }
                        else if (bossManager.EqualBossPhaseCount(1)) //第二段階なら
                        {
                            MovePhase2to3(ref statusData, gameManager);
                        }
                        else if (bossManager.EqualBossPhaseCount(2))  // 最終段階なら
                        {
                            MovePhase3toEnd(entity, ref statusData, gameManager, commandBuffer);
                        }
                    }

                }).Run();

            // EntityCommandBuffer経由でJobを登録
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }


        private void MovePhase1to2(ref StatusData statusData, GameManager gameManager)
        {
            // HPが0になったら
            if (statusData.HP == 0)
            {
                // ゲームを待機状態にする
                gameManager.GameMode = Define.GameStatus.Waiting;

                // HPを更新
                var nextHP = gameManager.BossManager.BossSettingData.InitialBossHP;
                statusData.HP = nextHP;
                var bossHPDisplay = World.GetOrCreateSystem<Boss_HPDisplaySystem>();
                bossHPDisplay.SetMaxValue(nextHP);

                // 攻撃段階を上げる
                gameManager.UpdateBossPhase();
            }
        }

        private void MovePhase2to3(ref StatusData statusData, GameManager gameManager)
        {
            // HPが0になったら
            if (statusData.HP == 0)
            {
                // ゲームを待機状態にする
                gameManager.GameMode = Define.GameStatus.Waiting;

                // HPを更新
                var nextHP = gameManager.BossManager.BossSettingData.ThirdBossHP;
                statusData.HP = nextHP;
                var bossHPDisplay = World.GetOrCreateSystem<Boss_HPDisplaySystem>();
                bossHPDisplay.SetMaxValue(nextHP);

                // HPバーを赤くする
                bossHPDisplay.SetColor(UnityEngine.Color.red);

                // 攻撃段階を上げる
                gameManager.UpdateBossPhase();
            }
        }

        private static void MovePhase3toEnd(Entity entity, ref StatusData statusData, GameManager gameManager, EntityCommandBuffer commandBuffer)
        {
            // HPが0になったら
            if (statusData.HP == 0)
            {
                // ゲームを終了状態にする
                gameManager.GameMode = Define.GameStatus.End;

                // 敵を破壊
                commandBuffer.DestroyEntity(entity);
                // 攻撃段階を上げる
                gameManager.UpdateBossPhase();
            }
        }
    }
}