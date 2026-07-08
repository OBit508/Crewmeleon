using Crewmeleon.Essential;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnityEngine;

namespace Crewmeleon.Components
{
    [FungleAPI.Attributes.RegisterTypeInIl2Cpp]
    public class ChameleonPaint : MonoBehaviour
    {
        public Dictionary<Vector2Int, Color32> PaintStrokes = new Dictionary<Vector2Int, Color32>();

        public const float paintDelay = 0.015f;

        public const int brushSize = 5;
        public const float brushSoftness = 0.3f;
        public const float minAlphaThreshold = 0f;

        public const int cursorPreviewSortingOrderOffset = 1;
        public const float cursorPreviewAlpha = 0.6f;

        private bool canPaint = true;
        public bool CanPaint
        {
            get => canPaint && ChameleonHelper.PaintState == PaintState.Painting && PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.CanMove;
            set
            {
                canPaint = value;

                if (!canPaint)
                {
                    isPainting = false;
                    lastPaintedPixel = new Vector2Int(-1, -1);
                }

                RefreshCursorPreviewVisibility();
            }
        }

        private Texture2D texture;
        public Sprite sprite;
        private SpriteRenderer spriteRenderer;
        private bool[] paintable;

        private bool isPainting;
        private Vector2Int lastPaintedPixel = new Vector2Int(-1, -1);

        private GameObject cursorPreviewObject;
        private SpriteRenderer cursorPreviewRenderer;
        private int cursorPreviewTextureSize = -1;

        private float paintTimer;

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

            cursorPreviewObject = new GameObject("BrushCursorPreview")
            {
                hideFlags = HideFlags.DontSave,
                transform =
                {
                    parent = transform
                }
            };

            cursorPreviewRenderer = cursorPreviewObject.AddComponent<SpriteRenderer>();
            cursorPreviewRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            cursorPreviewRenderer.sortingOrder = spriteRenderer.sortingOrder + cursorPreviewSortingOrderOffset;

            RebuildCursorPreviewSprite();

            cursorPreviewObject.SetActive(false);
        }
        public void Update()
        {
            if (texture == null)
            {
                return;
            }

            if (!CanPaint)
            {
                SetCursorPreviewActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isPainting = true;
                paintTimer = paintDelay;
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

            UpdateCursorPreview();
        }
        private void PaintAtMousePosition()
        {
            Vector2Int? pixel = MouseToPixelCoord();

            if (pixel == null)
            {
                return;
            }

            Vector2Int current = pixel.Value;

            paintTimer += Time.deltaTime;
            if (paintTimer >= paintDelay)
            {
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
                texture.Apply();
                paintTimer = 0;
            }

            lastPaintedPixel = current;
        }
        private Vector2Int? MouseToPixelCoord()
        {
            return MouseToPixelCoord(out _);
        }
        private Vector2Int? MouseToPixelCoord(out Vector3 mouseWorld)
        {
            mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        private void RebuildCursorPreviewSprite()
        {
            int diameter = Mathf.Max(2, brushSize * 2 + 1);

            if (diameter == cursorPreviewTextureSize && cursorPreviewRenderer.sprite != null)
            {
                return;
            }

            cursorPreviewTextureSize = diameter;

            Texture2D previewTexture = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Vector2 center = new Vector2((diameter - 1) * 0.5f, (diameter - 1) * 0.5f);
            float radius = brushSize;

            Color[] pixels = new Color[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = 0f;

                    if (distance <= radius)
                    {
                        alpha = 1f;

                        if (brushSoftness > 0f)
                        {
                            float edge = radius * (1f - brushSoftness);

                            if (distance > edge)
                            {
                                alpha = 1f - Mathf.InverseLerp(edge, radius, distance);
                            }
                        }
                    }

                    pixels[y * diameter + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            previewTexture.SetPixels(pixels);
            previewTexture.Apply();

            if (cursorPreviewRenderer.sprite != null)
            {
                Destroy(cursorPreviewRenderer.sprite.texture);
                Destroy(cursorPreviewRenderer.sprite);
            }

            float pixelsPerUnit = sprite.pixelsPerUnit;
            cursorPreviewRenderer.sprite = Sprite.Create(previewTexture, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
        private void UpdateCursorPreview()
        {
            RebuildCursorPreviewSprite();

            Vector2Int? pixel = MouseToPixelCoord(out Vector3 mouseWorld);

            if (pixel == null)
            {
                SetCursorPreviewActive(false);
                return;
            }

            SetCursorPreviewActive(true);

            Color previewColor = ChameleonHelper.BrushColor;
            previewColor.a = cursorPreviewAlpha;
            cursorPreviewRenderer.color = previewColor;

            mouseWorld.z = transform.position.z - 0.01f;
            cursorPreviewObject.transform.position = mouseWorld;
        }
        private void SetCursorPreviewActive(bool active)
        {
            if (cursorPreviewObject != null && cursorPreviewObject.activeSelf != active)
            {
                cursorPreviewObject.SetActive(active);
            }
        }
        private void RefreshCursorPreviewVisibility()
        {
            if (!canPaint)
            {
                SetCursorPreviewActive(false);
            }
        }
        public void OnDestroy()
        {
            if (cursorPreviewRenderer != null && cursorPreviewRenderer.sprite != null)
            {
                Destroy(cursorPreviewRenderer.sprite.texture);
                Destroy(cursorPreviewRenderer.sprite);
            }

            if (cursorPreviewObject != null)
            {
                Destroy(cursorPreviewObject);
            }
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

                    Color32 current = texture.GetPixel(x, y);

                    float strength = 1f;

                    if (brushSoftness > 0f)
                    {
                        float edge = radius * (1f - brushSoftness);

                        if (distance > edge)
                        {
                            strength = 1f - Mathf.InverseLerp(edge, radius, distance);
                        }
                    }

                    Color32 result = Color.Lerp(current, ChameleonHelper.BrushColor, strength * ChameleonHelper.BrushColor.a);
                    result.a = current.a;

                    texture.SetPixel(x, y, result);
                }
            }

            PaintStrokes[center] = ChameleonHelper.BrushColor;
        }
    }
}