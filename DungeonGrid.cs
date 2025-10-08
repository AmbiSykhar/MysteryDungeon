using SFML.System;
using System.Drawing;

namespace MysteryDungeon;

internal class DungeonGrid
{
	private const int MinimumRoomSize = 3;

	private byte[,]? _grid;

	private Vector2i _size;

	public Vector2i Size
	{
		get => this._size;
		set
		{
			this._size = value;
			this._grid = new byte[this._size.X, this._size.Y];
		}
	}

	public bool Generated { get; private set; } = false;

	private void FillRect(Vector2i position, Vector2i size, byte fillTile)
	{
		if (this._grid == null)
			return;

		for (int y = 0; y < size.Y; y++)
		{
			for (int x = 0; x < size.X; x++)
			{
				_grid[position.X + x, position.Y + y] = fillTile;
			}
		}
	}

	private static Cell GenerateRoom(Cell cell, Vector2i cellSize)
	{
		Vector2i size = new(Common.RNG.Next(MinimumRoomSize, cellSize.X - 2),
										Common.RNG.Next(MinimumRoomSize, cellSize.Y - 2));

		Vector2i remaining = cellSize - size;

		cell.RoomPosition = new(Common.RNG.Next(2, remaining.X - 1),
								Common.RNG.Next(2, remaining.Y - 1));
		cell.RoomSize = size;

		return cell;
	}

	private static Cell GenerateHallway(Cell cell, Vector2i cellSize)
	{
		cell.RoomPosition = new(Common.RNG.Next(2, cellSize.X - 1),
								Common.RNG.Next(2, cellSize.Y - 1));
		cell.RoomSize = new(1, 1);

		return cell;
	}

	private void GenerateConnection(Cell cellA, Cell cellB, Vector2i direction)
	{
		Vector2i posA = (direction.X == 1 ?
			new(cellA.RoomSize.X, Common.RNG.Next(0, cellA.RoomSize.Y)) :
			new(Common.RNG.Next(0, cellA.RoomSize.X), cellA.RoomSize.Y))
			+ cellA.Position + cellA.RoomPosition;
		Vector2i posB = (direction.X == 1 ?
			new(-1, Common.RNG.Next(0, cellB.RoomSize.Y)) :
			new(Common.RNG.Next(0, cellB.RoomSize.X), -1))
			+ cellB.Position + cellB.RoomPosition;

		var space = posB - posA;
		var middleDistance = Common.RNG.Next(1, direction.X == 1 ? space.X : space.Y);
		var middleA = direction.X == 1 ? new Vector2i(posA.X + middleDistance, posA.Y) : new Vector2i(posA.X, posA.Y + middleDistance);
		var middleB = direction.X == 1 ? new Vector2i(posA.X + middleDistance, posB.Y) : new Vector2i(posB.X, posA.Y + middleDistance);

		var fromA = middleA - posA;
		var middles = middleB - middleA;
		//if (middleA.X > middleB.X || middleA.Y > middleB.Y)
		//{
		//	middles = middleA - middleB;
		//}
		var toB = posB - middleB;

		this.FillRect(posA, fromA + new Vector2i(1, 1), 0);
		this.FillRect(middleA, middles + new Vector2i(1, 1), 0);
		this.FillRect(middleB, -middles + new Vector2i(1, 1), 0);
		this.FillRect(middleB, toB + new Vector2i(1, 1), 0);
	}

