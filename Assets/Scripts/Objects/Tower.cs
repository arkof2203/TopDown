using System;
using JetBrains.Annotations;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

namespace Netologia.TowerDefence
{
	public class Tower : MonoBehaviour
	{
		[Serializable]
		public struct TowerProgress
		{
			public float Damage;
			public float AttackDelay;
			public float Range;
			public int Cost;
		}
		
		private int _level = -1;
		private Unit _target;
		private bool _hasTarget;
		private float _delay;
		private bool _hasEffect;
		private bool _hasSound;

		[Inject]
		protected Director Director { get; private set; }
		
		[field: SerializeField, Tooltip("Прокачка объекта")]
		public TowerProgress[] Progress { get; private set; }
		
		[field: SerializeField]
		public ElementalType AttackElemental { get; private set; }
		[field: SerializeField]
		public Projectile Projectile { get; private set; }
		[field: SerializeField]
		public ParticleSystem AttackEffect { get; private set; }
		[field: SerializeField]
		public AudioClip AttackSound { get; private set; }

		[SerializeField]
		private SpriteRenderer _rangeBack;

		public bool HasTarget => _hasTarget;
		
		[CanBeNull]
		public Unit Target
		{
			get => _target;
			set
			{
				_target = value;
				_hasTarget = value != null;
			}
		}
		
		public int TargetID => _hasTarget ? _target.ID : -1;

		//Актуальные параметры с учетом прогрессии
		public float Damage => Progress[_level].Damage;
		public float AttackDelay => Progress[_level].AttackDelay;
		public float Range => Progress[_level].Range;

		/// <summary>
		/// Текущий уровень прокачки
		/// </summary>
		public int Level => _level;

		public bool CanLevelUp => Level < Progress.Length;
		
		/// <summary>
		/// Повышение уровня объекта
		/// </summary>
		public void LevelUp()
		{
			var clampLevel = Mathf.Clamp(_level + 1, 0, Progress.Length - 1);
			_level = clampLevel;
			UpdateRangeBackground();
		}
		
		public bool DecrementAttackReload(float delta)
		{
			_delay -= delta;
			return _delay <= 0f;
		}
		
		public void Attack()
		{
			_delay = AttackDelay;
			if (_hasEffect)
				AttackEffect.Play();
			if(_hasSound)
				AudioManager.PlayAttack(AttackSound);
		}
		
		//Reset params
		private void OnDisable()
		{
			_level = 0;
			UpdateRangeBackground();
		}

		protected void Awake()
		{
			if (AttackEffect != null)
			{
				if (AttackEffect.main.stopAction is ParticleSystemStopAction.None)
					_hasEffect = true;
				else
					LogUtility.Error(nameof(Tower), "Incorrect StopAction in AttackEffect", gameObject);
			}
			_hasSound = AttackSound != null;
		}

		private void UpdateRangeBackground()
			=> _rangeBack.transform.localScale = Vector3.one * 0.67f * Range;
		
		private void OnDrawGizmos()
		{
			if(_level < 0 || _level >= Progress.Length) return;

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, Range);
		}
	}
}