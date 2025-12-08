namespace Netologia.TowerDefence
{
	[System.Flags]
	public enum ElementalType : byte
	{
		Physic = 0,
		Fire = 1 << 1,
		Ice = 1 << 2,
		Earth = 1 << 3
	}
}