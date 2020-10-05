using UnityEngine;
using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// PlayerTagをGameObjectのComponentに加えるための変換クラス
    /// （EntityManagerに登録するためにカスタムオーサリングをしている）
    /// </summary>
    public class PlayerTagCustomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // 変換したEntityにプレイヤータグを追加
            dstManager.AddComponentData(entity, new PlayerTag());

            // HPの初期化
            dstManager.AddComponentData(entity, new StatusData
            {
                HP = GameManager.Instance.PlayerManager.PlayerSettingdata.InitialHP
            });

            // EntityInformationに登録
            EntityInformation.SetPlayerEntity(entity);
        }
    }
}