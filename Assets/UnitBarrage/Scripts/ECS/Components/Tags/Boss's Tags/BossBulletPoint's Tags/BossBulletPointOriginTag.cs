using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 発射台の親であることを示すタグ（回転のためだけに使用している）
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BossBulletPointOriginTag : IComponentData { }
}