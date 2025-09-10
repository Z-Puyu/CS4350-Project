using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataStructuresForUnity.Runtime.GeneralUtils {
    public static class ExpressionTree {
        public delegate P Getter<in T, out P>(T obj);
        
        public delegate void Callable<in T>(T obj);
        
        public delegate void Callable<in T, in A>(T obj, A arg);
        
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
