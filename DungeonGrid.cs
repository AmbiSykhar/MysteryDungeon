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
		Vector2i size = new(Common.RNG.Next(MinimumRoomSize, cellSize.X - 1),
										Common.RNG.Next(MinimumRoomSize, cellSize.Y - 1));

		Vector2i remaining = cellSize - size;

		cell.RoomPosition = new(Common.RNG.Next(1, remaining.X - 1),
								Common.RNG.Next(1, remaining.Y - 1));
		cell.RoomSize = size;

		return cell;
	}

	private static Cell GenerateHallway(Cell cell, Vector2i cellSize)
	{
		cell.RoomPosition = new(Common.RNG.Next(1, cellSize.X - 1),
								Common.RNG.Next(1, cellSize.Y - 1));
		cell.RoomSize = new(1, 1);

		return cell;
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
				Vector2i cellPosition = new(x * cellSize.X, y * cellSize.Y);

				if (cellGrid[x, y].Room)
				{
					var cell = GenerateRoom(currentCell, cellSize);
					this.FillRect(cellPosition + cell.RoomPosition, cell.RoomSize, 0);
					cellGrid[x, y] = cell;
				}
				else if (cellGrid[x, y].HallwayDown || cellGrid[x, y].HallwayRight)
				{
					var cell = GenerateHallway(currentCell, cellSize);
					this.FillRect(cellPosition + cell.RoomPosition, cell.RoomSize, 0);
					cellGrid[x, y] = cell;
				}
			}
		}

		for (ushort y = 0; y < cellGridSize.Y; y++)
		{
			for (ushort x = 0; x < cellGridSize.X; x++)
			{
				var currentCell = cellGrid[x, y];
				if (currentCell.HallwayRight)
				{
					var rightCell = cellGrid[x + 1, y];
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
				}
			}
			Console.Write('\n');
		}
	}

	public struct Cell
	{
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
