using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDrawer : MonoBehaviour
{
	[SerializeField] private Material _wallMaterial;
	[SerializeField] private Material _floorMaterial;
	[SerializeField] private Material _verticeSphereMaterial;
	private Room room;
	public string json;

	private Dictionary<RoomPart, List<Vector3>> allWallglobalBottomPositions;
	private void Start()
	{
		InitJson();
		DrawRoomPart(room.walls, "Wall", out allWallglobalBottomPositions, _wallMaterial);
		var showerData = GetFloorDataBetweenWalls(new RoomPart[]
		{
			room.walls[1],
			room.walls[2],
			room.walls[3],
			room.walls[4],
			room.walls[5],
		});
		DrawRoomPart(new RoomPart[] { showerData }, "Floor", out _, _floorMaterial);
	}

	private RoomPart GetFloorDataBetweenWalls(RoomPart[] walls)
	{
		var globalFloorVertices = GetWallsBottomVerticeGlobalPositions(allWallglobalBottomPositions, walls);
		RoomPart floorPart = new()
		{
			shape = new Vector2[globalFloorVertices.Count],
			position = CalculateAveragePosition(globalFloorVertices),
			rotation = new Vector3(90, 0, 0)
		};

		// Convert global vertices to local (relative to parent)
		for (int i = 0; i < globalFloorVertices.Count; i++)
		{
			Vector3 verticeOffset = globalFloorVertices[i] - floorPart.position;
			floorPart.shape[i] = new Vector2(verticeOffset.x, verticeOffset.z);
		}
		return floorPart;
	}
	private List<Vector3> GetWallsBottomVerticeGlobalPositions(Dictionary<RoomPart, List<Vector3>> allGlobalVertices, RoomPart[] walls)
	{
		var minDistance = 0.1;
		List<Vector3> result = new();
		foreach (var wall in walls)
		{
			if (!allGlobalVertices.TryGetValue(wall, out var globalVerticePositions))
				continue;
			var lowestYVertices = globalVerticePositions.OrderBy(v => v.y).Take(2).ToList();
			if (lowestYVertices.Count < 2)
				continue;

			List<Vector3> addingRange = new();
			for (var i = 0; i < 2; i++)
			{
				bool isFarEnough = true;
				var addingPosition = lowestYVertices[i];
				foreach (var existingVertex in result)
				{
					var distance = Vector3.Distance(existingVertex, addingPosition);
					if (distance < minDistance)
					{
						isFarEnough = false;
						break;
					}
				}
				if (isFarEnough)
					addingRange.Add(addingPosition);
			}
			if(addingRange.Count > 0)
			{
				if(result.Count > 0)
					addingRange.Sort((a, b) => Vector3.Distance(a, result[^1]).CompareTo(Vector3.Distance(b, result[^1])));
				result.AddRange(addingRange);
			}
		}
		return result;
	}

	private Vector3 CalculateAveragePosition(List<Vector3> vertices)
	{
		Vector3 sum = Vector3.zero;
		foreach (var vertex in vertices)
			sum += vertex;
		return sum / vertices.Count;
	}

	private void InitJson()
	{
		try
		{
			room = JsonUtility.FromJson<Room>(json);
		}
		catch (System.Exception)
		{
			throw;
		}
	}

	private void DrawRoomPart(RoomPart[] roomParts, string partName, out Dictionary<RoomPart, List<Vector3>> allGlobalVertices, Material material)
	{
		allGlobalVertices = new Dictionary<RoomPart, List<Vector3>>();
		if (roomParts == null)
			return;
		var shapeCount = 0;
		foreach (var part in roomParts)
		{
			GameObject roomObject = new($"{partName} - {shapeCount}");
			MeshFilter meshFilter = roomObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = roomObject.AddComponent<MeshRenderer>();

			Mesh mesh = new();
			meshFilter.mesh = mesh;

			Vector3[] vertices = new Vector3[part.shape.Length];
			for (int i = 0; i < part.shape.Length; i++)
			{
				vertices[i] = part.shape[i];
			}

			int[] triangles = new int[(vertices.Length - 2) * 3];
			var isWall = partName == "Wall";
			for (int i = 0; i < vertices.Length - 2; i++)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + (isWall ? 1 : 2);
				triangles[i * 3 + 2] = i + (isWall ? 2 : 1);
			}
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			roomObject.transform.position = part.position;
			roomObject.transform.eulerAngles = part.rotation;

			meshRenderer.material = material;

			allGlobalVertices[part] = new List<Vector3>();
			foreach (var vertex in vertices)
			{
				Vector3 globalPosition = roomObject.transform.TransformPoint(vertex);
				allGlobalVertices[part].Add(globalPosition);
				CreateVertexIndicator(globalPosition, $"{shapeCount}");
			}
			shapeCount++;
		}
	}

	private void CreateVertexIndicator(Vector3 position, string objName)
	{
		GameObject vertexIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		vertexIndicator.transform.SetParent(transform);
		vertexIndicator.transform.position = position;
		vertexIndicator.transform.localScale = Vector3.one * 0.1f;
		vertexIndicator.name = objName;

		var meshRenderer = vertexIndicator.GetComponent<MeshRenderer>();
		meshRenderer.material = _verticeSphereMaterial;
	}
}
