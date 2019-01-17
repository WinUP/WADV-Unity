using System;

namespace WADV.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SkipAutoRegistrationAttribute : Attribute { }
}