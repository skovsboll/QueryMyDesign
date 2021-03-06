using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Mono.Cecil;
using QueryMyDesign.Sugar;

namespace Reflector
{
	/// <summary>
	/// Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
	/// type.
	/// </summary>
	/// <typeparam name="TTarget">Type to reflect.</typeparam>
	public static class Reflect<TTarget>
	{
		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		public static MethodDefinition GetMethod(Expression<Action<TTarget>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		public static MethodDefinition GetMethod<T1>(Expression<Action<TTarget, T1>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		public static MethodDefinition GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		public static MethodDefinition GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> method)
		{
			return GetMethodInfo(method);
		}

		private static MethodDefinition GetMethodInfo(Expression method)
		{
			if (method == null) throw new ArgumentNullException("method");

			LambdaExpression lambda = method as LambdaExpression;
			if (lambda == null) throw new ArgumentException("Not a lambda expression", "method");
			if (lambda.Body.NodeType != ExpressionType.Call) throw new ArgumentException("Not a method call", "method");

			return ((MethodCallExpression)lambda.Body).Method.GetDefinition();
		}

		public new static TypeDefinition GetType() {
			return typeof(TTarget).GetTypeDefinition();
		}

		///// <summary>
		///// Gets the property represented by the lambda expression.
		///// </summary>
		///// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		///// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a property access.</exception>
		//public static PropertyDefinition GetProperty(Expression<Func<TTarget, object>> property)
		//{
		//      PropertyDefinition info = GetMemberInfo(property) as PropertyDefinition;
		//      if (info == null) throw new ArgumentException("Member is not a property");

		//      return info;
		//}

		///// <summary>
		///// Gets the field represented by the lambda expression.
		///// </summary>
		///// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		///// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a field access.</exception>
		//public static FieldDefinition GetField(Expression<Func<TTarget, object>> field)
		//{
		//      FieldDefinition info = GetMemberInfo(field) as FieldDefinition;
		//      if (info == null) throw new ArgumentException("Member is not a field");

		//      return info;
		//}

		//private static MemberReference GetMemberInfo(Expression member)
		//{
		//      if (member == null) throw new ArgumentNullException("member");

		//      LambdaExpression lambda = member as LambdaExpression;
		//      if (lambda == null) throw new ArgumentException("Not a lambda expression", "member");

		//      MemberExpression memberExpr = null;

		//      // The Func<TTarget, object> we use returns an object, so first statement can be either 
		//      // a cast (if the field/property does not return an object) or the direct member access.
		//      if (lambda.Body.NodeType == ExpressionType.Convert)
		//      {
		//            // The cast is an unary expression, where the operand is the 
		//            // actual member access expression.
		//            memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
		//      }
		//      else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
		//      {
		//            memberExpr = lambda.Body as MemberExpression;
		//      }

		//      if (memberExpr == null) throw new ArgumentException("Not a member access", "member");

		//      return memberExpr.Member;
		//}
	}
}