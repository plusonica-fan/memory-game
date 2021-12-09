using UnityEngine;

public static class TexToSprite
{
    public static Sprite Convert(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }
}
