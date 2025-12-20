using Netologia.Systems;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Unit))]
public class ArcherAI : MonoBehaviour
{
    [Header("Move")]
    [SerializeField, Min(0.1f)] private float _speed = 2.5f;
    [SerializeField, Min(0.01f)] private float _stopDistance = 0.1f;
    [SerializeField] private Rigidbody2D _rb2d;

    [Header("Area (Wander)")]
    [SerializeField] private float _newWanderPointDelay = 1.0f;

    [Header("Vision")]
    [SerializeField, Min(0.1f)] private float _visionRadius = 3.5f;
    [SerializeField] private LayerMask _enemyMask;

    [Header("Attack (ProjectileSystem)")]
    [SerializeField] private Projectile _arrowRef;
    [SerializeField] private Transform _shootPoint; 
    [SerializeField, Min(0.05f)] private float _attackCooldown = 0.8f;
    [SerializeField, Min(0.1f)] private float _attackDamage = 5f;
    [SerializeField] private ElementalType _element = ElementalType.Physic;

    private Unit _unit;
    private ProjectileSystem _projectiles;

    private ArcherArea[] _areas;
    private ArcherArea _area;
    private Vector2 _wanderTarget;
    private float _wanderTimer;

    private Unit _targetEnemy;
    private float _attackTimer;

    [Inject]
    private void Construct(ProjectileSystem projectiles)
    {
        _projectiles = projectiles;
    }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        if (_projectiles == null && Director.Instance != null)
            _projectiles = Director.Instance.Projectiles;
    }

    private void OnEnable()
    {
        _attackTimer = 0f;
        _wanderTimer = 0f;
        _targetEnemy = null;

        ChooseAreaNearestFree();
        PickNewWanderPoint();
    }

    private void Update()
    {
        if (Time.frameCount % 60 == 0)
        {

        }

        _attackTimer -= Time.deltaTime;

        _targetEnemy = FindNearestEnemy();
        if (_targetEnemy != null)
        {
            TryShoot(_targetEnemy);
            return;
        }

        Wander();
    }

    private void ChooseAreaNearestFree()
    {
        if (_areas == null || _areas.Length == 0)
        {
            _areas = FindObjectsOfType<ArcherArea>(false);
        }
        if (_areas == null || _areas.Length == 0)
        {
            _area = null;
            return;
        }
        ArcherArea best = null;
        float bestDist = float.MaxValue;
        for (int i = 0; i < _areas.Length; i++)
        {
            var a = _areas[i];
            if (a == null)
            {
                continue;
            }
            if (!a.HasFreeSlot) continue;
            float d = (a.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = a;
            }
        }
        if (best == null)
        {
            _area = null;
            return;
        }
        if (_area != null && _area != best)
            _area.Release(this);
        bool claimed = best.TryClaim(this);

        if (!claimed)
        {
            _area = null;
            return;
        }
        _area = best;
    }


    private void PickNewWanderPoint()
    {
        if (_area == null)
        {
            _wanderTarget = transform.position;
            return;
        }
        for (int i = 0; i < 6; i++)
        {
            var p = _area.GetRandomPointInside();
            if (((Vector2)transform.position - p).sqrMagnitude > 0.15f * 0.15f)
            {
                _wanderTarget = p;
                _wanderTimer = _newWanderPointDelay;
                return;
            }
        }
        _wanderTarget = _area.GetRandomPointInside();
        _wanderTimer = _newWanderPointDelay;
    }

    private void Wander()
    {
        _wanderTimer -= Time.deltaTime;

        Vector2 pos = transform.position;
        Vector2 to = _wanderTarget - pos;

        if (to.sqrMagnitude <= _stopDistance * _stopDistance || _wanderTimer <= 0f)
        {
            PickNewWanderPoint();
            return;
        }

        Vector2 dir = to.normalized;
        Move(dir);

        _unit.FlipByDirection(dir.x);
    }

    private void Move(Vector2 dir)
    {
        Vector2 pos = transform.position;
        Vector2 next = pos + dir * (_speed * Time.deltaTime);

        if (_rb2d != null) _rb2d.MovePosition(next);
        else transform.position = next;
    }

    // -------------------- VISION --------------------

    private Unit FindNearestEnemy()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _visionRadius, _enemyMask);

        if (Time.frameCount % 60 == 0)
            Debug.Log($"[ArcherAI] Vision hits={hits.Length}, mask={_enemyMask.value}, radius={_visionRadius}", this);

        Unit best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;
            var u = col.GetComponentInParent<Unit>();

            if (Time.frameCount % 60 == 0)
            if (u == null) continue;
            if (u == _unit) continue;
            if (u.IsDead) continue;

            float d = (u.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = u;
            }
        }

        return best;
    }

    private void TryShoot(Unit enemy)
    {
        if (enemy == null || enemy.IsDead) return;
        if (_projectiles == null)
        {
            return;
        }
        if (_arrowRef == null)
        {
            return;
        }
        if (_attackTimer > 0f) return;
        Vector3 spawnPos = _shootPoint != null ? _shootPoint.position : transform.position;
        var arrow = _projectiles[_arrowRef].Get;
        arrow.PrepareData(spawnPos, enemy, _attackDamage, _element);
        arrow.Ref = _arrowRef;
        _attackTimer = _attackCooldown;
    }

    private void OnDisable()
    {
        if (_area != null)
        {
            _area.Release(this);
            _area = null;
        }
    }
}

