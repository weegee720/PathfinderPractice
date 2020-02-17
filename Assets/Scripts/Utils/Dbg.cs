using System.Diagnostics;

/// <summary>
/// Контроль условной компиляции отладочных вызовов.
/// Отдельные вызовые включаются и выключаются в настройках плеера - Scripting Define Symbols
/// </summary>
public class Dbg
{
	[Conditional("MSG_LOGS")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("ERR_LOGS")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[Conditional("WRN_LOGS")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	[Conditional("MSG_LOGS")]
	public static void LogFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogFormat(format, args);
	}

	[Conditional("ERR_LOGS")]
	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	[Conditional("WRN_LOGS")]
	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}
}
