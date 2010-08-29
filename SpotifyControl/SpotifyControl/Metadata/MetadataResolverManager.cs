using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CG.SpotifyControl.Metadata.Resolvers;
using System.ComponentModel;

namespace CG.SpotifyControl.Metadata
{
	public static class MetadataResolverManager
	{
		private static readonly Dictionary<Type, ResolverInfos> _resolversCache = new Dictionary<Type, ResolverInfos>();
		private static IMetadataTrackResolver _resolverInstance;


		static MetadataResolverManager()
		{
			InitResolvers();
			LoadDefaultResolver();
		}

		private static void InitResolvers()
		{
			var resolversType = from type in Assembly.GetExecutingAssembly().GetTypes()
					    where typeof(IMetadataTrackResolver).IsAssignableFrom(type) && type.IsClass
					    select type;

			foreach (Type resolverType in resolversType)
			{
				_resolversCache.Add(resolverType, new ResolverInfos(resolverType));
			}
		}

		private static void LoadDefaultResolver()
		{
			foreach (Type resolverType in _resolversCache.Keys)
			{
				if (resolverType.FullName != Properties.Settings.Default.MetadataResolver)
					continue;

				_resolverInstance = _resolversCache[resolverType].Resolver;
				_resolverInstance.Initialize();
				return;
			}
		}

		public static void UpdateResolver()
		{
			if (_resolverInstance != null)
				_resolverInstance.Dispose();

			LoadDefaultResolver();
		}

		public static IMetadataTrackResolver ResolverInstance
		{
			get { return _resolverInstance; }
		}

		public static List<ResolverInfos> ResolversInfos
		{
			get { return _resolversCache.Values.ToList(); }
		}

		public class ResolverInfos
		{
			private readonly IMetadataTrackResolver _resolver;

			public ResolverInfos(Type resolverType)
			{
				this.ResolverType = resolverType;
				_resolver = (IMetadataTrackResolver)Activator.CreateInstance(resolverType);

				foreach (object attr in resolverType.GetCustomAttributes(false))
				{
					if (attr is DisplayNameAttribute)
						this.DisplayName = ((DisplayNameAttribute)attr).DisplayName;

					if (attr is DescriptionAttribute)
						this.Description = ((DescriptionAttribute)attr).Description;
				}
			}

			public string DisplayName
			{
				get;
				private set;
			}

			public string Description
			{
				get;
				private set;
			}

			public Type ResolverType
			{
				get;
				private set;
			}

			public bool IsSelected
			{
				get
				{
					return (this.ResolverType.FullName == Properties.Settings.Default.MetadataResolver);
				}
			}

			public IMetadataTrackResolver Resolver
			{
				get { return _resolver; }
			}

			public IMetadataTrackResolverOptions ResolverOptions
			{
				get { return _resolver.ResolverOptions; }
			}
		}
	}
}
