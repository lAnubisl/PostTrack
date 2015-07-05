using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Posttrack.DI;

namespace Posttrack.Web
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IInversionOfControlContainer container;

        public DependencyResolver(IInversionOfControlContainer container)
        {
            this.container = container;
        }

        IDependencyScope IDependencyResolver.BeginScope()
        {
            return new DependencyResolver(container.CreateScope());
        }

        object IDependencyScope.GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType)
        {
            return container.ResolveAll(serviceType);
        }

        void IDisposable.Dispose()
        {
            container.Dispose();
        }
    }
}