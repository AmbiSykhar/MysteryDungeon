
using SFML.Graphics;
using SFML.System;

namespace MysteryDungeon;

internal abstract class Entity
{
    public int Id { get; }
    public Vector2i Position { get; set; }
    public Vector2i Direction { get; set; }

    public abstract Color MinimapDraw();

}
