using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] private BezierCurvesGenerator bezierCurvesGenerator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int normalsPerCurve = 4;
        [SerializeField] private int borderSize = 5;
        [SerializeField] private Gradient grassGradien;
        [SerializeField] private Vector2 grassModifierRange = new(0.8f, 1f);
        [SerializeField] private int polygonPointsPerCurve = 10;
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private TerrainBlock terrainBlockPrefab;

        public const int PIXELS_PER_UNIT = 100;
        private BezierCurve2D[] beziers;

        private float widthUnit;
        private float heightUnit;

        private int widthPixels;
        private int heightPixels;

        Vector2 leftDownCorner;
        Vector2 leftUpCorner;
        Vector2 rightUpCorner;
        Vector2 rightDownCorner;

        private readonly Dictionary<Vector2, Vector2> normals = new();
        private readonly List<Dictionary<Vector2Int, int>> colliderPaths = new();

        public void OnDrawGizmos()
        {
            Gizmos.DrawLine(leftDownCorner, leftUpCorner);
            Gizmos.DrawLine(leftUpCorner, rightUpCorner);
            Gizmos.DrawLine(rightUpCorner, rightDownCorner);
            Gizmos.DrawLine(rightDownCorner, leftDownCorner);
            foreach (var normal in normals)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(normal.Key, normal.Key + normal.Value);
            }
        }

        [ContextMenu("Draw")]
        public void Draw()
        {
            CreateTexture();
            Debug.Log("leftDownCorner: " + leftDownCorner);
            Debug.Log("leftUpCorner: " + leftUpCorner);
            Debug.Log("rightUpCorner: " + rightUpCorner);
            Debug.Log("rightDownCorner: " + rightDownCorner);
        }

        private void CreateTexture()
        {
            normals.Clear();
            beziers = GetBeziers();
            Texture2D texture = DrawLineByBeziers(beziers);
            List<TextureBlock> textures = SeparateTexture(texture);
          
            if (Application.isPlaying)
            {
                spriteRenderer.enabled = false;
                int i = 0;
                foreach (TextureBlock block in textures)
                {
                    TerrainBlock terrain = Instantiate(terrainBlockPrefab, transform);
                    terrain.transform.localPosition = block.localPosition;
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

        private BezierCurve2D[] GetBeziers()
        {
            return bezierCurvesGenerator.Generate();
        }

        private Texture2D DrawLineByBeziers(BezierCurve2D[] beziers)
        {
            float LeftBorder = bezierCurvesGenerator.XBorders.x;
            float RightBorder = bezierCurvesGenerator.XBorders.y;
            float UpBorder = bezierCurvesGenerator.YBorders.y;
            float DownBorder = bezierCurvesGenerator.YBorders.x;

            leftDownCorner = new(LeftBorder, DownBorder);
            leftUpCorner = new(LeftBorder, UpBorder);
            rightUpCorner = new(RightBorder, UpBorder);
            rightDownCorner = new(RightBorder, DownBorder);
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
            float xBlocks = Mathf.Ceil((float)texture.width / 100f);
            float yBlocks = Mathf.Ceil((float)texture.height / 100f);
            for (int i = 0; i < xBlocks; i++)
            {
                for (int j = 0; j < yBlocks; j++)
                {
                    Vector2Int blockPixelPos = BlockCenter(i,j);
                    int xBlockStart = blockPixelPos.x - 50;
                    int yBlockStart = blockPixelPos.y - 50;
                    int blockXWidth = i == xBlocks - 1 ? widthPixels - xBlockStart : 100;
                    int blockYWidth = j == yBlocks - 1 ? heightPixels - yBlockStart : 100;
                    Color[] block = texture.GetPixels(xBlockStart, yBlockStart, blockXWidth, blockYWidth);

                    Texture2D newTexture = new(blockXWidth, blockYWidth);
                    newTexture.alphaIsTransparency = true;
                    newTexture.filterMode = FilterMode.Point;
                    newTexture.SetPixels(block);
                    newTexture.Apply();
                    TextureBlock textureBlock = new();
                    textureBlock.texture = newTexture;
                    textureBlock.localPosition = PixelToLocalPoint(blockPixelPos);
                    textureBlock.size = new Vector2(blockXWidth, blockYWidth);
                    textures.Add(textureBlock);
                }
            }
            return textures;
        }

        private Vector2Int BlockCenter(int xBlock, int yBlock)
        {
            int xPos = xBlock < widthUnit ? (xBlock + 1) * 100 - 50 : widthPixels - 50;
            int yPos = yBlock < heightUnit ? (yBlock + 1) * 100 - 50 : heightPixels - 50;
            return new Vector2Int(xPos, yPos);
        }

        private struct TextureBlock
        {
            public Texture2D texture;
            public Vector2 localPosition;
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
                    normals[curve.GetPoint(progress)] = normal;

                    FillRevertable(texture, pixelCoor + Vector2Int.CeilToInt(-normal * borderSize));
                    FillRevertable(texture, pixelCoor + Vector2Int.CeilToInt(normal * borderSize));
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
        

        private bool IsBoundPixel(Vector2Int pixel, Dictionary<Vector2Int, int> pixels)
        {
            Vector2Int north = new(pixel.x, pixel.y + 1);
            Vector2Int south = new(pixel.x, pixel.y - 1);
            Vector2Int west = new(pixel.x - 1, pixel.y);
            Vector2Int east = new(pixel.x + 1, pixel.y);

            return !(pixels.ContainsKey(north) && pixels.ContainsKey(south) && pixels.ContainsKey(west) && pixels.ContainsKey(east));
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



        private Vector2 PixelToLocalPoint(Vector2Int pixel)
        {
            float xUV = (float)pixel.x / widthPixels;
            float yUV = (float)pixel.y / heightPixels;

            Vector2 localPoint = new((xUV - 0.5f) * widthUnit, (yUV - 0.5f) * heightUnit);

            return localPoint;
        }
    }

}

