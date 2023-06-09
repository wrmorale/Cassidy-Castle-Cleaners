using UnityEngine;

public class SpriteSheetAnimator : MonoBehaviour
{
    public Texture2D spriteSheet;
    public int columns = 4;
    public int rows = 4;
    public float framesPerSecond = 10f;

    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = GenerateSpritesFromSheet();
        InvokeRepeating("NextFrame", 1f / framesPerSecond, 1f / framesPerSecond);
    }

    private Sprite[] GenerateSpritesFromSheet()
    {
        int frameCount = columns * rows;
        int spriteWidth = spriteSheet.width / columns;
        int spriteHeight = spriteSheet.height / rows;

        Sprite[] sprites = new Sprite[frameCount];
        int spriteIndex = 0;

        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                Rect rect = new Rect(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
                Sprite sprite = Sprite.Create(spriteSheet, rect, Vector2.zero);
                sprites[spriteIndex] = sprite;
                spriteIndex++;
            }
        }

        return sprites;
    }

    private void NextFrame()
    {
        currentFrame = (currentFrame + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[currentFrame];
    }
}
