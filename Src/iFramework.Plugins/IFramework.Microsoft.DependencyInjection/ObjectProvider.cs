﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFramework.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace IFramework.DependencyInjection.Microsoft
{
    public class ObjectProvider : IObjectProvider
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly IObjectProvider _extendedObjectProvider;


        public ObjectProvider(IServiceCollection serviceCollection)
        {
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        internal ObjectProvider(IServiceScope serviceScope, IObjectProvider extendedObjectProvider, IObjectProvider parent)
            : this(serviceScope, parent)
        {
            _extendedObjectProvider = extendedObjectProvider;
        }

        public ObjectProvider(IServiceScope serviceScope, IServiceCollection serviceCollection, IObjectProvider parent)
            : this(serviceScope, new ObjectProvider(serviceCollection), parent)
        {
        }

        public ObjectProvider(IServiceScope serviceScope, IObjectProvider parent)
        {
            _serviceScope = serviceScope;
            _serviceProvider = serviceScope.ServiceProvider;
            Parent = parent;
        }

        public void Dispose()
        {
            if (_serviceScope != null)
            {
                _serviceScope.Dispose();
            }
            else
            {
                (_serviceProvider as ServiceProvider)?.Dispose();
            }
            _extendedObjectProvider?.Dispose();
        }

        public IObjectProvider Parent { get; }
        public IObjectProvider CreateScope()
        {
            return new ObjectProvider(_serviceProvider.CreateScope(), this);
        }

        public IObjectProvider CreateScope(IServiceCollection serviceCollection)
        {
            return new ObjectProvider(_serviceProvider.CreateScope(), serviceCollection, this);
        }

        public IObjectProvider CreateScope(Action<IObjectProviderBuilder> buildAction)
        {
            if (buildAction == null)
            {
                throw new ArgumentNullException(nameof(buildAction));
            }

            var providerBuilder = new ObjectProviderBuilder();
            buildAction(providerBuilder);
            var provider = providerBuilder.Build();
            return new ObjectProvider(_serviceProvider.CreateScope(), provider, this);
        }

        public object GetService(Type serviceType)
        {
            return _extendedObjectProvider?.GetService(serviceType) ?? _serviceProvider.GetService(serviceType);
        }

        public object GetService(Type t, string name, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public object GetService(Type t, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public T GetService<T>(params Parameter[] overrides)
        {
            if (overrides != null)
            {
                throw new NotImplementedException();
            }
            return (T)GetService(typeof(T));
        }

        public T GetService<T>(string name, params Parameter[] overrides)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllService(Type type, params Parameter[] parameters)
        {
            if (parameters != null)
            {
                throw new NotImplementedException();
            }
            return _extendedObjectProvider?.GetServices(type)
                                           .Union(_serviceProvider.GetServices(type));
        }

        public IEnumerable<T> GetAllService<T>(params Parameter[] parameters)
        {
            if (parameters != null)
            {
                throw new NotImplementedException();
            }
            return GetAllService(typeof(T)).Cast<T>();
        }
    }
}
