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
    [UpdateAfter(typeof(UpdateWorldTimeSystem))]
    public class Boss_RotateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (GameManager.Instance.GameMode != Define.GameStatus.Pausing)
            {
                var normalTime = (float)Time.ElapsedTime;
                var deltaTime = Time.DeltaTime;

                // 敵の回転
                // 分散並列スレッド処理
                Entities
                    .WithName("BossRotation")
                    .WithAll<BossTag>()
                    .ForEach((ref Rotation rotation, in BossRotationData rotationData) =>
                    {
                        // 回転速度
                        float speed = rotationData.RotationSpeed * (math.sin(rotationData.RotationFrequency * normalTime)) + (rotationData.RotationSpeed / 2.0f + 1.0f);

                        // Z軸回転をするクォータニオンを求める
                        quaternion normalizedRotation = math.normalizesafe(rotation.Value);
                        quaternion angleToRotate = quaternion.AxisAngle(Define.ZAXIS, speed * deltaTime);

                        // 回転を反映
                        rotation.Value = math.mul(normalizedRotation, angleToRotate);

                    }).ScheduleParallel();
            }
        }
    }
}