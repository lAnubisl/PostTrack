using System;
using System.Collections.Generic;

namespace Posttrack.DI
{
    public interface IInversionOfControlContainer : IDisposable
    {
        T Resolve<T>() where T : class;
        object Resolve(Type serviceType);
        IEnumerable<object> ResolveAll(Type serviceType);
        IInversionOfControlContainer CreateScope();
    }
}