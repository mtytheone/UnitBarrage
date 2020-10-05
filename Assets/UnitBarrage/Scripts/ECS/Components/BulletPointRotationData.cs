using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    [GenerateAuthoringComponent]
    public struct BulletPointRotationData : IComponentData
    {
        /// <summary>
        /// 発射台の回転速度
        /// </summary>
        public float Speed;
    }
}