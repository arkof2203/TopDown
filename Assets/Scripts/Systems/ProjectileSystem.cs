using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
	public class ProjectileSystem : GameObjectPoolContainer<Projectile>, Director.IManualUpdate
	{
		private EffectSystem _effects;		//injected
		
		[SerializeField, Min(0.01f)]
		private float _hitDistance = 0.3f;

        public void ManualUpdate()
        {
            foreach (var pool in this)
            {
                foreach (var projectile in pool)
                {
                    if (projectile.TargetPosition == null)
                        continue;

                    Vector3 currentPosition = projectile.transform.position;
                    Vector3 targetPosition = projectile.TargetPosition;
                    Vector3 direction = targetPosition - currentPosition;

                    projectile.transform.position = Vector3.MoveTowards(
                        currentPosition,
                        targetPosition,
                        projectile.MoveSpeed * Time.deltaTime
                    );

                    if (Vector3.SqrMagnitude(targetPosition - projectile.transform.position) <= _hitDistance)
                    {
                        projectile.DealDamage();

                        if (projectile.HasEffect)
                        {
                            var effect = _effects[projectile.HitEffect].Get;
                            effect.transform.position = projectile.transform.position;
                            effect.Play();
                        }

                        if (projectile.HasSound)
                        {
                            AudioManager.PlayHit(projectile.HitSound);
                        }

                        this[projectile.Ref].ReturnElement(projectile.ID);
                    }
                }
            }
        }


        public void OnDespawnUnit(int unitID)
		{
			foreach (var pool in this)
				foreach (var projectile in pool)
					if(projectile.TargetID == unitID)
						projectile.ResetTarget();
		}

		[Inject]
		private void Construct(EffectSystem effects)
		{
			(_effects) = (effects);
			//SqrtMagnitude optimization
			_hitDistance *= _hitDistance;
		}
	}
}