using System;
using System.Reflection;

namespace SymbioticTS.Core.Visitors
{
    internal abstract class TsTypeVisitor
    {
        /// <summary>
        /// Visits the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Visit(Type type)
        {
            this.VisitType(type);
        }

        /// <summary>
        /// Visits the type of the interface.
        /// </summary>
        /// <param name="type">The type.</param>
        protected virtual void VisitInterfaceType(Type type)
        {
            this.VisitType(type);
        }

        /// <summary>
        /// Visits the type of the base.
        /// </summary>
        /// <param name="type">The type.</param>
        protected virtual void VisitBaseType(Type type)
        {
            this.VisitType(type);
        }

        /// <summary>
        /// Visits the property.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        protected virtual void VisitProperty(PropertyInfo propertyInfo)
        {
            this.VisitPropertyType(propertyInfo.PropertyType);
        }

        /// <summary>
        /// Visits the type of the property.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        protected virtual void VisitPropertyType(Type propertyType)
        {
            this.VisitType(propertyType);
        }

        /// <summary>
        /// Visits the type.
        /// </summary>
        /// <param name="type">The type.</param>
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