# Room Drawer

## Assignment Instructions

### Background

In our system, the structural components of a room (walls, ceilings, floors) are represented as arrays of JSON objects with the following properties:

- **Shape**: An array of `Vector2` coordinates (objects with numerical properties named `x` and `y`) that define a polygon of appropriate scale in local space.
- **Position**: A `Vector3` representing the component's position in the room.
- **Rotation**: A `Vector3` indicating the component's rotation.

### Drawing Structural Components

To draw a structural component, follow these steps:

1. Draw the shape with the normal facing forward toward the camera. Note that the order of the points is important; each point is connected to the next one in the array, and the final point is connected back to the initial point.
2. Extrude the shape to add depth.
3. Apply the position and rotation vectors to move the structural component to the correct position in world space.

### Showers

Showers are represented in the same format as walls, ceilings, and floors, but they are positioned orthogonally to the walls (on the same plane as the floors) and extruded upwards.

## Question

Below is the wall data for a room along with a **RED** area indicating where the customer would like the shower to be placed. Given this, what should the shower object data be?

## Code explanation
RoomDrawer is a Unity project designed to generate 3D rooms by defining wall positions and creating floor areas based on selected walls.

## Features

- Define walls and automatically generate floor areas.
- Simple customization by specifying wall numbers for each floor area.

## Example Usage

With RoomDrawer, you can specify wall numbers to generate floors within a specific area. Below are two examples:

### Example 1: Creating a Floor Using Walls 1, 2, 3, 4, and 5

The following code snippet will create a floor bounded by walls 1, 2, 3, 4, and 5:

```csharp
var showerData = GetFloorDataBetweenWalls(new RoomPart[]
{
    room.walls[1],
    room.walls[2],
    room.walls[3],
    room.walls[4],
    room.walls[5],
});
DrawRoomPart(new RoomPart[] { showerData }, "Floor", out _, _floorMaterial);
```
<img width="308" alt="Screenshot 2024-10-31 153705" src="https://github.com/user-attachments/assets/fabf6508-1cee-4286-ba03-ea596c394213">


### Example 2: Creating a Floor Using Walls 0,9

For a different floor layout, you can select a different set of walls, such as 0, 9

```csharp
var showerData = GetFloorDataBetweenWalls(new RoomPart[]
{
    room.walls[0],
    room.walls[9]
});
DrawRoomPart(new RoomPart[] { showerData }, "Floor", out _, _floorMaterial);
```
<img width="286" alt="Screenshot 2024-10-31 153835" src="https://github.com/user-attachments/assets/924a64b1-f9a3-4bfa-ac21-566fe9632a54">


## How It Works
The code uses a function called GetFloorDataBetweenWalls which takes an array of walls and automatically calculates the floor shape based on the positions of these walls. By simply specifying wall numbers, the floor is created in the specified area.
