using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netologia.Behaviours
{
	public interface IPoolElement<TRef>
		where TRef : Component
	{
		TRef Ref { get; set; }
		int ID { get; set; }
	}
	
	/// <summary>
	/// Родитель, для определения набора сущностей, стандартный пример паттерна "Object Pool" 
	/// </summary>
	/// <typeparam name="T">Тип компонентов, которые содержаться в пуле</typeparam>
	public class GameObjectPool<T> : IEnumerable<T>
		where T : Component
	{
		private bool[] _activities;
		private T[] _elements;
		private readonly Lazy<IEnumerator<T>> _iterator;

		private readonly T _prefab;
		private readonly Transform _parent;
		private readonly Action<T> _awakeMethod;
		
		public int Count => _elements.Length;

		public int CountActive
		{
			get
			{
				var count = 0;
				for(int i = 0, iMax = _activities.Length; i < iMax; i++)
					if (_activities[i]) count++;
				return count;
			}
		}
		
		public int CountDisable
		{
			get
			{
				var count = 0;
				for(int i = 0, iMax = _activities.Length; i < iMax; i++)
					if (!_activities[i]) count++;
				return count;
			}
		}

		public T this[int index]
			=> _elements[index];
		
		/// <summary>
		/// Возвращает первый найденный выключенный элемент из пула и включает его
		/// </summary>
		/// <returns>Свободный элемент для переиспользования</returns>
		/// <remarks>Если в пуле нет свободных элементов - создает новый</remarks>
		public T Get
		{
			get
			{
				var index = _activities is null
					? Array.FindIndex(_elements, t => !t.gameObject.activeSelf)
					: Array.FindIndex(_activities, t => t == false);
				
				var element = default(T);
				if (index == -1)
				{
					index = _elements.Length;
					Array.Resize(ref _elements, _elements.Length * 2);
					if (_activities is not null)
						Array.Resize(ref _activities, _activities.Length * 2);
					FillPool(index);
					element = _elements[index];
				}
				else
				{
					element = _elements[index];
					element.gameObject.SetActive(true);
					if (_activities is not null) _activities[index] = true;
				}

				return element;
			}
		}

		/// <summary>
		/// Возвращает элемент обратно в пул (и выключает его игровой объект)
		/// </summary>
		/// <param name="element">Ссылка на освободившийся элемент</param>
		public void ReturnElement(T element)
		{
			ReturnElement(Array.FindIndex(_elements, t => ReferenceEquals(t, element)));
		}

		/// <summary>
		/// Возвращает элемент обратно в пул (и выключает его игровой объект)
		/// </summary>
		/// <param name="index">Индекс вернувшегося элемента</param>
		public void ReturnElement(int index)
		{
			if (index < 0 || index >= _elements.Length)
			{
				Debug.LogError($"Incorrect return element <b> index = {index}</b>");
				return;
			}
			_elements[index].gameObject.SetActive(false);
			if(_activities is not null) _activities[index] = false;
		}
		
		/// <summary>
		/// Освобождает весь пул элементов (выключает их игровые объекты)
		/// </summary>
		public void DisableAllElements()
		{
			for(int i = 0, iMax = _elements.Length; i < iMax; i++)
				_elements[i].gameObject.SetActive(false);
			if(_activities is not null)
				for(int i = 0, iMax = _elements.Length; i < iMax; i++)
					_activities[i] = false;
		}

		public GameObjectPool(T prefab, Transform root, bool autoDisable = false, int capacity = 4, Action<T> awakeMethod = null)
		{
			(_parent, _awakeMethod, _prefab) = (root, awakeMethod, prefab);

			capacity = Mathf.Max(capacity, 4);
			(_activities, _elements) = autoDisable
				? (null, new T[capacity])
				: (new bool[capacity], new T[capacity]);

			FillPool(0);
			_iterator = new Lazy<IEnumerator<T>>(() 
				=> _activities is null 
					? ((IEnumerable<T>)_elements).GetEnumerator() 
					: new ObjectEnumerator(_elements, _activities));
		}

		private void FillPool(int start)
		{
			for (int i = start, iMax = _elements.Length; i < iMax; i++)
			{
				_elements[i] = GameObject.Instantiate(_prefab, _parent);
				if (_activities is not null) _activities[i] = false;
				var obj = _elements[i].gameObject;
				obj.SetActive(false);
			}
			if (_awakeMethod is not null)
				for (int i = start, iMax = _elements.Length; i < iMax; i++)
					_awakeMethod(_elements[i]);
			if(_prefab is IPoolElement<T>)
				for (int i = start, iMax = _elements.Length; i < iMax; i++)
				{
					var pooler = (IPoolElement<T>)_elements[i];
					(pooler.Ref, pooler.ID) = (_prefab, i);
				}
		}

		public IEnumerator<T> GetEnumerator()
		{
			if(_iterator.IsValueCreated)
				_iterator.Value.Reset();
			return _iterator.Value;
		}

		IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();
		
		private class ObjectEnumerator : IEnumerator<T>
		{
			private readonly bool[] _activities;
			private readonly T[] _array;
			private int _count;
			private int _index;
			
			public ObjectEnumerator(T[] array, bool[] activities)
				=> (_array, _activities, _index, _count) = (array, activities, -1, array.Length);

			public T Current => _array[_index];
			
			public bool MoveNext()
			{
				_index++;
				while (_index < _count)
				{
					if (_activities[_index])
						return true;
					_index++;
				}

				return false;
			}

			public void Reset() 
				=> (_index, _count) = (-1, _array.Length);
			object IEnumerator.Current => Current;
			public void Dispose() { }
		}
	}
}