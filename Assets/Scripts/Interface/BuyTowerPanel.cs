using System;
using UnityEngine;
using UnityEngine.UI;

namespace Netologia.TowerDefence.Interface
{
	public class BuyTowerPanel : MonoBehaviour
	{
		[SerializeField]
		private TowerButton _physicTowerButton;
		[SerializeField]
		private TowerButton _fireTowerButton;
		[SerializeField]
		private TowerButton _iceTowerButton;
        [SerializeField]
        private TowerButton _barrackButton;

        public event Action<ElementalType> OnBuyTowerHandler;
        public event Action OnBuyBarrackHandler;

        public void SetTowerParams(Tower[] towers)
        {
            for (int i = 0; i < towers.Length; i++)
            {
                var tower = towers[i];

                TowerButton conf = null;
                switch (tower.AttackElemental)
                {
                    case ElementalType.Physic: conf = _physicTowerButton; break;
                    case ElementalType.Fire: conf = _fireTowerButton; break;
                    case ElementalType.Ice: conf = _iceTowerButton; break;
                    default:
                        Debug.LogWarning($"[BuyTowerPanel] Unsupported tower element '{tower.AttackElemental}' for {tower.name}. Skipped.");
                        continue; // <-- ВАЖНО: не падаем
                }

                conf.Cost = tower.Progress[0].Cost;
                conf.Description = string.Empty;
                conf.Name = tower.name;
            }
        }


        public void SetBarrackParams(Tower barrackPrefab)
        {
            if (barrackPrefab == null) return;

            _barrackButton.Cost = barrackPrefab.Progress[0].Cost;
            _barrackButton.Description = string.Empty;
            _barrackButton.Name = barrackPrefab.name;
        }

        private void Awake()
		{
			_physicTowerButton.Button.onClick.AddListener(BuyPhysic);
			_fireTowerButton.Button.onClick.AddListener(BuyFire);
			_iceTowerButton.Button.onClick.AddListener(BuyIce);
            _barrackButton.Button.onClick.AddListener(BuyBarrack);
        }

		private void OnDestroy()
		{
			_physicTowerButton.Button.onClick.RemoveListener(BuyPhysic);
			_fireTowerButton.Button.onClick.RemoveListener(BuyFire);
			_iceTowerButton.Button.onClick.RemoveListener(BuyIce);
            _barrackButton.Button.onClick.RemoveListener(BuyBarrack);
        }

		private void BuyPhysic()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Physic);
		private void BuyFire()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Fire);
		private void BuyIce()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Ice);
        private void BuyBarrack()
            => OnBuyBarrackHandler?.Invoke();
    }
}