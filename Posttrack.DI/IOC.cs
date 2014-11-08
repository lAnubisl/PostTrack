using System.Configuration;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Posttrack.Data.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.Data;

namespace Posttrack.DI
{
    public sealed class InversionOfControlContainer : IInversionOfControlContainer
    {
        private readonly Container container;

        private static readonly Lazy<InversionOfControlContainer> Manager = new Lazy<InversionOfControlContainer>(() => new InversionOfControlContainer(), true);

        private InversionOfControlContainer()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["mongodb"].ConnectionString;
            container = new Container();
            container.RegisterDelegate<IPackageDAO>(r => new PackageDAO(connectionString), Reuse.Singleton);
            container.Register<IPackagePresentationService, PackagePresentationService>(Reuse.Singleton);
            container.Register<IPackageValidator, PackageValidator>(Reuse.Singleton);
            container.Register<IUpdateSearcher, BelpostSearcher>(Reuse.Singleton);
            container.Register<IMessageSender, EmailMessageSender>(Reuse.Singleton);
            container.Register<IEmailTemplateManager, EmailTemplateManager>(Reuse.Singleton);
            container.Register<IResponseReader, ResponseReader>(Reuse.Singleton);
        }

        private InversionOfControlContainer(Container container)
        {
            this.container = container;
        }

        public void RegisterController(Type serviceType)
        {
            container.Register(serviceType, Reuse.Transient);
        }

        public static InversionOfControlContainer Instance
        {
            get { return Manager.Value; }
        }

        public T Resolve<T>() where T : class
        {
            return (T)this.Resolve(typeof(T));
        }

        public object Resolve(Type serviceType)
        {
            if (container.IsRegistered(serviceType))
            {
                 return container.Resolve(serviceType);
            }
           
            return null;
        }

        IEnumerable<object> IInversionOfControlContainer.ResolveAll(Type serviceType)
        {
            var result = new Collection<object>();
            if (container.IsRegistered(serviceType))
            {
                result.Add(container.Resolve(serviceType));
            }

            return result;
        }

        IInversionOfControlContainer IInversionOfControlContainer.CreateScope()
        {
            return new InversionOfControlContainer(container.OpenScope());
        }

        void IDisposable.Dispose()
        {
            container.Dispose();
        }
    }
}