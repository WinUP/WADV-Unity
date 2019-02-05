using System;
using JetBrains.Annotations;

namespace WADV.Reflection {
    public interface IAssemblyRegister {
        void RegisterType(Type target, [CanBeNull] UseStaticRegistration info, string name);
    }
}