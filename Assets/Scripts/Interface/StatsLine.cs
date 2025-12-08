using TMPro;
using UnityEngine;

namespace Netologia.TowerDefence.Interface
{
	public class StatsLine : MonoBehaviour
	{
		[field: SerializeField]
		public TextMeshProUGUI Name { get; private set; }
		[field: SerializeField]
		public TextMeshProUGUI[] Values { get; private set; }
	}
}