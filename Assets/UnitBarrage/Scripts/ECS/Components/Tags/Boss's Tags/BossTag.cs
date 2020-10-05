using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 自身が敵であることを示すタグ
    /// </summary>
    [System.Serializable]
    public struct BossTag : IComponentData { }
}