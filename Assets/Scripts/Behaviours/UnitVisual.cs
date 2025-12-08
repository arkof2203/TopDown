using UnityEngine;

namespace Netologia.TowerDefence
{
	public class UnitVisual : MonoBehaviour
	{
		private float _delay;
		private int _current;
		
		[SerializeField]
		private SpriteRenderer _renderer;
		[SerializeField, Range(0.01f, 10f)]
		private float _spriteSwitchDelay = .2f;
		[SerializeField]
		private Sprite[] _sprites;

		public void ManualUpdate(float delta)
		{
			_delay -= delta;
			if (_delay <= 0f)
			{
				_delay = _spriteSwitchDelay;
				_renderer.sprite = _sprites[_current = (_current + 1) % _sprites.Length];
			}
		}
	}
}