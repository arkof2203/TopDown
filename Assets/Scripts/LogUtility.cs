using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Netologia
{
	public static class LogUtility
	{
		public static void Message(object holder, string message)
			=> PrintLog($"<b>[{holder}]</b>: {message}");
		
		public static void Message(object holder, string message, Object target)
			=> PrintLog($"<b>[{holder}]</b>: {message}", 0, target);
		
		public static void Message(object holder, object dotHolder, string message)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}");
		
		public static void Message(object holder, object dotHolder, string message, Object target)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}", 0, target);

		public static void Warning(object holder, string message)
			=> PrintLog($"<b>[{holder}]</b>: {message}", 1);
		
		public static void Warning(object holder, string message, Object target)
			=> PrintLog($"<b>[{holder}]</b>: {message}", 1, target);
		
		public static void Warning(object holder, object dotHolder, string message)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}", 1);
		
		public static void Warning(object holder, object dotHolder, string message, Object target)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}", 1, target);
		
		public static void Error(object holder, string message)
			=> PrintLog($"<b>[{holder}]</b>: {message}", 2);
		
		public static void Error(object holder, string message, Object target)
			=> PrintLog($"<b>[{holder}]</b>: {message}", 2, target);
		
		public static void Error(object holder, object dotHolder, string message)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}", 2);

		public static void Error(object holder, object dotHolder, string message, Object target)
			=> PrintLog($"<b>[{holder}.{dotHolder}]</b>: {message}", 2, target);
		
		public static ApplicationException Throw(object holder, string message)
			=> new ($"<b>[{holder}]</b>: {message}");
		
		public static ApplicationException Throw(object holder, object dotHolder, string message)
			=> new ( $"<b>[{holder}.{dotHolder}]</b>: {message}");
		
		public static ApplicationException Throw(object holder, string message, Exception innerException)
			=> new ($"<b>[{holder}]</b>: {message}", innerException);
		
		public static ApplicationException Throw(object holder, object dotHolder, string message, Exception innerException)
			=> new ( $"<b>[{holder}.{dotHolder}]</b>: {message}", innerException);
		
		public static T Throw<T>(object holder, string message)
			where T : Exception
			=> Activator.CreateInstance(typeof(T), $"<b>[{holder}]</b>: {message}") as T;

		public static T Throw<T>(object holder, object dotHolder, string message)
			where T : Exception
			=> Activator.CreateInstance(typeof(T), $"<b>[{holder}.{dotHolder}]</b>: {message}") as T;
		
		public static T Throw<T>(object holder, string message, Exception innerException)
			where T : Exception
			=> Activator.CreateInstance(typeof(T), $"<b>[{holder}]</b>: {message}", innerException) as T;
		
		public static T Throw<T>(object holder, object dotHolder, string message, Exception innerException)
			where T : Exception
			=> Activator.CreateInstance(typeof(T), $"<b>[{holder}.{dotHolder}]</b>: {message}", innerException) as T;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void PrintLog(string message, int type = 0, Object target = null)
		{
			switch(type)
			{
				case 0:
					Debug.Log(message);
					break;
				case 1:
					Debug.LogWarning(message);
					break;
				case 2:
					if (target == null)
						Debug.LogError(message);
					else Debug.LogError(message, target);
					break;
			}
		}
	}
}