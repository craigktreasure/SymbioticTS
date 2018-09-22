using System;
using System.Reflection;

namespace SymbioticTS.Core.Visitors
{
    internal abstract class TsTypeVisitor
    {
        public void Visit(Type type)
        {
            this.VisitType(type);
        }

        protected virtual void VisitInterfaceType(Type type)
        {
            this.VisitType(type);
        }

        protected virtual void VisitBaseType(Type type)
        {
            this.VisitType(type);
        }

        protected virtual void VisitProperty(PropertyInfo propertyInfo)
        {
            this.VisitPropertyType(propertyInfo.PropertyType);
        }

        protected virtual void VisitPropertyType(Type propertyType)
        {
            this.VisitType(propertyType);
        }

        protected virtual void VisitType(Type type)
        {
            if (type.IsArray)
            {
                this.VisitType(type.GetElementType());
            }

            if (type.IsGenericType)
            {
                foreach (Type genericType in type.GetGenericArguments())
                {
                    this.VisitType(genericType);
                }
            }

            if (type.IsNetFramework())
            {
                return;
            }

            if (type.BaseType != null)
            {
                this.VisitBaseType(type.BaseType);
            }

            foreach (Type interfaceType in type.GetInterfaces())
            {
                this.VisitInterfaceType(interfaceType);
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                this.VisitProperty(propertyInfo);
            }
        }
    }
}