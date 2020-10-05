using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 敵の弾の移動を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Boss_BulletShotSystem))]
    public class BossBullet_MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (GameManager.Instance.GameMode != Define.GameStatus.Pausing)
            {
                var deltaTime = Time.DeltaTime;

                // 弾の移動（第一段階）
                // 分散並列スレッド
                Entities
                    .WithName("BossBulletMovement1")
                    .WithAll<BossBulletTag1>()
                    .ForEach((ref Translation translation, in LocalToWorld localToWorld, in BulletStatusData bulletStatus) =>
                    {
                    // 進行方向に移動
                    translation.Value += localToWorld.Up * bulletStatus.BulletSpeed * deltaTime;

                    }).ScheduleParallel();

                // 弾の移動（第二段階）
                // 分散並列スレッド
                Entities
                    .WithName("BossBulletMovement2")
                    .WithAll<BossBulletTag2>()
                    .ForEach((ref Translation translation, in PhysicsVelocity velocity, in BulletStatusData bulletStatus) =>
                    {
                    // 自機狙いで移動（向くベクトルはvelocity.Linearに書き込み済）
                    translation.Value += velocity.Linear * bulletStatus.BulletSpeed * deltaTime;

                    }).ScheduleParallel();

                // 弾の移動（第三段階）
                // 分散並列スレッド
                Entities
                    .WithName("BossBulletMovement3")
                    .WithAll<BossBulletTag3>()
                    .ForEach((ref Translation translation, in LocalToWorld localToWorld, in BulletStatusData bulletStatus) =>
                    {
                        translation.Value += localToWorld.Up * bulletStatus.BulletSpeed * deltaTime;

                    }).ScheduleParallel();
            }
        }
    }
}