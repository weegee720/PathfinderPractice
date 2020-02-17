using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "ScriptableObjects/SceneConfig")]
public class SceneConfig : ScriptableObject
{
	public int width;
	public int height;

	public GameObject obstaclePrefab;
	public TextAsset layoutData;

	public bool[,] ParseLayout()
	{
		bool[,] map = new bool[width, height];

		if (layoutData)
		{
			string[] lines = layoutData.text.Split(new string[] {"\r\n"}, System.StringSplitOptions.None);

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
