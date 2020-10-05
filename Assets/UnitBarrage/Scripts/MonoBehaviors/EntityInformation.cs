using UnityEngine;
using Unity.Entities;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 今後複製するのに必要なEntity（GameObjcectから変換した）をstaticに持つクラス
    /// </summary>
    public class EntityInformation : MonoBehaviour, IConvertGameObjectToEntity
    {
        #region Public Variable
        /// <summary>
        /// プレイヤーのEntity
        /// </summary>
        [HideInInspector]
        public static Entity PlayerEntity { get; private set; }

        /// <summary>
        /// 敵のEntity
        /// </summary>
        [HideInInspector]
        public static Entity BossEntity { get; private set; }

        /// <summary>
        /// プレイヤーが出す弾の元になるEntity
        /// </summary>
        [HideInInspector]
        public static Entity PlayerBulletEntityPrefab { get; private set; }

        /// <summary>
        /// 敵が出す弾の元になるEntity（攻撃段階ごとで要素に入っている）
        /// </summary>
        [HideInInspector]
        public static Entity[] BossBulletEntityPrefab { get; private set; }
        #endregion

        #region Serialize Variable
        [SerializeField]
        private GameObject _playerBulletGameObject;

        [SerializeField]
        private GameObject _bossBullet1GameObject;

        [SerializeField]
        private GameObject _bossBullet2GameObject;

        [SerializeField]
        private GameObject _bossBullet3GameObject;
        #endregion

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Entity prefabEntity;
            BossBulletEntityPrefab = new Entity[3];

            // BlobAssetStoreはconversionSystemのプロパティを使っている
            // プレイヤー弾の変換
            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_playerBulletGameObject, GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore));
            PlayerBulletEntityPrefab = prefabEntity;

            // 敵弾の変換
            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_bossBullet1GameObject, GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore));
            BossBulletEntityPrefab[0] = prefabEntity;

            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_bossBullet2GameObject, GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore));
            BossBulletEntityPrefab[1] = prefabEntity;

            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_bossBullet3GameObject, GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore));
            BossBulletEntityPrefab[2] = prefabEntity;
        }

        /// <summary>
        /// プレイヤーEntityを登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="entity">登録したいEntity</param>
        public static void SetPlayerEntity(Entity entity)
        {
            PlayerEntity = entity;
        }

        /// <summary>
        /// 敵のEntityを登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="entity">登録したいEntity</param>
        public static void SetBossEntity(Entity entity)
        {
            BossEntity = entity;
        }
    }
}