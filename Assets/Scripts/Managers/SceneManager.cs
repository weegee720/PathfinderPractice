using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Обеспечивает взаимодействие со сценой
/// </summary>
public class SceneManager : Manager
{
	[SerializeField]
	[Tooltip("Объект текущей сцены")]
	private Scene _currentScene;

	private void Awake()
	{
		ManagerLocator.RegisterManager<SceneManager>(this);
	}

	private void Start()
	{
		if (!_currentScene)
			Dbg.LogError("SceneManager: Scene not provided");
	}

	/// <summary>
	/// Проевряет есть ли препятствие в указанных мировых координатах
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	/// <returns>True если есть препятствие</returns>
	public bool HasObstacleAtWorldPos(Vector3 wPos)
	{
		return _currentScene.HasObstacleAtWorldPos(wPos);
	}

	/// <summary>
	/// Помещает препятствие в указанных мировых кординатах. Если препятствие уже есть ничего не происходит.
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	public void PlaceObstacleAtWorldPos(Vector3 wPos)
	{
		_currentScene.PlaceObstacleAtWorldPos(wPos);
	}

	/// <summary>
	/// Убирает препятствие из указанных мировых координат если оно там есть.
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	public void RemoveObstacleAtWorldPos(Vector3 wPos)
	{
		_currentScene.RemoveObstacleAtWorldPos(wPos);
	}

	/// <summary>
	/// Переводит двумерные координаты сцены в мировые координаты
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	/// <returns>Мировые координаты</returns>
	public Vector3 TranslateScenePos(Vector2Int sPos)
	{
		return _currentScene.TranslateScenePos(sPos);
	}

	/// <summary>
	/// Возвращает путь от точки from до to в мировых координатах
	/// </summary>
	/// <param name="from">Откуда</param>
	/// <param name="to">Куда</param>
	/// <returns>Список узлов пути в мировых координатах</returns>
	public List<Vector3> FindPath(Vector3 from, Vector3 to)
	{
		return _currentScene.FindPath(from, to);
	}
}
