using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 弾の状態を保持するデータ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BulletStatusData : IComponentData
    {
        /// <summary>
        /// 弾速度
        /// </summary>
        public float BulletSpeed;
    }
}