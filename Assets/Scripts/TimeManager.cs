
namespace Netologia
{
	public static class TimeManager
	{
		public static float Time { get; private set; }
		public static float DeltaTime => IsGame ? UnityEngine.Time.deltaTime : 0f;
		public static float FixedDeltaTime => IsGame ? UnityEngine.Time.fixedDeltaTime : 0f;
		
		public static bool IsGame { get; set; }

		public static void IncrementDeltaTime()
			=> Time += UnityEngine.Time.deltaTime;

		public static void Reset()
			=> Time = 0f;
	}
}