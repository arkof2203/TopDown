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

		public event Action<ElementalType> OnBuyTowerHandler;

		public void SetTowerParams(Tower[] towers)
		{//todo crutch work
			for (int i = 0; i < towers.Length; i++)
			{
				var tower = towers[i];
				var conf = tower.AttackElemental switch
				{
					ElementalType.Physic => _physicTowerButton,
					ElementalType.Fire => _fireTowerButton,
					ElementalType.Ice => _iceTowerButton,
					_ => throw new ArgumentOutOfRangeException()
				};

				conf.Cost = tower.Progress[0].Cost;
				conf.Description = string.Empty;
				conf.Name = tower.name;
			}
		}
		
		private void Awake()
		{
			_physicTowerButton.Button.onClick.AddListener(BuyPhysic);
			_fireTowerButton.Button.onClick.AddListener(BuyFire);
			_iceTowerButton.Button.onClick.AddListener(BuyIce);
		}

		private void OnDestroy()
		{
			_physicTowerButton.Button.onClick.RemoveListener(BuyPhysic);
			_fireTowerButton.Button.onClick.RemoveListener(BuyFire);
			_iceTowerButton.Button.onClick.RemoveListener(BuyIce);
		}

		private void BuyPhysic()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Physic);
		private void BuyFire()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Fire);
		private void BuyIce()
			=> OnBuyTowerHandler?.Invoke(ElementalType.Ice);
	}
}