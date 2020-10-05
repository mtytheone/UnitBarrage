using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が1つ目の敵弾の発射台であることを示すタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletPoint1Tag : IComponentData { }
}