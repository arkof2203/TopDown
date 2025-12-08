using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using Zenject;

namespace Netologia.Systems
{
	public class TowerSystem : GameObjectPoolContainer<Tower>, Director.IManualUpdate
	{
		private UnitSystem _units;				//injected
		private ProjectileSystem _projectiles;  //injected

        public void ManualUpdate()
        {
            foreach (var pool in this)
            {
                foreach (var tower in pool)
                {
                    if (!tower.HasTarget || tower.Target == null || tower.Target.IsDead)
                    {
                        tower.Target = FindNearestTarget(tower);
                        continue;
                    }

                    float distance = (tower.Target.transform.position - tower.transform.position).sqrMagnitude;
                    float range = tower.Range * tower.Range;
                    if (distance > range)
                    {
                        tower.Target = null;
                        continue;
                    }

                    if (tower.DecrementAttackReload(UnityEngine.Time.deltaTime))
                    {
                        tower.Attack();

                        var projectile = _projectiles[tower.Projectile].Get;
                        projectile.PrepareData(tower.transform.position, tower.Target, tower.Damage, tower.AttackElemental);
                        projectile.Ref = tower.Projectile;
                    }
                }
            }
        }


        public void OnDespawnUnit(int unitID)
		{
			foreach (var pair in this)
				foreach (var tower in pair)
					if (tower.TargetID == unitID)
						tower.Target = null;
		}
		
		[Inject]
		private void Construct(UnitSystem units, ProjectileSystem projectiles)
		{
			_units = units;
			_projectiles = projectiles;
		}

        private Unit FindNearestTarget(Tower tower)
        {
            Unit nearest = null;
            float minDistance = float.MaxValue;
            float range = tower.Range * tower.Range;

            foreach (var pool in _units)
            {
                foreach (var unit in pool)
                {
                    if (unit.IsDead) continue;

                    float dist = (unit.transform.position - tower.transform.position).sqrMagnitude;
                    if (dist < range && dist < minDistance)
                    {
                        minDistance = dist;
                        nearest = unit;
                    }
                }
            }

            return nearest;
        }

    }


}