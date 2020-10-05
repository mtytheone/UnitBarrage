using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーと敵の弾との衝突判定とそれによる処理を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class Player_TriggerBulletSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            _commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (GameManager.Instance.GameMode == Define.GameStatus.Playing && EntityManager.GetComponentData<StatusData>(EntityInformation.BossEntity).HP > 0)
            {
                // Jobの生成とメンバーの登録
                var job = new PlayerTriggerSystemJob();
                job.AllEnemies = GetComponentDataFromEntity<BossTag>(true);
                job.AllPlayers = GetComponentDataFromEntity<PlayerTag>(true);
                job.AllBossBullets = GetComponentDataFromEntity<BossBulletTag>(true);
                job.AllStatuses = GetComponentDataFromEntity<StatusData>(true);
                job.EntityCommandBuffer = _commandBufferSystem.CreateCommandBuffer();

                // 依存関係を反映
                Dependency = job.Schedule(
                    _stepPhysicsWorld.Simulation,
                    ref _buildPhysicsWorld.PhysicsWorld,
                    Dependency
                    );

                // EntityCommandBuffer経由でJobを登録
                _commandBufferSystem.AddJobHandleForProducer(Dependency);
            }
        }



        [BurstCompile]
        struct PlayerTriggerSystemJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<BossTag> AllEnemies;
            [ReadOnly] public ComponentDataFromEntity<BossBulletTag> AllBossBullets;
            [ReadOnly] public ComponentDataFromEntity<PlayerTag> AllPlayers;
            [ReadOnly] public ComponentDataFromEntity<StatusData> AllStatuses;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                // 当たったのが共に弾だったら何もしない
                if (AllPlayers.HasComponent(entityA) && AllPlayers.HasComponent(entityB)) return;

                // 片方が弾でもう片方が敵だったら、弾は消してプレイヤーの体力を1つ減らす
                if (AllPlayers.HasComponent(entityA) && AllBossBullets.HasComponent(entityB)) //プレイヤー対敵弾
                {
                    EntityCommandBuffer.SetComponent(entityA, new StatusData
                    {
                        HP = AllStatuses[entityA].HP - 1.0f
                    });
                    EntityCommandBuffer.DestroyEntity(entityB);
                }
                else if (AllBossBullets.HasComponent(entityA) && AllPlayers.HasComponent(entityB)) //敵弾対プレイヤー
                {
                    EntityCommandBuffer.SetComponent(entityB, new StatusData
                    {
                        HP = AllStatuses[entityB].HP - 1.0f
                    });
                    EntityCommandBuffer.DestroyEntity(entityA);
                }
                else if (AllPlayers.HasComponent(entityA) && AllEnemies.HasComponent(entityB)) //プレイヤー対敵
                {
                    EntityCommandBuffer.SetComponent(entityA, new StatusData
                    {
                        HP = AllStatuses[entityA].HP - 1.0f
                    });
                }
                else if (AllEnemies.HasComponent(entityA) && AllPlayers.HasComponent(entityB)) //敵対プレイヤー
                {
                    EntityCommandBuffer.SetComponent(entityB, new StatusData
                    {
                        HP = AllStatuses[entityB].HP - 1.0f
                    });
                }
            }
        }

    }
}