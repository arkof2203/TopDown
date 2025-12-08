using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Netologia.TowerDefence.Interface
{
	public class UpgradeTowerPanel : MonoBehaviour
	{
		private const string c_upgradeTableHeader = "Upgrade Table (Current Level: <u><b>{0}</b></u>)";
		private const int c_countLevel = 5;

		private readonly Dictionary<ElementalType, List<string>> _cashe = new (4);
		private List<StatsLine> _stats;
		
		[SerializeField]
		private TextMeshProUGUI _header;
		[SerializeField]
		private StatsLine _statsLine;

		public void SetTarget(Tower tower)
		{
			_header.text = string.Format(c_upgradeTableHeader, tower.Level);

			if (_stats is null)
				CreateStatsLines();

			if (!_cashe.TryGetValue(tower.AttackElemental, out var list))
			{
				list = new(c_countLevel * 4);
				CreateCache(ref list, tower.Progress, nameof(Tower.TowerProgress.Cost));
				CreateCache(ref list, tower.Progress, nameof(Tower.TowerProgress.Damage));
				CreateCache(ref list, tower.Progress, nameof(Tower.TowerProgress.AttackDelay));
				CreateCache(ref list, tower.Progress, nameof(Tower.TowerProgress.Range));
				_cashe.Add(tower.AttackElemental, list);
			}

			for (int i = 0, start = 0, length = _stats[0].Values.Length; i < _stats.Count; i++)
			{
				FillData(start, length * (i + 1), _stats[i].Values, list);
				start += length;
			}
		}

		private void CreateStatsLines()
		{
			StatsLine create(StatsLine source, Transform parent, string name)
			{
				var line = Instantiate(source, parent);	
				line.Name.text = name;
				return line;
			}
				
			_stats = new(4);
			_stats.Add(_statsLine);
			_statsLine.Name.text = "Cost";
			var parent = _statsLine.transform.parent;
				
			_stats.Add(create(_statsLine, parent, "Damage"));
			_stats.Add(create(_statsLine, parent, "Attack Delay"));
			_stats.Add(create(_statsLine, parent, "Range"));
		}

		private void CreateCache(ref List<string> list, Tower.TowerProgress[] progress, string fieldName)
		{
			var field = typeof(Tower.TowerProgress).GetField(fieldName);
			for (int i = 0, iMax = progress.Length; i < iMax; i++)
				list.Add(field.GetValue(progress[i]).ToString());
		}

		private void FillData(int i, int length, TextMeshProUGUI[] texts, List<string> values)
		{
			for (int j = 0; i < length; i++, j++)
				texts[j].text = values[i];
		}
	}
}