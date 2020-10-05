using Unity.Entities;
using UnityEngine.UI;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// プレイヤーのHPをHPバーに反映するためのSystem
    /// </summary>
    [UpdateAfter(typeof(Player_TriggerBulletSystem))]
    public class Player_HPDisplaySystem : SystemBase
    {
        /// <summary>
        /// 画面右部にあるHPバー
        /// </summary>
        private Slider _hpBar;

        protected override void OnUpdate()
        {
            var playerEntity = EntityInformation.PlayerEntity;

            if (EntityManager.Exists(playerEntity))
            {
                var playerStatus = GetComponent<StatusData>(playerEntity);
                _hpBar.value = playerStatus.HP;
            }
        }

        /// <summary>
        /// 扱うスライダーを登録するために外部から呼ぶ関数
        /// </summary>
        /// <param name="slider">登録したいスライダー</param>
        public void SetSlider(Slider slider)
        {
            _hpBar = slider;
        }
    }
}