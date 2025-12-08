using UnityEngine;

namespace Netologia.TowerDefence.Settings
{
	[CreateAssetMenu(fileName = "CellPresetSettings", menuName = "Tower Defence/Cell Preset Settings", order = 0)]
	public class CellPresetSettings : ScriptableObject
	{
		[field: SerializeField, Space(10f)]
		public Color FieldFocus { get; private set; }
		[field: SerializeField]
		public Color FieldSelect { get; private set; }
		
		
		[field: SerializeField, Space(10f)]
		public Color BuildFocus { get; private set; }
		[field: SerializeField]
		public Color BuildSelect { get; private set; }
		
		
		[field:SerializeField, Space]
		public Color Clear { get; private set; }
	}
}