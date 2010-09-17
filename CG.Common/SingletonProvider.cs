using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG.Common
{
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class SingletonProvider<T> where T : new()
	{
		public static T Instance
		{
			get { return SingletonCreator._instance; }
		}

		[System.Diagnostics.DebuggerStepThrough()]
		class SingletonCreator
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static SingletonCreator()
			{
			}

			internal static readonly T _instance = new T();
		}
	}

	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class SingletonCustomCtorProvider<T> where T : class
	{
		private static readonly object _lock = new object();

		public static T InstanceCustomCtor(Func<T> ctor)
		{
			if (ctor == null)
				throw new ArgumentException("You must provide a valid constructor");

			if (SingletonCreator._instance == null)
			{
				lock (_lock)
				{
					if (SingletonCreator._instance == null)
					{
						SingletonCreator.Initialize(ctor);
					}
				}
			}

			return SingletonCreator._instance;
		}

		[System.Diagnostics.DebuggerStepThrough()]
		class SingletonCreator
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static SingletonCreator()
			{
			}

			internal static void Initialize(Func<T> ctor)
			{
				_instance = ctor();
			}

			internal static T _instance;
		}
	}
}
