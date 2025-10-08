using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDungeon;
internal class Minimap : Drawable
{
	private DungeonGrid _grid;

	private Texture _texture;
	private Sprite _sprite;

	public Minimap(DungeonGrid dungeonGrid)
	{
		this._grid = dungeonGrid;

		Color[,] pixels = new Color[this._grid.Size.X, this._grid.Size.Y];
		for (int y = 0; y < this._grid?.Size.Y; y++)
		{
			for (int x = 0; x < this._grid?.Size.X; x++)
			{
				pixels[x, y] = this._grid?.Grid?[x, y] switch
				{
					0 => Color.Black,
					1 => Color.Blue,
					_ => Color.Red,
				};
			}
		}

		Image image = new(pixels);

		this._texture = new Texture(image);
		this._sprite = new(this._texture)
		{
			Scale = new(10f, 10f),
		};
	}

	public void Draw(RenderTarget target, RenderStates states)
	{
		target.Draw(this._sprite, states);
	}
}
