using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自機弾の移動を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Player_ShotBulletSystem))]
    public class PlayerBullet_MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (GameManager.Instance.GameMode != Define.GameStatus.Pausing)
            {
                var deltaTime = Time.DeltaTime;

                // 弾の移動
                // 並列スレッド処理
                Entities
                    .WithName("PlayerBulletMovement")
                    .WithAll<PlayerBulletTag>()
                    .ForEach((ref Translation translation, in BulletStatusData bulletStatus) =>
                    {
                        translation.Value.y += bulletStatus.BulletSpeed * deltaTime;

                    }).Schedule();
            }
        }
    }
}