using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Обеспечивает обнаружение зарегистрированных менеджеров на сцене. 
/// Менеджер обязан зарегистрироваться в Awake.
/// Каждый менеджер может присутствовать в единственном экземпляре
/// </summary>
public static class ManagerLocator
{
	/// <summary>
	/// Зарегистрированные менеджеры
	/// </summary>
	private static Dictionary<Type, Manager> _managers = new Dictionary<Type, Manager>();

	/// <summary>
	/// Регистрация менеджера
	/// </summary>
	/// <typeparam name="T">Класс менеджера</typeparam>
	/// <param name="m">Объект менеджера</param>
	public static void RegisterManager<T>(Manager m) where T : Manager
	{
		_managers[typeof(T)] = m;
	}

	/// <summary>
	/// Удаление менеджера
	/// </summary>
	/// <typeparam name="T">Класс менеджера</typeparam>
	public static void UnregisterManager<T>()
	{
		Type managerType = typeof(T);

		if (_managers.ContainsKey(managerType))
			_managers.Remove(managerType);
	}

	/// <summary>
	/// Получение менджера. Если менеджера нет будыт вызвано исключение NotImplementedException
	/// </summary>
	/// <typeparam name="T">Класс менеджера</typeparam>
	/// <returns>Ссылка на менеджер либо исключение NotImplementedException если иенеджера нет</returns>
	public static T GetManager<T>() where T: Manager
	{
		Type managerType = typeof(T);

		if (_managers.ContainsKey(managerType))
			return _managers[managerType] as T;

		string errMsg = string.Format("ManagerLocator: Manager {0} not found!", managerType);

		Dbg.LogErrorFormat(errMsg);
		throw new NotImplementedException(errMsg);
	}
}
