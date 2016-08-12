using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

internal class AssemblyValidation
{
	[CompilerGenerated]
	private sealed class <ValidationRuleTypesFor>c__Iterator5 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Type>, IEnumerator<Type>
	{
		internal RuntimePlatform platform;

		internal List<Type>.Enumerator <$s_907>__0;

		internal Type <validationType>__1;

		internal int $PC;

		internal Type $current;

		internal RuntimePlatform <$>platform;

		Type IEnumerator<Type>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.System.Collections.Generic.IEnumerable<System.Type>.GetEnumerator();
		}

		[DebuggerHidden]
		IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
		{
			if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
			{
				return this;
			}
			AssemblyValidation.<ValidationRuleTypesFor>c__Iterator5 <ValidationRuleTypesFor>c__Iterator = new AssemblyValidation.<ValidationRuleTypesFor>c__Iterator5();
			<ValidationRuleTypesFor>c__Iterator.platform = this.<$>platform;
			return <ValidationRuleTypesFor>c__Iterator;
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			bool flag = false;
			switch (num)
			{
			case 0u:
				if (!AssemblyValidation._rulesByPlatform.ContainsKey(this.platform))
				{
					return false;
				}
				this.<$s_907>__0 = AssemblyValidation._rulesByPlatform[this.platform].GetEnumerator();
				num = 4294967293u;
				break;
			case 1u:
				break;
			default:
				return false;
			}
			try
			{
				switch (num)
				{
				}
				if (this.<$s_907>__0.MoveNext())
				{
					this.<validationType>__1 = this.<$s_907>__0.Current;
					this.$current = this.<validationType>__1;
					this.$PC = 1;
					flag = true;
					return true;
				}
			}
			finally
			{
				if (!flag)
				{
					((IDisposable)this.<$s_907>__0).Dispose();
				}
			}
			this.$PC = -1;
			return false;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 1u:
				try
				{
				}
				finally
				{
					((IDisposable)this.<$s_907>__0).Dispose();
				}
				break;
			}
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	private static Dictionary<RuntimePlatform, List<Type>> _rulesByPlatform;

	public static ValidationResult Validate(RuntimePlatform platform, IEnumerable<string> userAssemblies, params object[] options)
	{
		AssemblyValidation.WarmUpRulesCache();
		string[] array = (userAssemblies as string[]) ?? userAssemblies.ToArray<string>();
		if (array.Length != 0)
		{
			foreach (IValidationRule current in AssemblyValidation.ValidationRulesFor(platform, options))
			{
				ValidationResult result = current.Validate(array, options);
				if (!result.Success)
				{
					return result;
				}
			}
		}
		return new ValidationResult
		{
			Success = true
		};
	}

	private static void WarmUpRulesCache()
	{
		if (AssemblyValidation._rulesByPlatform != null)
		{
			return;
		}
		AssemblyValidation._rulesByPlatform = new Dictionary<RuntimePlatform, List<Type>>();
		Assembly assembly = typeof(AssemblyValidation).Assembly;
		foreach (Type current in assembly.GetTypes().Where(new Func<Type, bool>(AssemblyValidation.IsValidationRule)))
		{
			AssemblyValidation.RegisterValidationRule(current);
		}
	}

	private static bool IsValidationRule(Type type)
	{
		return AssemblyValidation.ValidationRuleAttributesFor(type).Any<AssemblyValidationRule>();
	}

	private static IEnumerable<IValidationRule> ValidationRulesFor(RuntimePlatform platform, params object[] options)
	{
		return from t in AssemblyValidation.ValidationRuleTypesFor(platform)
		select AssemblyValidation.CreateValidationRuleWithOptions(t, options) into v
		where v != null
		select v;
	}

	[DebuggerHidden]
	private static IEnumerable<Type> ValidationRuleTypesFor(RuntimePlatform platform)
	{
		AssemblyValidation.<ValidationRuleTypesFor>c__Iterator5 <ValidationRuleTypesFor>c__Iterator = new AssemblyValidation.<ValidationRuleTypesFor>c__Iterator5();
		<ValidationRuleTypesFor>c__Iterator.platform = platform;
		<ValidationRuleTypesFor>c__Iterator.<$>platform = platform;
		AssemblyValidation.<ValidationRuleTypesFor>c__Iterator5 expr_15 = <ValidationRuleTypesFor>c__Iterator;
		expr_15.$PC = -2;
		return expr_15;
	}

	private static IValidationRule CreateValidationRuleWithOptions(Type type, params object[] options)
	{
		List<object> list = new List<object>(options);
		object[] array;
		ConstructorInfo constructorInfo;
		while (true)
		{
			array = list.ToArray();
			constructorInfo = AssemblyValidation.ConstructorFor(type, array);
			if (constructorInfo != null)
			{
				break;
			}
			if (list.Count == 0)
			{
				goto Block_2;
			}
			list.RemoveAt(list.Count - 1);
		}
		return (IValidationRule)constructorInfo.Invoke(array);
		Block_2:
		return null;
	}

	private static ConstructorInfo ConstructorFor(Type type, IEnumerable<object> options)
	{
		Type[] types = (from o in options
		select o.GetType()).ToArray<Type>();
		return type.GetConstructor(types);
	}

	internal static void RegisterValidationRule(Type type)
	{
		foreach (AssemblyValidationRule current in AssemblyValidation.ValidationRuleAttributesFor(type))
		{
			AssemblyValidation.RegisterValidationRuleForPlatform(current.Platform, type);
		}
	}

	internal static void RegisterValidationRuleForPlatform(RuntimePlatform platform, Type type)
	{
		if (!AssemblyValidation._rulesByPlatform.ContainsKey(platform))
		{
			AssemblyValidation._rulesByPlatform[platform] = new List<Type>();
		}
		if (AssemblyValidation._rulesByPlatform[platform].IndexOf(type) == -1)
		{
			AssemblyValidation._rulesByPlatform[platform].Add(type);
		}
		AssemblyValidation._rulesByPlatform[platform].Sort((Type a, Type b) => AssemblyValidation.CompareValidationRulesByPriority(a, b, platform));
	}

	internal static int CompareValidationRulesByPriority(Type a, Type b, RuntimePlatform platform)
	{
		int num = AssemblyValidation.PriorityFor(a, platform);
		int num2 = AssemblyValidation.PriorityFor(b, platform);
		if (num == num2)
		{
			return 0;
		}
		return (num >= num2) ? 1 : -1;
	}

	private static int PriorityFor(Type type, RuntimePlatform platform)
	{
		return (from attr in AssemblyValidation.ValidationRuleAttributesFor(type)
		where attr.Platform == platform
		select attr.Priority).FirstOrDefault<int>();
	}

	private static IEnumerable<AssemblyValidationRule> ValidationRuleAttributesFor(Type type)
	{
		return type.GetCustomAttributes(true).OfType<AssemblyValidationRule>();
	}
}
