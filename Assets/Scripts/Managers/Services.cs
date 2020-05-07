using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Stores static references to many commonly used scripts and systems in the game
/// </summary>
public static class Services
{
	public static AudioManager Audio;
	public static UIManager UI;
	public static UtilityManager Utility;
	public static PlayerController Player;
	public static GameManager Game;
	public static Camera MainCamera;
	public static SaveSystem Save;
	public static EventManager Events;
	public static ObjectPooler ObjectPools;

	public static void InitializeServices()
	{
		MainCamera = Camera.main;
		Events = new EventManager();
	}
}