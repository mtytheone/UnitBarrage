using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が1つ目の敵弾であることを示すタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletTag1 : IComponentData { }
}