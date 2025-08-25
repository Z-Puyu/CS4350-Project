using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    public class TreeDropdownAttribute: PathedDropdownAttribute
    {
        public TreeDropdownAttribute(string funcName = null, EUnique unique = EUnique.None): base(funcName, unique)
        {
        }

        public TreeDropdownAttribute(EUnique unique) : base(unique) {}
    }
}
