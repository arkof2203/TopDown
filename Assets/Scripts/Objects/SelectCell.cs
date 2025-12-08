using System;
using Netologia.TowerDefence.Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.TowerDefence
{
	public class SelectCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private bool _selected;

		[SerializeField]
		private SpriteRenderer _border;

		[SerializeField]
		private CellPresetSettings _settings;

		public event Action<SelectCell> OnSelectCellHandler;

		public bool HasTower { get; private set; }
		public Tower Tower { get; private set; }
		
		public void SetTower(Tower tower)
		{
			Tower = tower;
			HasTower = true;
			tower.transform.position = transform.position;
		}

		public void RemoveTower()
		{
			Tower = null;
			HasTower = false;
		}
		
		public void Unselect()
		{
			_selected = false;
			_border.color = _settings.Clear;
		}
		
		public void OnPointerClick(PointerEventData eventData)
		{
			_selected = true;
			_border.color = HasTower
				? _settings.BuildSelect
				: _settings.FieldSelect;
			
			OnSelectCellHandler.Invoke(this);
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_selected) return;
			_border.color = HasTower
				? _settings.BuildFocus
				: _settings.FieldFocus;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_selected) return;
			_border.color = _settings.Clear;
		}
	}
}