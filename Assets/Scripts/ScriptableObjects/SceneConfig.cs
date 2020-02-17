using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Параметры сцены
/// </summary>
[CreateAssetMenu(fileName = "SceneConfig", menuName = "ScriptableObjects/SceneConfig")]
public class SceneConfig : ScriptableObject
{
	[Tooltip("Ширина в тайлах")]
	public int width;
	[Tooltip("Высота в тайлах")]
	public int height;

	[Tooltip("Префаб препятствия")]
	public GameObject obstaclePrefab;
	[Tooltip("Текстовый файл с расстановкой препятствий")]
	public TextAsset layoutData;

	/// <summary>
	/// Парсит расстановку препятствий из текстового файла и возвращает массив
	/// </summary>
	/// <returns>Возвращает массив с картой препятствий</returns>
	public bool[,] ParseLayout()
	{
		bool[,] map = new bool[width, height];

		if (layoutData)
		{
			string text = layoutData.text.Replace("\r\n", "\n");

			string[] lines = text.Split(new string[] {"\n"}, System.StringSplitOptions.None);

			for (int y = 0; y < lines.Length; y++)
			{
				for (int x = 0; x < lines[y].Length; x++)
				{
					if (x < width && y < height && lines[y][x] != '0')
						map[x, y] = true;
				}
			}
		}

		return map;
	}
}
