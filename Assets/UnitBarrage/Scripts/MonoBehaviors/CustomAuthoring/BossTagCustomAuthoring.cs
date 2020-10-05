using UnityEngine;
using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// BossTagをGameObjectのComponentに加えるための変換クラス
    /// （EntityManagerに登録するためにカスタムオーサリングをしている）
    /// </summary>
    public class BossTagCustomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BossTag());
            // HPの初期化
            dstManager.AddComponentData(entity, new StatusData
            {
                HP = GameManager.Instance.BossManager.BossSettingData.InitialBossHP
            });
            EntityInformation.SetBossEntity(entity);
        }
    }
}