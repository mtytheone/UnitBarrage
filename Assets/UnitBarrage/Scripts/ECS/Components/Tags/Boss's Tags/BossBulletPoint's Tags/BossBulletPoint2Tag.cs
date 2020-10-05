using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が2つ目の敵弾の発射台であることを示すタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletPoint2Tag : IComponentData { }
}