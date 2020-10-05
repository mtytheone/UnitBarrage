using UnityEngine;
using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 消すときに子オブジェクトも一緒に消す親オブジェクトに付けるCustomAuthoringComponent
    /// 参考サイト : http://tsubakit1.hateblo.jp/entry/2019/10/21/224421
    /// </summary>
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class LinkedEntityGroupComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var buffer = dstManager.AddBuffer<LinkedEntityGroup>(entity);

            var children = transform.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                var childEntity = conversionSystem.GetPrimaryEntity(child.gameObject);
                buffer.Add(childEntity);
            }
        }
    }
}