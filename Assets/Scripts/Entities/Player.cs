using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляемый игроком юнит
/// </summary>
public class Player : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Scriptable object конфигурации игрока")]
	private PlayerConfig _playerConfig = null; 

	/// <summary>
	/// Исходный размер капсулы
	/// </summary>
	private Vector3 _originalScale;

	/// <summary>
	/// Скорость перемещения ед/сек
	/// </summary>
	private float _speed = 1;

	/// <summary>
	/// True если юнит выбран
	/// </summary>
	private bool _isSelected;

	/// <summary>
	/// True если юнит в движении
	/// </summary>
	private bool _isMoving;

	/// <summary>
	/// Текущий путь по которому двигается юнит
	/// </summary>
	private List<Vector3> _path = new List<Vector3>();

	private void Awake()
	{
		_originalScale = transform.localScale;

		if (_playerConfig && _playerConfig.speed > 0)
			_speed = _playerConfig.speed;
	}

	private void Update()
	{
		if (!_isMoving && _path.Count > 0)
		{
			//Перемещаем юнит на один шаг текущего пути
			Vector3 dest = _path[0];
			_path.RemoveAt(0);

			if (transform.position != dest)
				StartCoroutine(MoveCoroutine(transform.position, dest));
		}
	}

	/// <summary>
	/// Установка нового пути для перемещения юнита
	/// </summary>
	/// <param name="path">Список узлов нового пути</param>
	public void SetPath(List<Vector3> path)
	{
		_path = path;
	}

	/// <summary>
	/// True если юнит выбран
	/// </summary>
	public bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;

			if (_isSelected)
				transform.localScale = _originalScale * 1.25f;
			else
				transform.localScale = _originalScale;
		}
	}

	/// <summary>
	/// Плавно перемещает юнит на один шаг текущего пути
	/// </summary>
	/// <param name="from">Откуда</param>
	/// <param name="to">Куда</param>
	/// <returns>Итератор сопрограммы</returns>
	private IEnumerator MoveCoroutine(Vector3 from, Vector3 to)
	{
		float timeElapsed = 0;
		float timeTotal = (to - from).magnitude / _speed;

		_isMoving = true;

		while (timeElapsed < timeTotal)
		{
			timeElapsed = Mathf.Min(timeTotal, timeElapsed + Time.deltaTime);
			transform.position = Vector3.Lerp(from, to, timeElapsed / timeTotal);

			yield return 0;
		}

		_isMoving = false;
	}
}
