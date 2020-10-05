using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が自機弾であることを示すタグ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct PlayerBulletTag : IComponentData { }
}