using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// キー入力を反映させるシステム
    /// </summary>
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(Player_ShotBulletSystem))]
    public class Player_InputSystem : SystemBase, InputControlData.IPlayerActions
    {
        /// <summary>
        /// InputActionsのデータ
        /// </summary>
        private InputControlData _inputActions;

        /// <summary>
        /// PlayerTagのクエリ
        /// </summary>
        private EntityQuery _playerQuery;

        /// <summary>
        /// プレイヤーの移動方向
        /// </summary>
        private Vector2 _playerDirection;
        /// <summary>
        /// ショットボタンを押しているかどうか
        /// </summary>
        private bool _isPressedShotButton;
        /// <summary>
        /// 低速移動ボタンを押しているかどうか
        /// </summary>
        private bool _isPressedSlowButton;

        protected override void OnCreate()
        {
            // InputSystemの作成
            _inputActions = new InputControlData();
            _inputActions.Player.SetCallbacks(this);

            // クエリの読み込み
            _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        }

        protected override void OnStartRunning()
        {
            // 起動
            _inputActions.Enable();
        }

        protected override void OnUpdate()
        {
            if (GameManager.Instance.GameMode != Define.GameStatus.Pausing)
            {
                // PlayerのEntityが存在する場合のみ
                if (_playerQuery.CalculateEntityCount() != 0)
                {
                    // 入力によるプレイヤーの移動
                    // メインスレッド処理
                    Entities
                        .WithName("Player_Input")
                        .WithoutBurst()
                        .WithAll<PlayerTag>()
                        .ForEach((ref PlayerSettingData playerData) =>
                        {
                        // 入力の反映
                        playerData.IsPressedShotKey = _isPressedShotButton;
                            playerData.MoveSpeedMultiplier = _isPressedSlowButton ? 0.4f : 1.0f;
                            playerData.MoveDirection = new float3(_playerDirection.x, _playerDirection.y, 0);

                        }).Run();
                }
            }
        }

        protected override void OnStopRunning()
        {
            // 無効化
            _inputActions.Disable();
        }

        protected override void OnDestroy()
        {
            // 開放
            _inputActions.Dispose();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _playerDirection = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            _isPressedShotButton = context.performed;
        }

        public void OnSlow(InputAction.CallbackContext context)
        {
            _isPressedSlowButton = context.performed;
        }

        public void OnSubmit(InputAction.CallbackContext context) { }

        public void OnPause(InputAction.CallbackContext context) { }
    }
}