using System.Collections.Generic;
using Netologia.Behaviours;
using Netologia.TowerDefence.Settings;
using UnityEngine;

namespace Netologia.TowerDefence
{
	public class Unit : MonoBehaviour, IPoolElement<Unit>
	{
		private List<float> _fireEffects;
		private List<float> _iceEffects;

		public UnitPresetSettings.Stats Stats { get; private set; }
		
		[field: SerializeField]
		public ParticleSystem DieEffect { get; private set; }
		[field: SerializeField]
		public AudioClip DieSound { get; private set; }
		public bool HasEffect { get; private set; }
		public bool HasSound { get; private set; }
		public Constants Constants { get; set; }
		
		public int PathIndex { get; set; }
		
		public Unit Ref { get; set; }
		public int ID { get; set; }
		
		public float CurrentHealth { get; set; }
        public bool IsDead => CurrentHealth <= 0;
        public UnitVisual Visual { get; private set; }
		
		public float MoveSpeed
			=> _iceEffects.Count > 0
				? Mathf.Pow(Constants.IceDebuffMoveSpeedMult, _iceEffects.Count) * Stats.MoveSpeed
				: Stats.MoveSpeed;

		public int CountEffect(ElementalType type)
			=> type switch
			{
				ElementalType.Fire => _fireEffects.Count,
				ElementalType.Ice => _iceEffects.Count,
				_ => 0
			};
		
		public void TryAddEffect(float time, ElementalType type)
		{
			var list = default(List<float>);
			var max = default(int);
			switch(type)
			{
				case ElementalType.Fire:
					(list, max) = (_fireEffects, Constants.FireDebuffMaxStack);
					break; 
				case ElementalType.Ice:
					(list, max) = (_iceEffects, Constants.IceDebuffMaxStack);
					break;
				default:
					return;
			}

			//Renewal oldest effect
			if (list.Count >= max)
				list[^1] = time;
			else
				list.Add(time);
			
			list.Sort(Compare);
		}

		public void TryRemoveEffect(float time, ElementalType type)
		{
			var list = default(List<float>);
			var delay = default(float);
			(list, delay) = type switch
			{
				ElementalType.Fire => (_fireEffects, Constants.FireDebuffDuration),
				ElementalType.Ice => (_iceEffects, Constants.IceDebuffDuration),
				_ => (null, 0)
			};

			if (list.Count == 0) return;
			if(time - list[^1] >= delay) list.RemoveAt(list.Count - 1);
		}

		public void Respawn(UnitPresetSettings.Stats stats, Vector3 position)
		{
			(Stats, CurrentHealth, PathIndex) = (stats, stats.Health, 0);
			transform.position = position;
			if (_fireEffects is null)	//first Spawn
			{
				HasEffect = DieEffect != null;
				HasSound = DieSound != null;
				Visual = GetComponent<UnitVisual>();
				
				_fireEffects = new (Constants.FireDebuffMaxStack);
				_iceEffects = new (Constants.IceDebuffMaxStack);
			}
			
			_fireEffects.Clear(); _iceEffects.Clear();
		}

		private static int Compare(float a, float b)
			=> a > b ? -1 : 1;
	}
}