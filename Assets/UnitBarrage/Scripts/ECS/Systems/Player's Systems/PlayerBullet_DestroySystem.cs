using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自機弾の消去を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerBullet_MovementSystem))]
    public class PlayerBullet_DestroySystem : SystemBase
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
        /// 画面右上（補正値）のワールド座標
        /// </summary>
        private Vector2 _max;

        protected override void OnCreate()
        {
            // EntityCommandBufferの取得
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();

            // 弾が範囲外に出たら消す
            // メインスレッド処理
            Entities
                .WithName("DestroyPlayerBulletOffScreen")
                .WithoutBurst()
                .WithAll<PlayerBulletTag>()
                .ForEach((Entity entity, in Translation translation) =>
                {
                    if (translation.Value.y > _max.y)
                    {
                        commandBuffer.DestroyEntity(entity);
                    }

                }).Run();

            // EntityCommandBuffer経由でJobを登録
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        public void SetBounds(float xMax, float yMax)
        {
            // 画面右上のワールド座標を取得
            _mainCamera = Camera.main;
            _max = _mainCamera.ViewportToWorldPoint(new Vector2(xMax, yMax));
        }
    }
}