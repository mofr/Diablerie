using System;
using System.Resources;

namespace CrystalMpq
{
	internal static class ErrorMessages
	{
		private static readonly ResourceManager resourceManager = new ResourceManager(typeof(ErrorMessages));

		public static string GetString(string name) { return resourceManager.GetString(name); }
	}
}
