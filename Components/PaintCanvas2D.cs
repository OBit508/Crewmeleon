using UnityEngine;

namespace Crewmeleon
{
    [FungleAPI.Attributes.RegisterTypeInIl2Cpp]
    public class ChamaleonPaint : MonoBehaviour
    {
        public Color brushColor = Color.red;
        public int brushSize = 5;
        public float brushSoftness = 0.3f;
        public float minAlphaThreshold = 0f;

        private Texture2D texture;
        private Sprite sprite;
        private SpriteRenderer spriteRenderer;
        private bool[] paintable;

        private bool isPainting;
        private Vector2Int lastPaintedPixel = new Vector2Int(-1, -1);

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            sprite = spriteRenderer.sprite;
            texture = sprite.texture;

            Color[] pixels = texture.GetPixels();
            paintable = new bool[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                paintable[i] = pixels[i].a > minAlphaThreshold;
            }
        }
        public void Update()
        {
            if (texture == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isPainting = true;
                lastPaintedPixel = new(-1, -1);
            }

            if (Input.GetMouseButtonUp(0))
            {
                isPainting = false;
            }

            if (isPainting)
            {
                PaintAtMousePosition();
            }
        }

        private void PaintAtMousePosition()
        {
            Vector2Int? pixel = MouseToPixelCoord();

            if (pixel == null)
            {
                return;
            }

            Vector2Int current = pixel.Value;

            if (lastPaintedPixel.x < 0)
            {
                PaintBrush(current);
            }
            else
            {
                float distance = Vector2Int.Distance(lastPaintedPixel, current);
                int steps = Mathf.Max(1, Mathf.CeilToInt(distance));

                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;

                    PaintBrush(new Vector2Int(Mathf.RoundToInt(Mathf.Lerp(lastPaintedPixel.x, current.x, t)), Mathf.RoundToInt(Mathf.Lerp(lastPaintedPixel.y, current.y, t))));
                }
            }

            lastPaintedPixel = current;
            texture.Apply();
        }

        private Vector2Int? MouseToPixelCoord()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localPos = transform.InverseTransformPoint(mouseWorld);

            float width = sprite.rect.width / sprite.pixelsPerUnit;
            float height = sprite.rect.height / sprite.pixelsPerUnit;

            Vector2 pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);

            float x = (localPos.x + pivot.x * width) / width;
            float y = (localPos.y + pivot.y * height) / height;

            if (x < 0 || x > 1 || y < 0 || y > 1)
            {
                return null;
            }

            if (spriteRenderer.flipX)
            {
                x = 1f - x;
            }

            return new Vector2Int(Mathf.Clamp(Mathf.FloorToInt(x * texture.width), 0, texture.width - 1), Mathf.Clamp(Mathf.FloorToInt(y * texture.height), 0, texture.height - 1));
        }
        private void PaintBrush(Vector2Int center)
        {
            int radius = brushSize;

            int minX = Mathf.Max(0, center.x - radius);
            int maxX = Mathf.Min(texture.width - 1, center.x + radius);
            int minY = Mathf.Max(0, center.y - radius);
            int maxY = Mathf.Min(texture.height - 1, center.y + radius);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2Int pixel = new(x, y);
                    float distance = Vector2Int.Distance(pixel, center);

                    if (distance > radius)
                    {
                        continue;
                    }

                    int index = y * texture.width + x;

                    if (!paintable[index])
                    {
                        continue;
                    }

                    Color current = texture.GetPixel(x, y);

                    float strength = 1f;

                    if (brushSoftness > 0f)
                    {
                        float edge = radius * (1f - brushSoftness);

                        if (distance > edge)
                        {
                            strength = 1f - Mathf.InverseLerp(edge, radius, distance);
                        }
                    }

                    Color result = Color.Lerp(current, brushColor, strength * brushColor.a);
                    result.a = current.a;

                    texture.SetPixel(x, y, result);
                }
            }
        }
        public void SetBrushColor(Color color)
        {
            brushColor = color;
        }
        public void SetBrushSize(int size)
        {
            brushSize = Mathf.Max(1, size);
        }
    }
}