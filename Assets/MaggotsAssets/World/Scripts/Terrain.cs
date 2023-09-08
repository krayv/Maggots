using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int normalsPerCurve = 4;
        [SerializeField] private int borderSize = 5;
        [SerializeField] private Gradient grassGradien;
        [SerializeField] private Vector2 grassModifierRange = new(0.8f, 1f);
        [SerializeField] private int polygonPointsPerCurve = 10;
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private TerrainBlock terrainBlockPrefab;
        [SerializeField] private float spawnPointMaxDifToUp = 0.1f;
        [SerializeField] private int blockSize = 100;

        public const int PIXELS_PER_UNIT = 100;
        private BezierCurve2D[] curves;
        private BattleStarter starter;


        private float widthUnit;
        private float heightUnit;

        private int widthPixels;
        private int heightPixels;

        private readonly Dictionary<Vector2, Vector2> normals = new();

        public List<Vector2> Generate(BattleStarter starter)
        {
            this.starter = starter;
            this.curves = starter.mapCurves;
            CreateTexture();
            List<Vector2> spawnPoints = new();
            foreach (var normal in normals)
            {
                if (Vector2.Dot(normal.Value, Vector2.up) > 1f - spawnPointMaxDifToUp)
                {
                    spawnPoints.Add(UVToWorldPosition(normal.Key) + Vector2.up);
                }
            }

            return spawnPoints;
        }

        private void CreateTexture()
        {
            normals.Clear();
            Texture2D texture = DrawLineByBeziers(curves);
            List<TextureBlock> textures = SeparateTexture(texture);
          
            if (Application.isPlaying)
            {
                spriteRenderer.enabled = false;
                int i = 0;

                Vector2 center = new(widthUnit * 0.5f, heightUnit * 0.5f);

                foreach (TextureBlock block in textures)
                {
                    TerrainBlock terrain = Instantiate(terrainBlockPrefab, transform);
                    
                    terrain.transform.localPosition = new Vector2(block.gridPosition.x, block.gridPosition.y) * ((float)blockSize / (float)PIXELS_PER_UNIT) - center;
                    terrain.name = "Block" + i;
                    terrain.SetTexture(block.texture, block.size, this);
                    
                    i++;
                }
            }
            else
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, widthPixels, heightPixels), new Vector2(0.5f, 0.5f));
                spriteRenderer.sprite = sprite;
            }
        }

        private Texture2D DrawLineByBeziers(BezierCurve2D[] beziers)
        {
            float LeftBorder = starter.mapXBorders.x;
            float RightBorder = starter.mapXBorders.y;
            float UpBorder = starter.mapYBorders.y;
            float DownBorder = starter.mapYBorders.x;

            widthUnit = Mathf.Abs(LeftBorder) + Mathf.Abs(RightBorder);
            heightUnit = Mathf.Abs(DownBorder) + Mathf.Abs(UpBorder);

            widthPixels = (int) (widthUnit * PIXELS_PER_UNIT);
            heightPixels = (int) (heightUnit * PIXELS_PER_UNIT);

            Texture2D texture = new(widthPixels, heightPixels);

            texture.alphaIsTransparency = true;
            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < widthPixels; x++)
            {
                for (int y = 0; y < heightPixels; y++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            foreach (BezierCurve2D curve in beziers)
            {
                DrawCurve(curve, texture);
            }

            DrawGround(texture, beziers);
            texture.Apply();

            return texture;
        }

        private List<TextureBlock> SeparateTexture(Texture2D texture)
        {
            List<TextureBlock> textures = new();
            float xBlocks = Mathf.Ceil((float)texture.width / blockSize);
            float yBlocks = Mathf.Ceil((float)texture.height / blockSize);
            for (int i = 0; i < xBlocks; i++)
            {
                for (int j = 0; j < yBlocks; j++)
                {
                    Vector2Int blockPixelPos = BlockCenter(i,j);
                    int xBlockStart = blockPixelPos.x - blockSize / 2;
                    int yBlockStart = blockPixelPos.y - blockSize / 2;
                    int blockXWidth = i == xBlocks - 1 ? widthPixels - xBlockStart : blockSize;
                    int blockYWidth = j == yBlocks - 1 ? heightPixels - yBlockStart : blockSize;
                    Color[] block = texture.GetPixels(xBlockStart, yBlockStart, blockXWidth, blockYWidth);

                    Texture2D newTexture = new(blockXWidth, blockYWidth);
                    newTexture.alphaIsTransparency = true;
                    newTexture.filterMode = FilterMode.Point;
                    newTexture.SetPixels(block);
                    newTexture.Apply();
                    TextureBlock textureBlock = new();
                    textureBlock.texture = newTexture;
                    textureBlock.size = new Vector2(blockXWidth, blockYWidth);
                    textureBlock.gridPosition = new Vector2Int(i, j);
                    textures.Add(textureBlock);
                }
            }
            return textures;
        }

        private Vector2Int BlockCenter(int xBlock, int yBlock)
        {
            float ratio = (float)PIXELS_PER_UNIT / (float)blockSize;
            int xPos = xBlock < widthUnit * ratio ? (xBlock + 1) * blockSize - blockSize / 2 : widthPixels - blockSize / 2;
            int yPos = yBlock < heightUnit * ratio ? (yBlock + 1) * blockSize - blockSize / 2 : heightPixels - blockSize / 2;
            return new Vector2Int(xPos, yPos);
        }

        private struct TextureBlock
        {
            public Texture2D texture;
            public Vector2Int gridPosition;
            public Vector2 size;
        }

        private Color GetRandomGroundColor()
        {
            return new Color(Random.Range(0.3f, 0.4f), Random.Range(0.2f, 0.3f), Random.Range(0f, 0.1f), 1f);
        }

        private void DrawCurve(BezierCurve2D curve, Texture2D texture)
        {
            int curveLength = (int)(curve.Length * PIXELS_PER_UNIT);
            for (int i = 0; i < curveLength; i++)
            {
                float progress = (float)i / curveLength;
                Vector2 point = curve.GetPointUV(progress);
                Vector2Int pixelCoor = UVToPixelPoint(point);
                DrawGrass(pixelCoor, grassGradien, texture, curve.GetNormal(progress),  borderSize);
            }
        }

        private Vector2Int UVToPixelPoint(Vector2 uv)
        {
            return new((int)(widthPixels * uv.x), (int)(heightPixels * uv.y));
        }

        private void DrawGround(Texture2D texture, BezierCurve2D[] curves)
        {
            foreach (BezierCurve2D curve in curves)
            {
                for (int i = 1; i < normalsPerCurve; i++)
                {
                    float progress = (float)i / (float)normalsPerCurve;
                    Vector2 point = curve.GetPointUV(progress);
                    Vector2Int pixelCoor = new((int)(widthPixels * point.x), (int)(heightPixels * point.y));

                    Vector2 normal = curve.GetNormal(progress);
                    normals[curve.GetPointUV(progress)] = normal;

                    if (i == 1)
                    {
                        Vector2Int p1 = pixelCoor + Vector2Int.CeilToInt(-normal * borderSize);
                        p1 = new Vector2Int(Mathf.Abs(p1.x), Mathf.Abs(p1.y));
                        FillRevertable(texture, p1);
                    }                
                }               
            }
        }

        private void DrawGrass(Vector2Int pixel, Gradient gradient, Texture2D texture, Vector2 normal, int radius)
        {
            List<Vector2Int> pixels = GetCirclePixels(pixel, radius);
            foreach (Vector2Int pixelInCircle in pixels)
            {
                if ((pixelInCircle - pixel).magnitude < radius)
                {
                    float t = VectorExtensions.InverseLerp((normal * radius + pixel), (-normal * radius + pixel), pixelInCircle);
                    t = Mathf.Clamp01(t);
                    texture.SetPixel(pixelInCircle.x, pixelInCircle.y, gradient.Evaluate(t) * Random.Range(grassModifierRange.x, grassModifierRange.y));
                }
            }
        }

        private List<Vector2Int> GetCirclePixels(Vector2Int pixel, int radius)
        {
            int xMaxIndex = pixel.x + radius - 1 < widthPixels ? pixel.x + radius - 1 : widthPixels;
            int xMinIndex = pixel.x - radius + 1 >= 0 ? pixel.x - radius + 1 : 0;
            int yMaxIndex = pixel.y + radius + 1 < heightPixels ? pixel.y + radius - 1 : heightPixels;
            int yMinIndex = pixel.y - radius - 1 >= 0 ? pixel.y - radius + 1 : 0;

            List<Vector2Int> pixels = new((xMaxIndex - xMinIndex + 1) * (yMaxIndex - yMinIndex + 1));

            int i = 0;

            for (int x = xMinIndex; x <= xMaxIndex; x++)
            {
                for (int y = yMinIndex; y <= yMaxIndex; y++)
                {
                    if ((new Vector2Int(x, y) - pixel).magnitude <= radius)
                    {
                        pixels.Add(new Vector2Int(x,y));
                        i++;
                    }
                }
            }
            return pixels;
        }    

        private void FillRevertable(Texture2D texture, Vector2Int point)
        {
            Queue<Vector2Int> pixelsToReplace = new();
            var pixelData = texture.GetPixels();

            if (pixelData[Vector2IntToArrayIndex(point)] != Color.clear)
            {
                return;
            }
            pixelsToReplace.Enqueue(point);
            while (pixelsToReplace.Count > 0)
            {
                Vector2Int pixel = pixelsToReplace.Dequeue();
                if (pixelData[Vector2IntToArrayIndex(pixel)] == Color.clear)
                {
                    pixelData[Vector2IntToArrayIndex(pixel)] = GetRandomGroundColor();
                }

                if (pixel.x > 0)
                {
                    Vector2Int westPixel = new(pixel.x - 1, pixel.y);
                    ReplacePixel(westPixel, pixelsToReplace, pixelData);
                }
                else
                {
                    return;
                }
               

                if (pixel.x < widthPixels - 1)
                {
                    Vector2Int eastPixel = new(pixel.x + 1, pixel.y);
                    ReplacePixel(eastPixel, pixelsToReplace, pixelData);
                }
                else
                {
                    return;
                }


                if (pixel.y > 0)
                {
                    Vector2Int southPixel = new(pixel.x, pixel.y - 1);
                    ReplacePixel(southPixel, pixelsToReplace, pixelData);
                }
                else
                {
                    return;
                }


                if (pixel.y < heightPixels - 1)
                {
                    Vector2Int northPixel = new(pixel.x, pixel.y + 1);
                    ReplacePixel(northPixel, pixelsToReplace, pixelData);
                }
                else
                {
                    return;
                }
            }
            texture.SetPixels(pixelData);
        }

        private void ReplacePixel(Vector2Int pixel, Queue<Vector2Int> queye, Color[] pixels)
        {
            int index = Vector2IntToArrayIndex(pixel);
            if (pixels[index] == Color.clear)
            {
                queye.Enqueue(pixel);
                pixels[index] = GetRandomGroundColor();
            }
        }

        private int Vector2IntToArrayIndex(Vector2Int vector)
        {
            return vector.y * widthPixels + vector.x;
        }

        private Vector2 LocalPositionToUV(Vector2 position)
        {
            float width = widthPixels;
            float height = heightPixels;
            float widthInUnit = width / PIXELS_PER_UNIT;
            float heightInUnit = height / PIXELS_PER_UNIT;
            position = new Vector2(position.x / widthInUnit + 0.5f, position.y / heightInUnit + 0.5f);
            return position;
        }

        private Vector2 WorldPositionToUV(Vector2 worldPosition)
        {       
            Vector2 localPoint = WorldPositionToLocal(worldPosition);
            return LocalPositionToUV(localPoint);
        }

        private Vector2 WorldPositionToLocal(Vector2 worldPosition)
        {
            var matrix = transform.worldToLocalMatrix;
            return matrix.MultiplyPoint3x4(worldPosition);
        }

        private Vector2 LocalToWorldPosition(Vector2 localPosition)
        {
            var matrix = transform.localToWorldMatrix;
            return matrix.MultiplyPoint3x4(localPosition);
        }

        private Vector2 UVToWorldPosition(Vector2 uv)
        {
            Vector2 localPos = new(uv.x * widthUnit - widthUnit * 0.5f, uv.y * heightUnit - heightUnit * 0.5f);
            return transform.localToWorldMatrix.MultiplyPoint(localPos);
        }

        private Vector2 PixelToLocalPoint(Vector2Int pixel)
        {
            float xUV = (float)pixel.x / widthPixels;
            float yUV = (float)pixel.y / heightPixels;

            Vector2 localPoint = new((xUV - 0.5f) * widthUnit, (yUV - 0.5f) * heightUnit);

            return localPoint;
        }
    }

}