	public void Generate(Vector2i gridSize, Vector2i cellGridSize, int roomCount)
	{
		this.Size = gridSize;
		Vector2i cellSize = new(this.Size.X / cellGridSize.X, this.Size.Y / cellGridSize.Y);

		this.FillRect(new(0, 0), this.Size, 1);

		Cell[,] cellGrid = Generators.CommonWalk(cellGridSize, roomCount);

		for (ushort y = 0; y < cellGridSize.Y; y++)
		{
			for (ushort x = 0; x < cellGridSize.X; x++)
			{
				var currentCell = cellGrid[x, y];
				currentCell.Position = new(x * cellSize.X, y * cellSize.Y);
				cellGrid[x, y] = currentCell;

				if (cellGrid[x, y].Room)
				{
					var cell = GenerateRoom(currentCell, cellSize);
					this.FillRect(currentCell.Position + cell.RoomPosition, cell.RoomSize, 0);
					cellGrid[x, y] = cell;
				}
				else
				{
					var cell = GenerateHallway(currentCell, cellSize);
					if (cellGrid[x, y].HallwayDown || cellGrid[x, y].HallwayRight || (x > 0 && cellGrid[x - 1, y].HallwayRight) || (y > 0 && cellGrid[x, y - 1].HallwayDown))
					{
						this.FillRect(currentCell.Position + cell.RoomPosition, cell.RoomSize, 0);
					}
					cellGrid[x, y] = cell;
				}
			}
		}

		for (ushort y = 0; y < cellGridSize.Y; y++)
		{
			for (ushort x = 0; x < cellGridSize.X; x++)
			{
				var cellA = cellGrid[x, y];

				if (cellA.HallwayDown)
				{
					this.GenerateConnection(cellA, cellGrid[x, y + 1], new(0, 1));
				}
				if (cellA.HallwayRight)
				{
					this.GenerateConnection(cellA, cellGrid[x + 1, y], new(1, 0));
				}
			}
		}

		this.Generated = true;
	}

	public void Render()
	{
		if (this._grid == null || !this.Generated)
			return;

		for (int y = 0; y < this.Size.Y; y++)
		{
			for (int x = 0; x < this.Size.X; x++)
			{
				switch (this._grid[x, y])
				{
				case 0:
					Console.Write("  ");
					break;

				case 1:
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("██");
					break;

				case 254:
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.Write("██");
					break;

				case 255:
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write("██");
					break;
				}
			}
			Console.Write('\n');
		}
	}

	public struct Cell
	{
		public Vector2i Position = new(0, 0);

		public bool Room = false;
		public bool HallwayDown = false;
		public bool HallwayRight = false;

		public Vector2i RoomPosition = new(0, 0);
		public Vector2i RoomSize = new(0, 0);

		public Cell()
		{ }
	}

	/////////////////////////////
	// Dungeon Room Bitfield
	// 00000x00 - Hallway to Down
	// 000000x0 - Hallway to Right
	// 0000000x - Room
	/////////////////////////////

	public static class Generators
	{
		private static readonly Vector2i[] DirectionVectors =
		[
			new( 0, -1), // Up
			new( 0,  1), // Down
			new(-1,  0), // Left
			new( 1,  0), // Right
		];

		public static Cell[,] CommonWalk(Vector2i cellGridSize, int roomCount)
		{
			Vector2i startPosition = new(Common.RNG.Next(0, cellGridSize.X), Common.RNG.Next(0, cellGridSize.Y));

			Cell[,] cellGrid = new Cell[cellGridSize.X, cellGridSize.Y];

			var rooms = 0;
			while (rooms < roomCount)
			{
				var d = Common.RNG.Next(0, 4);
				var direction = DirectionVectors[d];
				var nextSquare = startPosition + direction;

				if (nextSquare.X < 0 || nextSquare.X >= cellGridSize.X ||
					nextSquare.Y < 0 || nextSquare.Y >= cellGridSize.Y)
				{
					continue;
				}

				switch (d)
				{
				case 0: // Up
					cellGrid[nextSquare.X, nextSquare.Y].HallwayDown = true;
					break;

				case 1: // Down
					cellGrid[startPosition.X, startPosition.Y].HallwayDown = true;
					break;

				case 2: // Left
					cellGrid[nextSquare.X, nextSquare.Y].HallwayRight = true;
					break;

				case 3: // Right
					cellGrid[startPosition.X, startPosition.Y].HallwayRight = true;
					break;
				}

				startPosition = nextSquare;

				if (cellGrid[nextSquare.X, nextSquare.Y].Room)
				{
					continue;
				}

				bool room = cellGrid[nextSquare.X, nextSquare.Y].Room = Common.RNG.Next(0, 2) == 1;
				if (room)
					rooms++;
			}

			return cellGrid;
		}
	}
}
