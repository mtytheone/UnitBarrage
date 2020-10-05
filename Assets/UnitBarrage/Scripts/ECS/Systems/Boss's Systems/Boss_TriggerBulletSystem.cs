using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーの弾と敵との衝突判定とそれによる処理を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class Boss_TriggerBulletSystem : SystemBase
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
                var job = new BossTriggerSystemJob();
                job.AllBullets = GetComponentDataFromEntity<PlayerBulletTag>(true);
                job.AllEnemies = GetComponentDataFromEntity<BossTag>(true);
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
        struct BossTriggerSystemJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<PlayerBulletTag> AllBullets;
            [ReadOnly] public ComponentDataFromEntity<BossTag> AllEnemies;
            [ReadOnly] public ComponentDataFromEntity<StatusData> AllStatuses;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                // 当たったのが共に弾だったら何もしない
                if (AllBullets.HasComponent(entityA) && AllBullets.HasComponent(entityB)) return;

                // 片方が弾でもう片方が敵だったら、敵のHPを引いて弾を消す
                if (AllEnemies.HasComponent(entityA) && AllBullets.HasComponent(entityB))
                {
                    EntityCommandBuffer.SetComponent(entityA, new StatusData
                    {
                        HP = AllStatuses[entityA].HP - 1.0f
                    });

                    EntityCommandBuffer.DestroyEntity(entityB);
                }
                else if (AllBullets.HasComponent(entityA) && AllEnemies.HasComponent(entityB))
                {
                    EntityCommandBuffer.SetComponent(entityB, new StatusData
                    {
                        HP = AllStatuses[entityB].HP - 1.0f
                    });

                    EntityCommandBuffer.DestroyEntity(entityA);
                }
            }
        }
    }
}