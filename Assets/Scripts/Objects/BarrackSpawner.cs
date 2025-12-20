using System.Collections.Generic;
using UnityEngine;

namespace Netologia.TowerDefence
{
    [RequireComponent(typeof(Tower))]
    public class BarrackSpawner : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private Unit _archerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField, Min(1)] private int _maxAliveArchers = 3;

        private Tower _tower;
        private float _timer;

        private readonly List<Unit> _alive = new();

        private void Awake()
        {
            _tower = GetComponent<Tower>();
        }

        private void OnEnable()
        {
            _timer = 5f;
        }

        private void Update()
        {
            CleanupDead();

            if (_archerPrefab == null) return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            float interval = Mathf.Max(0.05f, _tower.AttackDelay);

            if (_alive.Count < _maxAliveArchers)
            {
                SpawnArcher();
                _tower.Attack();
            }

            _timer = interval;
        }

        private void SpawnArcher()
        {
            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion rot = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

            Unit archer = Instantiate(_archerPrefab, pos, rot);
            _alive.Add(archer);
        }

        private void CleanupDead()
        {
            for (int i = _alive.Count - 1; i >= 0; i--)
            {
                var u = _alive[i];
                if (u == null || u.IsDead) _alive.RemoveAt(i);
            }
        }
    }
}
