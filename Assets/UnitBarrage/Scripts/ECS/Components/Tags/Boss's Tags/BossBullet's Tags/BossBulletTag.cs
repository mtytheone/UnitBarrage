using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が敵弾であることを示すベースタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletTag : IComponentData { }
}