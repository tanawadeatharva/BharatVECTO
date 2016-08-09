using System;
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// StopWatch which works like an IDisposable-Context.
	/// Usage: using(new ContextStopWatch("main")) { ... }
	/// </summary>
	public class ContextStopWatch : Stopwatch, IDisposable
	{
		private readonly string _name;

		public ContextStopWatch(string name = null)
		{
			Start();
			_name = name;
		}

		public void Dispose()
		{
			Stop();
			if (_name != null)
				Console.WriteLine("{0}: {1}", _name, Elapsed);
			else
				Console.WriteLine(Elapsed);
		}
	}
}