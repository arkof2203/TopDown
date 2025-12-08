using System;
using Netologia.Systems;
using Netologia.TowerDefence.Interface;
using Netologia.TowerDefence.Settings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Netologia.TowerDefence.Behaviors
{
	public class CellController : MonoBehaviour
	{
		private TowerSystem _towers;			//injected
		private Constants _constants;			//injected
		private SelectCell[] _cells;

		private bool _hasSelect;
		private SelectCell _current;

		[SerializeField]
		private GameObject _buildPanel;
		[SerializeField]
		private Button _closeButton;
		[SerializeField]
		private Button _upgradeButton;
		[SerializeField]
		private Button _sellButton;
		[SerializeField]
		private BuyTowerPanel _buyTowerPanel;
		[SerializeField]
		private UpgradeTowerPanel _upgradeTowerPanel;

		[SerializeField]
		private Tower[] _prefabs;

		public event Func<int, bool> OnTryGoldChanged; 

		private void OnSelectCellHandler(SelectCell obj)
		{
			if(_hasSelect)
				_current.Unselect();

			_current = obj;
			_hasSelect = true;
			
			_buildPanel.SetActive(true);
			if (obj.HasTower)
			{
				_buyTowerPanel.gameObject.SetActive(false);
				_upgradeTowerPanel.gameObject.SetActive(true);
				_upgradeTowerPanel.SetTarget(_current.Tower);
				_upgradeButton.interactable = _current.Tower.CanLevelUp;
				_sellButton.interactable = true;
			}
			else
			{
				_buyTowerPanel.gameObject.SetActive(true);
				_upgradeTowerPanel.gameObject.SetActive(false);
				_upgradeButton.interactable = false;
				_sellButton.interactable = false;
			}
		}

		private void OnCloseBuildPanel()
		{
			_buildPanel.SetActive(false);
			_current.Unselect();
			_current = null;
			_hasSelect = false;
		}

		private void OnUpgradeTower()
		{
			var tower = _current.Tower;
			if (OnTryGoldChanged.Invoke(-tower.Progress[tower.Level].Cost))
			{
				_current.Tower.LevelUp();
				OnSelectCellHandler(_current);//rewrite interface
				_upgradeButton.interactable = _current.Tower.CanLevelUp;
			}
		}
		
		private void OnCreateTower(ElementalType obj)
		{
			var index = Array.FindIndex(_prefabs, t => t.AttackElemental == obj);
			if (OnTryGoldChanged.Invoke(-_prefabs[index].Progress[0].Cost))
			{
				var tower = _towers[_prefabs[index]].Get;
				_current.SetTower(tower);
			
				OnCloseBuildPanel();
			}
		}

		private void OnSellTower()
		{
			var tower = _current.Tower;
			var value = 0;
			for (int i = 0, iMax = tower.Level; i <= iMax; i++)
				value += tower.Progress[i].Cost;

			OnTryGoldChanged.Invoke(Mathf.RoundToInt(value * _constants.SellPercent));
			_current.RemoveTower();
			
			value = Array.FindIndex(_prefabs, t => t.AttackElemental == tower.AttackElemental);
			_towers[_prefabs[value]].ReturnElement(tower);
			OnCloseBuildPanel();
		}
		
		private void Awake()
		{
			_cells = FindObjectsOfType<SelectCell>();
			for (int i = 0, iMax = _cells.Length; i < iMax; i++)
				_cells[i].OnSelectCellHandler += OnSelectCellHandler;
			
			_closeButton.onClick.AddListener(OnCloseBuildPanel);
			_upgradeButton.onClick.AddListener(OnUpgradeTower);
			_sellButton.onClick.AddListener(OnSellTower);
			_buyTowerPanel.OnBuyTowerHandler += OnCreateTower;

			_buyTowerPanel.SetTowerParams(_prefabs);
		}

		

		private void OnDestroy()
		{
			for (int i = 0, iMax = _cells.Length; i < iMax; i++)
				_cells[i].OnSelectCellHandler -= OnSelectCellHandler;
			
			_closeButton.onClick.RemoveListener(OnCloseBuildPanel);
			_upgradeButton.onClick.RemoveListener(OnUpgradeTower);
			_sellButton.onClick.RemoveListener(OnSellTower);
			_buyTowerPanel.OnBuyTowerHandler -= OnCreateTower;
		}

		[Inject]
		private void Construct(TowerSystem towers, Constants constants)
			=> (_towers, _constants) = (towers, constants);
	}
}