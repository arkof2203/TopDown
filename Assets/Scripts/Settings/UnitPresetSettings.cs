using System;
using UnityEngine;

namespace Netologia.TowerDefence.Settings
{
	[CreateAssetMenu(fileName = "UnitPresetSettings", menuName = "Tower Defence/Unit Preset Settings", order = 0)]
	public class UnitPresetSettings : ScriptableObject
	{
		[field: SerializeField]
		public Stats Preset { get; private set; }
			
		[Serializable]
		public struct Stats
		{
			public float Health;
			public float MoveSpeed;
			public int Cost;
		}
	}
}