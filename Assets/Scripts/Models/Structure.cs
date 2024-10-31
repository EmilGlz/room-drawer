using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RoomPart
{
	public Vector2[] shape;
	public Vector3 position;
	public Vector3 rotation;
}

[System.Serializable]
public class Room
{
	public RoomPart[] walls;
	public RoomPart ceiling;
	public RoomPart floor;
}
