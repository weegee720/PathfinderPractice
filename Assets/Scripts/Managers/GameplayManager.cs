using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Управляет игровым процессом и взаимодейтсвием с его объектами
/// </summary>
public class GameplayManager : Manager
{
	[SerializeField]
	[Tooltip("Префаб юнита")]
	private Player _playerPrefab;

	/// <summary>
	/// Управляемый игроком юнит
	/// </summary>
	private Player _player;

	/// <summary>
	/// Менеджер ввода
	/// </summary>
	private InputManager _inputManager;

	/// <summary>
	/// Менеджер сцены
	/// </summary>
	private SceneManager _sceneManager;

	/// <summary>
	/// Текущая камера
	/// </summary>
	private Camera _camera;

	private void Awake()
	{
		ManagerLocator.RegisterManager<GameplayManager>(this);
	}

	private void Start()
	{
		_inputManager = ManagerLocator.GetManager<InputManager>();

		if (!_inputManager)
			Dbg.LogError("GameplayManager: InputManager was not found!");

		_sceneManager = ManagerLocator.GetManager<SceneManager>();

		if (!_sceneManager)
			Dbg.LogError("SceneManager: SceneManager was not found!");

		_camera = Camera.main;

		_inputManager.OnTap += InputManager_OnTap;

		SpawnPlayer();
	}

	private void OnEnable()
	{
		if (_inputManager)
			_inputManager.OnTap += InputManager_OnTap;
	}

	private void OnDisable()
	{
		if (_inputManager)
			_inputManager.OnTap -= InputManager_OnTap;
	}

	/// <summary>
	/// Спавн юнита игрока
	/// </summary>
	private void SpawnPlayer()
	{
		_player = Instantiate<Player>(_playerPrefab);
		if (_player)
			_player.transform.position = _sceneManager.TranslateScenePos(new Vector2Int(2, 4));
		else
			Debug.LogError("GameplayManager: Error spawning player");
	}

	/// <summary>
	/// Обработчик события нажатия от InputManager
	/// </summary>
	/// <param name="tapPosition">Экранные кординаты нажатия</param>
	private void InputManager_OnTap(Vector3 tapPosition)
	{
		bool playerInteracted = TryInteractWithPlayerUnderTap(tapPosition);

		// Если не тапнули по юниту то управляем препятствиями либо отправляем юнита в путь
		if (!playerInteracted)
		{
			if (_player.isSelected)
				_player.SetPath(_sceneManager.FindPath(_player.transform.position, GetPlaneCoords(tapPosition)));
			else
				InteractWithObstacleUnderTap(tapPosition);
		}
	}

	/// <summary>
	/// Выделяет/отключает юнит если тапнули по нему
	/// </summary>
	/// <param name="tapPosition">Экранные координаты нажатия</param>
	/// <returns>True если попали по юниту</returns>
	private bool TryInteractWithPlayerUnderTap(Vector3 tapPosition)
	{
		Ray ray = _camera.ScreenPointToRay(tapPosition);

		int layerMask = 1 << LayerMask.NameToLayer("PlayerCapsule");

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
		{
			if (_player)
				_player.isSelected = !_player.isSelected;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Устанавливает/убирает препятствия в точке тапа заданной в экранных координатах
	/// </summary>
	/// <param name="tapPosition">Точка нажатия в экранных координатах</param>
	private void InteractWithObstacleUnderTap(Vector3 tapPosition)
	{
		Vector3 planePos = GetPlaneCoords(tapPosition);

		if (_sceneManager.HasObstacleAtWorldPos(planePos))
			_sceneManager.RemoveObstacleAtWorldPos(planePos);
		else
			_sceneManager.PlaceObstacleAtWorldPos(planePos);
	}

	/// <summary>
	/// Пересчитывает экранные координаты в мировые координаты на плоскости карты
	/// </summary>
	/// <param name="tapPosition">Экранные координаты</param>
	/// <returns>Мировые координаты</returns>
	private Vector3 GetPlaneCoords(Vector3 tapPosition)
	{
		Ray ray = _camera.ScreenPointToRay(tapPosition);

		int layerMask = 1 << LayerMask.NameToLayer("BasePlane");

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
			return hit.point;

		return Vector3.zero;
	}
}
