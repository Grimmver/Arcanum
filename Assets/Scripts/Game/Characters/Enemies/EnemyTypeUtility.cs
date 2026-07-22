using System;

public static class EnemyTypeUtility
{
	public static Type GetDirectSubclassBeforeBase(Type instanceType, Type baseType)
	{
		if (!baseType.IsAssignableFrom(instanceType) || instanceType == baseType)
			return null;

		Type current = instanceType;

		while (current != null && current.BaseType != null)
		{
			if (current.BaseType == baseType)
				return current;

			current = current.BaseType;
		}

		return null;
	}

	public static string GetEnemyCategoryName(object obj)
	{
		Type subclass = GetDirectSubclassBeforeBase(obj.GetType(), typeof(Enemy));
		return subclass != null ? subclass.Name : "Unknown";
	}
}
