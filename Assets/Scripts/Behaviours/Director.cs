using System;
using Behaviours;
using Netologia.Systems;
using Netologia.TowerDefence.Settings;
using UnityEngine;
using Zenject;

namespace Netologia.TowerDefence.Behaviors
{
	public class Director : MonoInstaller
	{
		private bool _startFindWin;
		
		[SerializeField]
		private ProjectileSystem _projectiles;
		[SerializeField]
		private UnitSystem _units;
		[SerializeField]
		private TowerSystem _towers;
		[SerializeField]
		private EffectSystem _effects;
		
		[SerializeField]
		private WaveController _wave;

		[SerializeField]
		private InterfaceController _interface;
		[SerializeField]
		private AudioManager _audio;
		[SerializeField]
		private CellController _cell;

		[SerializeField, Min(1f), Space(15f)]
		private int _playerHP = 100;
		[SerializeField, Min(0f)]
		private float _gold = 100f;
		
		[Header("---Settings---"), Space(15f)][SerializeField]
		private WavePresetSettings _waves;
		[SerializeField]
		private Constants _constants;
		
		public static Director Instance { get; private set; }

		private void Update()
		{
			if (!TimeManager.IsGame) return;
			TimeManager.IncrementDeltaTime();
			//Set all damage
			_projectiles.ManualUpdate();
			//Remove all targets
			_units.ManualUpdate();
			//Find new targets
			_towers.ManualUpdate();
			_interface.ManualUpdate();
			//Last wave
			if (_startFindWin && _units.CountActive == 0)
				_interface.GameWin();
		}

		public void AddPlayerDamage(int value)
		{
			_playerHP -= value;
			if (_playerHP <= 0)
				_interface.GameLose();
			else _interface.SetHP(_playerHP);
		}
		
		public void AddMoney(float value)
		{
			_gold += value;
			_interface.SetGold(_gold);
		}

		private bool TryChangeGold(int value)
		{
			var result = _gold + value;
			if (result > 0)
			{
				_gold = result;
				_interface.SetGold(result);
				return true;
			}
			
			return false;
		}
		
		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(this);
				LogUtility.Error(nameof(Director), nameof(Awake), "Second Director!", gameObject);
				return;
			}
			Instance = this;
			TimeManager.Reset();
			TimeManager.IsGame = true;

			_units.OnDespawnUnitHandler += _projectiles.OnDespawnUnit;
			_units.OnDespawnUnitHandler += _towers.OnDespawnUnit;
			_wave.OnLastWaveEnded += OnStartFindFinish;
			_cell.OnTryGoldChanged += TryChangeGold;
			
			_interface.SetHP(_playerHP);
			_interface.SetGold(_gold);
		}

		private void OnStartFindFinish()
			=> _startFindWin = true;
		
		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
			Container.BindInstance(_projectiles).AsSingle();
			Container.BindInstance(_units).AsSingle();
			Container.BindInstance(_towers).AsSingle();
			Container.BindInstance(_effects).AsSingle();
			Container.BindInstance(_wave).AsSingle();
			Container.BindInstance(_audio).AsSingle();
			Container.BindInstance(_cell).AsSingle();
			Container.BindInstance(_waves).AsSingle();
			Container.BindInstance(_constants).AsSingle();
		}

		private void OnDestroy()
		{
			_units.OnDespawnUnitHandler -= _projectiles.OnDespawnUnit;
			_units.OnDespawnUnitHandler -= _towers.OnDespawnUnit;
			_wave.OnLastWaveEnded -= OnStartFindFinish;
			_cell.OnTryGoldChanged -= TryChangeGold;
		}

		public interface IManualUpdate
		{
			void ManualUpdate();
		}
	}
}