using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Параметры юнита игрока
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
	[Tooltip("Скорость перемещения")]
	public float speed;
}
