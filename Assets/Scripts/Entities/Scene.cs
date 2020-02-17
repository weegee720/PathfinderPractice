using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Игровое поле на котором размещены препятствия
/// </summary>
public class Scene : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Scriptable object конфигурации сцены")]
	private SceneConfig _config;

	[SerializeField]
	[Tooltip("Пол сцены")]
	private GameObject _floor;

	/// <summary>
	/// Ширина поля
	/// </summary>
	private int _width;

	/// <summary>
	/// Высота поля
	/// </summary>
	private int _height;

	/// <summary>
	/// Препятствия
	/// </summary>
	private GameObject[,] _obstacles = new GameObject[1, 1];

	private void Awake()
	{
		ApplyConfig(_config);
	}

	/// <summary>
	/// Инициализирует поле используя параметры из конфигурации
	/// </summary>
	/// <param name="cfg">Scriptable object конфигурации сцены</param>
	public void ApplyConfig(SceneConfig cfg)
	{
		_config = cfg;

		if (!_config)
			Dbg.LogError("Scene: Config not provided");

		_width = _config.width;
		_height = _config.height;

		_floor.transform.localScale = new Vector3(_width, 0.1f, _height);
		_obstacles = new GameObject[_width, _height];

		if (_config.layoutData)
		{
			// Парсинг файла расстановки препятствий
			bool[,] dataMap = _config.ParseLayout();

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					if (dataMap[x, _height - y - 1])
						PlaceObstacleAtWorldPos(TranslateScenePos(new Vector2Int(x, y)));
				}
			}
		}
	}

	/// <summary>
	/// Переводит мировые координаты в двумерные целочисленные координаты сцены
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	/// <returns>Координаты сцены</returns>
	public Vector2Int TranslateWorldPos(Vector3 wPos)
	{
		return new Vector2Int((int)wPos.x, (int)wPos.z);
	}

	/// <summary>
	/// Переводит двумерные координаты сцены в мировые координаты
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	/// <returns>Мировые координаты</returns>
	public Vector3 TranslateScenePos(Vector2Int sPos)
	{
		return new Vector3(sPos.x + 0.5f, 0, sPos.y + 0.5f);
	}

	/// <summary>
	/// Проверяет лежат ли мировые координаты в пределах поля, игнорируя высоту
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	/// <returns>True если в пределах поля</returns>
	public bool IsWorldPosWithinBounds(Vector3 wPos)
	{
		return IsScenePosWithinBounds(TranslateWorldPos(wPos));
	}

	/// <summary>
	/// Проверяет лежат ли координаты сцены в пределах поля
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	/// <returns>True если в пределах поля </returns>
	private bool IsScenePosWithinBounds(Vector2Int sPos)
	{
		if (sPos.x < 0 || sPos.y < 0 || sPos.x >= _width || sPos.y >= _height)
			return false;

		return true;
	}

	/// <summary>
	/// Проевряет есть ли препятствие в указанных мировых координатах
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	/// <returns>True если есть препятствие</returns>
	public bool HasObstacleAtWorldPos(Vector3 wPos)
	{
		Vector2Int pos = TranslateWorldPos(wPos);

		return HasObstacleAtScenePos(pos);
	}

	/// <summary>
	/// Проверяет есть ли препятствие в указанных координатах сцены
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	/// <returns>True если есть препятствие</returns>
	private bool HasObstacleAtScenePos(Vector2Int sPos)
	{
		if (IsScenePosWithinBounds(sPos))
			return _obstacles[sPos.x, sPos.y] != null;
		else
			return true;
	}

	/// <summary>
	/// Помещает препятствие в указанных мировых кординатах. Если препятствие уже есть ничего не происходит.
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	public void PlaceObstacleAtWorldPos(Vector3 wPos)
	{
		PlaceObstacleAtScenePos(TranslateWorldPos(wPos));
	}

	/// <summary>
	/// Помещает препятствие в указанных кординатах сцены. Если препятствие уже есть ничего не происходит.
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	private void PlaceObstacleAtScenePos(Vector2Int sPos)
	{
		if (IsScenePosWithinBounds(sPos) && !HasObstacleAtScenePos(sPos))
		{
			GameObject obj = GameObject.Instantiate<GameObject>(_config.obstaclePrefab, TranslateScenePos(sPos), Quaternion.identity, transform);
			_obstacles[sPos.x, sPos.y] = obj;
		}
	}

	/// <summary>
	/// Убирает препятствие из указанных мировых координат если оно там есть.
	/// </summary>
	/// <param name="wPos">Мировые координаты</param>
	public void RemoveObstacleAtWorldPos(Vector3 wPos)
	{
		RemoveObstacleAtScenePos(TranslateWorldPos(wPos));
	}

	/// <summary>
	/// Убирает препятствие из указанных координат сцены если оно есть
	/// </summary>
	/// <param name="sPos">Координаты сцены</param>
	private void RemoveObstacleAtScenePos(Vector2Int sPos)
	{
		if (IsScenePosWithinBounds(sPos) && HasObstacleAtScenePos(sPos))
		{
			Destroy(_obstacles[sPos.x, sPos.y]);
			_obstacles[sPos.x, sPos.y] = null;
		}
	}

	/// <summary>
	/// Возвращает путь от точки from до to в мировых координатах
	/// </summary>
	/// <param name="from">Откуда</param>
	/// <param name="to">Куда</param>
	/// <returns>Список узлов пути в мировых координатах</returns>
	public List<Vector3> FindPath(Vector3 from, Vector3 to)
	{
		Pathfinder pf = new Pathfinder(HasObstacleAtScenePos);
		return pf.FindPath(from, to);
	}
}
