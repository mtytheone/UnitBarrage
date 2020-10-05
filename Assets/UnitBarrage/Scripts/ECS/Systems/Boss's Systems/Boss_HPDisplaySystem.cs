using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// ボスのHPをHPバーに反映するためのSystem
    /// </summary>
    [UpdateAfter(typeof(Boss_StatusControlSystem))]
    public class Boss_HPDisplaySystem : SystemBase
    {
        /// <summary>
        /// 画面上部にあるHPバー
        /// </summary>
        private Slider _hpBar;
        /// <summary>
        /// 経過時間
        /// </summary>
        private float _elapsedTime;
        /// <summary>
        /// Lerpの割合
        /// </summary>
        private float _lerpParameter;

        protected override void OnCreate()
        {
            _elapsedTime = 0;
            _lerpParameter = 0;
        }

        protected override void OnUpdate()
        {
            var gameManager = GameManager.Instance;
            var bossEntity = EntityInformation.BossEntity;

            // プレイ中
            if (gameManager.GameMode == Define.GameStatus.Playing)
            {
                // 時間変数たちは初期化
                _elapsedTime = 0;
                _lerpParameter = 0;

                // 敵のEntityが存在していたら
                if (EntityManager.Exists(bossEntity))
                {
                    // HBバーに反映
                    var bossStatus = GetComponent<StatusData>(bossEntity);
                    _hpBar.value = bossStatus.HP;
                }
            }
            // 待機状態
            else if (gameManager.GameMode == Define.GameStatus.Waiting)
            {
                if (_hpBar != null)
                {
                    // 経過時間からHPバーの位置を反映（チャージアニメーション）
                    _elapsedTime += Time.DeltaTime;
                    _lerpParameter = (_elapsedTime / 4.0f);
                    _hpBar.value = math.lerp(_hpBar.minValue, _hpBar.maxValue, _lerpParameter);
                }
            }
        }

        /// <summary>
        /// タイムの初期化をする際に外部から呼ぶ関数
        /// </summary>
        public void Iniialize()
        {
            _elapsedTime = 0;
            _lerpParameter = 0;
        }

        /// <summary>
        /// 扱うスライダーを登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="slider">登録したいスライダー</param>
        public void SetSlider(Slider slider)
        {
            _hpBar = slider;
        }

        /// <summary>
        /// スライダーの最大値を設定するために外部から呼ぶ関数
        /// </summary>
        /// <param name="max">最大値</param>
        public void SetMaxValue(float max)
        {
            _hpBar.maxValue = max;
        }

        /// <summary>
        /// スライダーの色を変えるために外部から呼ぶ関数
        /// </summary>
        /// <param name="color">指定色</param>
        public void SetColor(Color color)
        {
            // SliderのFillオブジェクトを取得
            var image = _hpBar.gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>();
            if (image != null)
            {
                // 色を更新
                image.color = color;
            }
        }
    }
}