using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 敵の弾を消去するシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BossBullet_MovementSystem))]
    public class BossBullet_DestroySystem : SystemBase
    {
        /// <summary>
        /// EntityCommandBuffer
        /// </summary>
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        /// <summary>
        /// メインカメラ
        /// </summary>
        private Camera _mainCamera;
        /// <summary>
        /// 画面左上（補正値）のワールド座標
        /// </summary>
        private Vector2 _min;
        /// <summary>
        /// 画面右上（補正値）のワールド座標
        /// </summary>
        private Vector2 _max;

        protected override void OnCreate()
        {
            // EntityCommandBufferの取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            // 画面左下と右上のワールド座標を取得
            _mainCamera = Camera.main;
            _min = _mainCamera.ViewportToWorldPoint(new Vector2(-0.1f, -0.25f));
            _max = _mainCamera.ViewportToWorldPoint(new Vector2(1.1f, 1.25f));
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // 弾が範囲外に出たら消す
            // メインスレッド処理
            Entities
                .WithName("DestroyBossBulletOffScreen")
                .WithoutBurst()
                .WithAny<BossBulletTag1, BossBulletTag2, BossBulletTag3>()
                .ForEach((Entity entity, ref Translation translation) =>
                {
                // 範囲外に出たら消す
                if (translation.Value.x > _max.x || translation.Value.x < _min.x || translation.Value.y > _max.y || translation.Value.y < _min.y)
                    {
                        commandBuffer.DestroyEntity(entity);
                    }

                }).Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}