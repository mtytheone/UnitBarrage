using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーの諸処理に使う設定を保存しているデータ
    /// </summary>
    [GenerateAuthoringComponent]
    public struct PlayerSettingData : IComponentData
    {
        /// <summary>
        /// 動かす方向ベクトル
        /// </summary>
        [HideInInspector]
        public float3 MoveDirection;

        /// <summary>
        /// 移動速度
        /// </summary>
        public float MoveSpeed;

        /// <summary>
        /// 速度係数
        /// </summary>
        [HideInInspector]
        public float MoveSpeedMultiplier;

        /// <summary>
        /// 低速移動キーが押されているかどうか
        /// </summary>
        [HideInInspector]
        public bool IsPressedShotKey;
    }
}