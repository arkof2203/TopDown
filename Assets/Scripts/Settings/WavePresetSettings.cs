using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netologia.TowerDefence.Settings
{
	[CreateAssetMenu(fileName = "WavePresetSettings", menuName = "Tower Defence/Wave Preset Settings", order = 1)]
	public class WavePresetSettings : ScriptableObject, IReadOnlyList<WavePresetSettings.Wave>
	{
		[Serializable]
		public struct Wave
		{
			public float StartDelay;
			public Pack[] Packs;
		}

		[Serializable]
		public struct Pack
		{
			public Unit Prefab;
			public UnitPresetSettings Preset;
			public int Count;
			public float SpawnDelay;
		}
		
		[SerializeField]
		private List<Wave> _sea;


		public int Count => _sea.Count;

		public Wave this[int index] => _sea[Mathf.Clamp(index, 0, _sea.Count - 1)];
		public Pack this[int wave, int pack] => this[wave].Packs[pack];
		
		public IEnumerator<Wave> GetEnumerator()
			=> _sea.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}
}