using SFML.System;
using System.Drawing;

namespace MysteryDungeon;

internal class DungeonGrid
{
	private const uint MinimumRoomSize = 3;

	private byte[,] _grid;

	public Vector2u Size { get; init; }

	public DungeonGrid(Vector2u size)
	{
		this.Size = size;
		this._grid = new byte[this.Size.X, this.Size.Y];
	}

	private void FillRect(Vector2u position, Vector2u size, byte fillTile)
	{
		for (uint y = 0; y < size.Y; y++)
		{
			for (uint x = 0; x < size.X; x++)
			{
				_grid[position.X + x, position.Y + y] = fillTile;
			}
		}
	}

	public void Generate(Vector2u roomGridSize)
	{
		Vector2u roomSize = new(this.Size.X / roomGridSize.X, this.Size.Y / roomGridSize.Y);

		var roomGrid = new byte[roomGridSize.X, roomGridSize.Y];
		Random rng = new();

		this.FillRect(new(0, 0), this.Size, 1);
		for (ushort y = 0; y < roomGridSize.Y; y++)
		{
			for (ushort x = 0; x < roomGridSize.X; x++)
			{
				roomGrid[x, y] = (byte)rng.Next(0, 3);

				if (roomGrid[x, y] == 1)
				{
					this.FillRect(new(x * roomSize.X, y * roomSize.Y), roomSize, 255);

					Vector2u size = new((uint)rng.Next((int)MinimumRoomSize, (int)roomSize.X - 1),
						                (uint)rng.Next((int)MinimumRoomSize, (int)roomSize.Y - 1));

					uint remaining = roomSize - size;

					Vector2u pos = new((x * roomSize.X) + (uint)rng.Next(1, (int)remaining.X - 2),
					                   (y * roomSize.Y) + (uint)rng.Next(1, (int)remaining.Y - 2));

					this.FillRect(pos, size, 0);
				}
			}
		}
	}

	public void Render()
	{
		for (uint y = 0; y < this.Size.Y; y++)
		{
			for (uint x = 0; x < this.Size.X; x++)
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

					case 255:
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("██");
						break;
				}
			}
			Console.Write('\n');
		}
	}
}