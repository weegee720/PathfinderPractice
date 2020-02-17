using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Рассчитывает путь в тайловом пространстве используя алгоритм A*
/// </summary>
public class Pathfinder 
{
	/// <summary>
	/// Узел графа для расчета пути
	/// </summary>
	private class Node
	{
		/// <summary>
		/// Общий вес узла
		/// </summary>
		public float weight
		{
			get { return distanceWeight + heuristicsWeight; }
		}

		/// <summary>
		/// Длина пути к этому узлу
		/// </summary>
		public float distanceWeight;

		/// <summary>
		/// Значение эвристической функции для этого узла
		/// </summary>
		public float heuristicsWeight;

		/// <summary>
		/// Округленные мировые координаты узла
		/// </summary>
		public Vector2Int pos;

		/// <summary>
		/// Родительский узел
		/// </summary>
		public Node parent;
	}

	/// <summary>
	/// Простая очередь с приоритетами для хранения списка открытых узлов
	/// </summary>
	private class PriorityQueue
	{
		/// <summary>
		/// Коллекция узлов
		/// </summary>
		Dictionary<Vector2Int, Node> _nodes = new Dictionary<Vector2Int, Node>();

		/// <summary>
		/// Добавление узла в очередь
		/// </summary>
		/// <param name="n">Добавляемый узел</param>
		public void Enqueue(Node n)
		{
			_nodes[n.pos] = n;
		}

		/// <summary>
		/// Получение наилучшего узла
		/// </summary>
		/// <returns>Узел с наименьшим весом или null если такого узла нет</returns>
		public Node DequeueBest()
		{
			if (_nodes.Count > 0)
			{
				Node n = null;

				foreach (var kv in _nodes)
				{
					if (n == null || kv.Value.weight < n.weight)
						n = kv.Value;
				}

				_nodes.Remove(n.pos);
				return n;
			}

			return null;
		}

