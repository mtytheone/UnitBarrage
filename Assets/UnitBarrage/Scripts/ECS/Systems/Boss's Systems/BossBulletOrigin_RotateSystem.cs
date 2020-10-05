using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 敵の回転を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(Boss_RotateSystem))]
    public class BossBulletOrigin_RotateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var gameManager = GameManager.Instance;

            if (gameManager.GameMode == Define.GameStatus.Playing || gameManager.GameMode == Define.GameStatus.Waiting)
            {
                var normalTime = (float)Time.ElapsedTime;
                var deltaTime = Time.DeltaTime;

                // 発射台の親の回転
                // 分散並列スレッド処理
                Entities
                    .WithName("BossBulletOriginRotation")
                    .WithAll<BossBulletPointOriginTag>()
                    .ForEach((ref Rotation rotation, in BulletPointRotationData rotationData) =>
                    {
                        quaternion normalizedRotation = math.normalizesafe(rotation.Value);
                        quaternion angleToRotate = quaternion.AxisAngle(Define.ZAXIS, rotationData.Speed * deltaTime);

                        rotation.Value = math.mul(normalizedRotation, angleToRotate);

                    }).ScheduleParallel();
            }
        }
    }
}