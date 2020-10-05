using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    [GenerateAuthoringComponent]
    public struct BossRotationData : IComponentData
    {
        /// <summary>
        /// ボスの回転速度
        /// </summary>
        public float RotationSpeed;
        /// <summary>
        /// ボスの回転の位相周期
        /// </summary>
        public float RotationFrequency;
    }
}