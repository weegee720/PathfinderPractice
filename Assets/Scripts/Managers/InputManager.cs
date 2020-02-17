using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Обуспечивает обработку событий ввода
/// </summary>
public class InputManager : Manager
{
	/// <summary>
	/// Событие одиночного нажатия на экран. Передаются экранные координаты нажатия
	/// </summary>
	public event Action<Vector3> OnTap = delegate { };

	private void Awake()
	{
		ManagerLocator.RegisterManager<InputManager>(this);
	}

	private void Update()
    {
		if (Input.GetMouseButtonUp(0))
		{
			OnTap.Invoke(Input.mousePosition);
		}
    }
}
