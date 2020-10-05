using Unity.Entities;
using Unity.Jobs;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーのHPが無くなったときの処理をするSystem
    /// </summary>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Player_TriggerBulletSystem))]
    public class Player_StatusControlSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            // EntityCommandBufferの取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // プレイヤーのHPが0以下になったらゲームを終了する
            // メインスレッド処理
            Entities
                .WithName("DestroyPlayerForPlayerHP")
                .WithoutBurst()
                .WithAll<PlayerTag>()
                .ForEach((Entity entity, in StatusData statusData) =>
                {
                    if (statusData.HP <= 0)
                    {
                        commandBuffer.DestroyEntity(entity);
                        GameManager.Instance.SceneController.EndGame();
                    }

                }).Run();

            // EntityCommandBuffer経由でJobを登録
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}