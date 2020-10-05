using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// 入力に応じた移動を行うシステム
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(Player_InputSystem))]
    public class Player_MovementSystem : SystemBase
    {
        /// <summary>
        /// PlayerTagのクエリ
        /// </summary>
        private EntityQuery _playerQuery;

        /// <summary>
        /// メインカメラ
        /// </summary>
        private Camera _mainCamera;
        /// <summary>
        /// 画面左下（補正値）のワールド座標
        /// </summary>
        private Vector2 _min;
        /// <summary>
        /// 画面右上（補正値）のワールド座標
        /// </summary>
        private Vector2 _max;

        protected override void OnCreate()
        {
            // クエリの読み込み
            _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        }

        protected override void OnUpdate()
        {
            // PlayerのEntityが存在する場合のみ
            if (_playerQuery.CalculateEntityCount() != 0)
            {
                var deltaTime = Time.DeltaTime;

                Entities
                    .WithName("Player_Movement")
                    .WithoutBurst()
                    .WithAll<PlayerTag>()
                    .ForEach((ref Translation translation, ref PlayerSettingData playerData) =>
                    {
                    // プレイヤーの移動
                    var moveDirection = math.normalizesafe(playerData.MoveDirection);
                        translation.Value += moveDirection * playerData.MoveSpeed * playerData.MoveSpeedMultiplier * deltaTime;

                    // Clamp処理
                    translation.Value.x = math.clamp(translation.Value.x, _min.x, _max.x);
                        translation.Value.y = math.clamp(translation.Value.y, _min.y, _max.y);

                    }).Run();
            }
        }

        public void SetBounds(float xMin, float xMax, float yMin, float yMax)
        {
            // 画面左下と右上のワールド座標を取得
            _mainCamera = Camera.main;
            _min = _mainCamera.ViewportToWorldPoint(new Vector2(xMin, yMin));
            _max = _mainCamera.ViewportToWorldPoint(new Vector2(xMax, yMax));
        }
    }
}