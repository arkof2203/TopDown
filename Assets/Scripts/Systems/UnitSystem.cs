using System;
using Behaviours;
using JetBrains.Annotations;
using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using Netologia.TowerDefence.Settings;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
	public class UnitSystem : GameObjectPoolContainer<Unit>, Director.IManualUpdate
	{
		private Director _director;					//injected
		private EffectSystem _effects;				//injected
		private Constants _constants;				//injected
		private Vector3[] _path;					//injected

		[SerializeField, Min(0.01f)]
		private float _arrivalDistance = 0.1f;

		public event Action<int> OnDespawnUnitHandler;

		[CanBeNull]
		public Unit FindTarget(in Vector3 position, float range)
		{
			range *= range;
			var target = default(Unit);
			foreach (var pair in this)
			{
				foreach (var unit in pair)
				{
					var distance = Vector3.SqrMagnitude(unit.transform.position - position);
					if (distance < range)
						(range, target) = (distance, unit);
				}
			}

			return target;
		}

        public void ManualUpdate()
        {
            foreach (var pair in this)
            {
                foreach (var unit in pair)
                {
                    if (unit.CurrentHealth <= 0)
                    {
                        DespawnUnit(unit, unit.transform.position);
                        continue;
                    }

                    if (unit.PathIndex >= _path.Length)
                    {
                        DespawnUnit(unit, unit.transform.position);
                        continue;
                    }

                    Vector3 target = _path[unit.PathIndex];

                    unit.transform.position = Vector3.MoveTowards(
                        unit.transform.position,
                        target,
                        unit.MoveSpeed * Time.deltaTime);

                    if (Vector3.SqrMagnitude(unit.transform.position - target) <= _arrivalDistance)
                    {
                        unit.PathIndex++;

                        if (unit.PathIndex >= _path.Length)
                        {
                            // Враг дошёл до базы — наносим урон игроку
                            _director.AddPlayerDamage(10); // можно поставить урон из настроек, если есть
                           // DespawnUnit(unit, target);
                        }

                    }
                }
            }
        }


        private void DespawnUnit(Unit unit, in Vector3 position)
		{
			//Create HitEffect
			if (unit.HasEffect)
			{
				var effect = _effects[unit.DieEffect].Get;
				effect.transform.position = position;
				effect.Play();
			}
			//Play sound
			if (unit.HasSound)
			{
				AudioManager.PlayHit(unit.DieSound);
			}

			_director.AddMoney(unit.Stats.Cost);
			this[unit.Ref].ReturnElement(unit.ID);
		}
		
		[Inject]
		private void Construct(EffectSystem effects, Director director, Constants constants, WaveController path)
		{
			(_effects, _director, _constants, _path) = (effects, director, constants, path.GetPath());
			_arrivalDistance *= _arrivalDistance;
			AwakeMethod = t => t.Constants = _constants;
		}
	}
}