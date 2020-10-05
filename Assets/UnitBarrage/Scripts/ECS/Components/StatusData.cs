using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤー及び敵のHP等のステータスを保持するデータ
    /// </summary>
    [System.Serializable]
    public struct StatusData : IComponentData
    {
        /// <summary>
        /// HP
        /// </summary>
        public float HP;
    }
}