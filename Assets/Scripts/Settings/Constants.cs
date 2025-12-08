using UnityEngine;

namespace Netologia.TowerDefence.Settings
{
	[CreateAssetMenu(fileName = "Constants", menuName = "Tower Defence/Constants", order = 51)]
	public class Constants : ScriptableObject
	{
		[field: SerializeField, Tooltip("Процент возврата за продажу здания")]
		[field: Range(0f, 1f), Header("---Common Constants---")]
		public float SellPercent { get; private set; } = .7f;

		
		
		[field: SerializeField, Tooltip("Множитель дебафа на скорость юнитов от ледяного эффекта")]
		[field: Range(0f, 1f), Header("---Ice Elemental debuff---"), Space(15f)]
		public float IceDebuffMoveSpeedMult { get; private set; } = .9f;
		[field: SerializeField, Tooltip("Время действия одного эффекта заморозки")]
		public float IceDebuffDuration { get; private set; } = 2f;
		[field: SerializeField, Tooltip("Максимальное кол-во стаков заморозки")]
		public int IceDebuffMaxStack { get; private set; } = 3;

		
		
		[field: SerializeField, Tooltip("Процент урона в секунду от горения от максимального ХП")]
		[field: Range(0f, 1f), Header("---Fire Elemental debuff---"), Space(15f)]
		public float FireDebuffDamageMult { get; private set; } = .01f;
		[field: SerializeField, Tooltip("Время действия одного эффекта горения")]
		public float FireDebuffDuration { get; private set; } =2f;
		[field: SerializeField, Tooltip("Максимальное кол-во стаков горения")]
		public int FireDebuffMaxStack { get; private set; } = 3;
	}
}