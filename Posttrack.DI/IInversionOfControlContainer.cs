using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
