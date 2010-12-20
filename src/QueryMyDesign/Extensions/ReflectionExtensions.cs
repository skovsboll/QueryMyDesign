using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using SkinnyDip.Annotations;

namespace SkinnyDip {
	public static class ReflectionExtensions {

		public static IEnumerable<MethodInfo> GetPublicMethods(this Type type) {
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<IEnumerable<MethodInfo>>() != null);

			return from m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				 where m.Name != "ToString" && m.Name != "GetHashCode"
				 && !m.GetCustomAttributes(typeof(HiddenAttribute), true).Any()
				 select m;
		}

		public static IEnumerable<IDataInfo> GetPublicPropertiesAndFields(this Type type) {
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<IEnumerable<PropertyInfo>>() != null);

			var properties =
				from f in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				where !f.GetCustomAttributes(typeof(HiddenAttribute), true).Any()
				select new PropertyDataInfo(f);

			var fields =
				from f in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				where !f.GetCustomAttributes(typeof(HiddenAttribute), true).Any()
				select new FieldDataInfo(f);

			return properties.OfType<IDataInfo>().Union(fields);
		}

		public static IEnumerable<ConstructorInfo> GetPublicConstructors(this Type type) {
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<IEnumerable<ConstructorInfo>>() != null);

			return from c in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			       where !c.GetCustomAttributes(typeof(HiddenAttribute), true).Any()
			       select c;
		}

	}
}