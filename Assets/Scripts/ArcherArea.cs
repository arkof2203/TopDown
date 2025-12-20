using System.Collections.Generic;
using UnityEngine;

public class ArcherArea : MonoBehaviour
{
    [SerializeField, Min(1)] private int _capacity = 5;
    [SerializeField] private Collider2D _areaCollider;

    private readonly HashSet<ArcherAI> _occupants = new();

    public int Capacity => _capacity;
    public int Count => _occupants.Count;
    public bool HasFreeSlot => Count < Capacity;

    private void Reset()
    {
        _areaCollider = GetComponent<Collider2D>();
        if (_areaCollider != null) _areaCollider.isTrigger = true;
    }

    private void Awake()
    {
        if (_areaCollider == null)
            _areaCollider = GetComponent<Collider2D>();
        if (_areaCollider == null)
            Debug.LogError($"[ArcherArea] No Collider2D on {name}", this);
        else if (!_areaCollider.isTrigger)
            Debug.LogWarning($"[ArcherArea] Collider2D should be IsTrigger=true on {name}", this);
    }

    public bool TryClaim(ArcherAI archer)
    {
        if (archer == null) return false;
        if (_occupants.Contains(archer)) return true;
        if (!HasFreeSlot) return false;

        _occupants.Add(archer);
        return true;
    }

    public void Release(ArcherAI archer)
    {
        if (archer == null) return;
        _occupants.Remove(archer);
    }

    public Vector2 GetRandomPointInside(int attempts = 12)
    {
        if (_areaCollider == null) return transform.position;

        Bounds b = _areaCollider.bounds;
        if (_areaCollider is CircleCollider2D circle)
        {
            Vector2 center = circle.bounds.center;
            float radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            return center + Random.insideUnitCircle * radius;
        }
        for (int i = 0; i < attempts; i++)
        {
            var p = new Vector2(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y)
            );

            if (_areaCollider.OverlapPoint(p))
                return p;
        }
        return _areaCollider.bounds.center;
    }
}
