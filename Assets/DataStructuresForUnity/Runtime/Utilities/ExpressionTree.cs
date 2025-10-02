using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DataStructuresForUnity.Runtime.Utilities {
    public static class ExpressionTree {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public delegate P Getter<in T, out P>(T obj);
        
        public delegate void Callable<in T>(T obj);
        
        public delegate void Callable<in T, in A>(T obj, A arg);

        public static IEnumerable<string> GetProducerMethodNames(this Type type) {
            return type.GetMethods(ExpressionTree.Flags)
                       .Where(m => m.ReturnType != typeof(void) && m.GetParameters().Length == 0 && !m.IsSpecialName)
                       .Select(m => m.Name);
        }

        public static IEnumerable<string> GetPropertyGetterNames(this Type type) {
            return type.GetProperties(ExpressionTree.Flags)
                       .Where(p => p.GetGetMethod(true) is not null)
                       .Select(p => p.Name);
        }
        
        public static IEnumerable<string> GetFieldNames(this Type type) {
            return type.GetFields(ExpressionTree.Flags)
                       .Where(f => !f.IsDefined(typeof(CompilerGeneratedAttribute), false))
                       .Select(f => f.Name);
        }

        /// <summary>
        /// Creates a property accessor lambda expression.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <typeparam name="T">The type of the object which owns the property</typeparam>
        /// <typeparam name="P">The type of the property</typeparam>
        /// <returns>A property accessor lambda expression</returns>
        public static Getter<T, P> PropertyAccessor<T, P>(string propertyName) {
            ParameterExpression owner = Expression.Parameter(typeof(T), "obj");
            MemberExpression member = Expression.Property(owner, propertyName);
            return Expression.Lambda<Getter<T, P>>(member, owner).Compile();
        }
        
        /// <summary>
        /// Creates an invoker for a method accepting a single parameter.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <typeparam name="T">The type of the instance from which the method is invoked</typeparam>
        /// <typeparam name="A">The type of the parameter</typeparam>       
        /// <returns>A method invoker lambda expression</returns>
        /// <exception cref="ArgumentException">Thrown when the method is not found on the instance</exception>
        public static Callable<T, A> MethodInvoker<T, A>(string methodName) {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression arg = Expression.Parameter(typeof(A), "arg");
            MethodInfo method = typeof(T).GetMethod(methodName) ??
                                throw new ArgumentException($"Method {methodName} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(instance, method, Expression.Convert(arg, method.GetParameters()[0].ParameterType));
            return Expression.Lambda<Callable<T, A>>(call, instance, arg).Compile();
        }
        
        /// <summary>
        /// Creates an invoker for a method without parameters.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <typeparam name="T">The type of the instance from which the method is invoked</typeparam>
        /// <returns>A method invoker lambda expression</returns>
        /// <exception cref="ArgumentException">Thrown when the method is not found on the instance</exception>
        public static Callable<T> ActionInvoker<T>(string methodName) {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            MethodInfo method = typeof(T).GetMethod(methodName) ??
                                throw new ArgumentException($"Method {methodName} not found on type {typeof(T)}");
            MethodCallExpression call = Expression.Call(instance, method);
            return Expression.Lambda<Callable<T>>(call, instance).Compile();
        }
    }
}
