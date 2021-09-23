using System;
using System.Reflection;

namespace EvenMoreDeepWoods
{
    class ReflectionHelper
    {
        private static Assembly GetAssembly(string name)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.GetName().Name == name)
                    return a;

            throw new Exception("Assembly " + name + "does not exist.");
        }

        public static Type GetType(string assemblyName, string typeName)
        {
            foreach (Type t in GetAssembly(assemblyName).GetTypes())
            {
                if (t.Name == typeName)
                    return t;
            }

            throw new Exception("Type " + typeName + " does not exist in assembly " + assemblyName + ".");
        }

        public static Type GetDeepWoodsType(string typeName)
        {
            return GetType("DeepWoodsMod", typeName);
        }
    }
}
