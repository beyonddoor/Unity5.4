using System;

namespace Unity.UNetWeaver
{
	public static class Log
	{
		public static Action<string> WarningMethod;

		public static Action<string> ErrorMethod;

		public static void Warning(string msg)
		{
			Log.WarningMethod("UNetWeaver warning: " + msg);
		}

		public static void Error(string msg)
		{
			Log.ErrorMethod("UNetWeaver error: " + msg);
		}
	}
}
