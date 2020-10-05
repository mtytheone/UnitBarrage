using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身がの敵弾の発射台であることを示すベースタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletPointTag : IComponentData
    {
        public float Interval;
    }
}