		/// <summary>
		/// Проверяет есть ли в очереди узел с меньшим весом чем у указанного
		/// </summary>
		/// <param name="n">Узел для проверки</param>
		/// <returns>True если в очереди есть узел с меньшим весом</returns>
		public bool HasBetterElement(Node n)
		{
			if (_nodes.ContainsKey(n.pos))
			{
				if (_nodes[n.pos].weight < n.weight)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Количество узлов в очереди
		/// </summary>
		public int Count
		{
			get { return _nodes.Count; }
		}
	}

	/// <summary>
	/// Делегат для проверки наличия препятствий по указанным координатам
	/// </summary>
	private Func<Vector2Int, bool> _hasObstaclesDelegate;

	/// <summary>
	/// Массив для нахождения координат смежных узлов
	/// </summary>
	private static Vector2Int[] _adjacentOffsets =
		{
			new Vector2Int(-1, 1), new Vector2Int(0, 1),  new Vector2Int(1, 1),
			new Vector2Int(-1, 0),                        new Vector2Int(1, 0),
			new Vector2Int(-1,-1), new Vector2Int(0, -1), new Vector2Int(1, -1)
		};

	/// <summary>
	/// Очередь открытых узлов
	/// </summary>
	PriorityQueue _openNodes = new PriorityQueue();

	/// <summary>
	/// Коллекция закрытых узлов
	/// </summary>
	Dictionary<Vector2Int, Node> _closedNodes = new Dictionary<Vector2Int, Node>();

	public Pathfinder(Func<Vector2Int, bool> hasObstaclesDelegate)
	{
		if (hasObstaclesDelegate == null)
			throw new Exception("Pathfinder: Delegate cannot be null!");

		_hasObstaclesDelegate = hasObstaclesDelegate;
	}

	/// <summary>
	/// Эвристика для расчета веса эврстического компонента веса узла. Используется евклидово расстояние до точки назначения 
	/// </summary>
	/// <param name="thisPos">Координаты текущего узла</param>
	/// <param name="destPos">Координаты точки назначения</param>
	/// <returns>Расстояние от текущего узла до точки назначения</returns>
	private float EuclideHeuristics(Vector2Int thisPos, Vector2Int destPos)
	{
		return (destPos - thisPos).magnitude;
	}

	/// <summary>
	/// Эвристика для расчета веса эврстического компонента веса узла. Используется расстояние Чебышева до точки назначения 
	/// </summary>
	/// <param name="thisPos">Координаты текущего узла</param>
	/// <param name="destPos">Координаты точки назначения</param>
	/// <returns>Расстояние от текущего узла до точки назначения</returns>
	private float ChebyshevHeuristics(Vector2Int thisPos, Vector2Int destPos)
	{
		return Mathf.Max(Mathf.Abs(thisPos.x - destPos.x), Mathf.Abs(thisPos.y - destPos.y));
	}

	/// <summary>
	/// Возвращает список координат доступных для перемещение соседних узлов из текущего
	/// </summary>
	/// <param name="parentNode">Родительский узел вокруг которого ищутся смежные</param>
	/// <returns>Список смежных узлов</returns>
	private List<Node> GetAdjacentNodes(Node parentNode)
	{
		List<Node> result = new List<Node>();

		for (int i = 0; i < _adjacentOffsets.Length; i++)
		{
			if (!_hasObstaclesDelegate(parentNode.pos + _adjacentOffsets[i]))
			{
				Node n = new Node();

				n.parent = parentNode;
				n.pos = parentNode.pos + _adjacentOffsets[i];
				// Положим диагонали равными обычным перемещениям
				n.distanceWeight = parentNode.distanceWeight + 1;

				result.Add(n);
			}
		}

		return result;
	}

	/// <summary>
	/// Находит путь от from до to и возвращает список с координатами шагов этого пути либо пустой список если путь не найден
	/// </summary>
	/// <param name="from">Мировые координаты начала пути</param>
	/// <param name="to">Мировые координаты точки назначения</param>
	/// <returns>Список шагов пути либо пустой список если путь не найден</returns>
	public List<Vector3> FindPath(Vector3 from, Vector3 to)
	{
		Vector2Int dest = new Vector2Int((int)to.x, (int)to.z);

		// Поместим начальный узел в очередь открытых узлов
		Node root = new Node();
		root.pos = new Vector2Int((int)from.x, (int)from.z);

		_openNodes.Enqueue(root);

		// Проходим по открытым узлам
		while (_openNodes.Count > 0)
		{

			Node thisNode = _openNodes.DequeueBest();

			// Проверяем смежные узлы
			foreach(Node candidateNode in GetAdjacentNodes(thisNode))
			{
				// Расчитываем эвристику для соседнего узла
				candidateNode.heuristicsWeight = EuclideHeuristics(candidateNode.pos, dest);


				if (candidateNode.pos != dest)
				{
					// Точка назначения не достигнута
					if (!_openNodes.HasBetterElement(candidateNode))
					{
						// Проверяем нет ли в закрытых узла с меньшим весом чем у текущего
						if (_closedNodes.ContainsKey(candidateNode.pos))
						{
							Node closedNode = _closedNodes[candidateNode.pos];

							if (closedNode.weight > candidateNode.weight)
								_openNodes.Enqueue(candidateNode);
						}
						else
						{
							_openNodes.Enqueue(candidateNode);
						}
					}
				}
				else
				{
					// Точка назначения достигнута
					List<Vector3> result = new List<Vector3>();

					Node pathNode = candidateNode;

					// Формируем результат проходя по ссылкам на родителей от текущего узла до корневого
					while (pathNode.parent != null)
					{
						result.Insert(0, new Vector3(pathNode.pos.x + 0.5f, 0, pathNode.pos.y + 0.5f));
						pathNode = pathNode.parent;
					}

					return result;
				}
			}

			_closedNodes[thisNode.pos] = thisNode;
		}

		return new List<Vector3>();
	}

	/// <summary>
	/// Возвращяет прямой путь от from до to без обхода препятствий и учета границ
	/// </summary>
	/// <param name="from">Мировые координаты начала пути</param>
	/// <param name="to">Мировые координаты точки назначения</param>
	/// <returns>Список шагов пути</returns>
	public List<Vector3> FindDirectPath(Vector3 from, Vector3 to)
	{
		List<Vector3> result = new List<Vector3>();

		float scale = 1 / ((to - from).magnitude);
		float f = 0;

		while (f < 1)
		{
			f += scale;
			result.Add(Vector3.Lerp(from, to, f));
		}

		return result;
	}
}
