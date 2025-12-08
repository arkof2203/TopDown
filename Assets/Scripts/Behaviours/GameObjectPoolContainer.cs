using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netologia.Behaviours
{
	/// <summary>
	/// Используется для менеджмента пулов объектов в игре
	/// </summary>
	/// <typeparam name="TRef">Тип объектов в пуле</typeparam>
	/// <typeparam name="TPool">Тип пулов</typeparam>
	public abstract class GameObjectPoolContainer<TRef> : MonoBehaviour, 
		IPoolContainer<TRef>, IEnumerable<GameObjectPool<TRef>>
		where TRef : Component
	{
		protected readonly Dictionary<TRef, GameObjectPool<TRef>> _pools = new (8);

		[SerializeField]
		private int _poolCapacity = 4;
		[SerializeField]
		private bool _disableTracking = true;
		
#if UNITY_EDITOR
		[SerializeField, Space(15f), ReadOnly]
		private int _maxCapacity;
#endif
		
		public Action<TRef> AwakeMethod { get; set; }
		
		/// <summary>
		/// Возвращает пул объектов по префабу-источнику
		/// </summary>
		/// <param name="prefab">Ссылка на префаб-источник</param>
		public GameObjectPool<TRef> this[TRef prefab]
			=> _pools.TryGetValue(prefab, out var pool) ? pool : CreatePool(prefab, _poolCapacity);

		/// <summary>
		/// Создан-ли пул объектов
		/// </summary>
		/// <param name="prefab">Источник пула</param>
		/// <returns>Создан-ли пул</returns>
		public bool IsCreated(TRef prefab)
			=> _pools.ContainsKey(prefab);

		public int CountActive
		{
			get
			{
				var count = 0;
				foreach (var pool in _pools)
					count += pool.Value.CountActive;
				return count;
			}
		}
		
		public int CountDisable
		{
			get
			{
				var count = 0;
				foreach (var pool in _pools)
					count += pool.Value.CountDisable;
				return count;
			}
		}
		
		/// <summary>
		/// Создает и возвращает пул объектов
		/// </summary>
		/// <param name="prefab">Ссылка на префаб-источник для пула</param>
		/// <param name="capacity">Предварительная ёмкость пула</param>
		/// <returns>Проинициализированный пул объектов</returns>
		/// <exception cref="ApplicationException">Попытка создать дубликат пула</exception>
		public GameObjectPool<TRef> CreatePool(TRef prefab, int capacity)
		{
#if UNITY_EDITOR
			if (_pools.TryGetValue(prefab, out var pool))
				throw LogUtility.Throw(nameof(GameObjectPoolContainer<TRef>), nameof(CreatePool), 
					$"Attempt to create a duplicate pool: <b>{prefab.name}</b>");
#else
						
#endif

			if (IncorrectPrefabParameters(prefab, out var message))
				throw LogUtility.Throw(nameof(GameObjectPoolContainer<TRef>), nameof(CreatePool),
					message);
			
			var obj = new GameObject(string.Concat("Pool_", prefab.name));
			var root = obj.transform;
			root.parent = transform;
			root.localScale = Vector3.one;
			root.SetPositionAndRotation(new Vector3(), Quaternion.identity);
			pool = new GameObjectPool<TRef>(prefab, root, !_disableTracking, capacity, AwakeMethod);
			_pools.Add(prefab, pool);

			return pool;
		}

		/// <summary>
		/// Проверяет, неправильная-ли настройка префаба для создания пула 
		/// </summary>
		/// <param name="prefab">Ссылка на префаб</param>
		/// <param name="message">Сообщение, выводимое в ошибку</param>
		/// <returns>True - префаб настроен некорректно | False - префаб настроен правильно</returns>
		protected virtual bool IncorrectPrefabParameters(TRef prefab, out string message)
		{
			message = default;
			return false;
		}

		public IEnumerator<GameObjectPool<TRef>> GetEnumerator()
			=> _pools.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

#if UNITY_EDITOR
		private void Update()
		{
			var count = default(int);
			foreach (var pool in _pools)
				count = Mathf.Max(pool.Value.Count, count);
			_maxCapacity = count;
		}
#endif
	}
	
	public interface IPoolContainer<TRef>
		where TRef : Component
	{
		GameObjectPool<TRef> this[TRef prefab] { get; }
		bool IsCreated(TRef prefab);
		GameObjectPool<TRef> CreatePool(TRef prefab, int capacity);		
	}
}