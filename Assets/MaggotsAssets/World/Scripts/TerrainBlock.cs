using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class TerrainBlock : MonoBehaviour, IExplodable
    {

        [SerializeField] private SpriteRenderer terrainSpriteRenderer;
        [SerializeField] private PolygonCollider2D polygonCollider;
        private Sprite sprite;

        private readonly List<Dictionary<Vector2Int, Vector2>> paths = new();

        private readonly List<Dictionary<Vector2Int, Color>> coloredPixelsGroups = new();
         
        private Texture2D Texture => sprite.texture;
        private Terrain terrain;

        private readonly int checkPixelStep = 3;
        private readonly int boxSizingStep = 5;

        private float widthUnit => (float)Texture.width / (float)Terrain.PIXELS_PER_UNIT;
        private float heightUnit => (float)Texture.height / (float)Terrain.PIXELS_PER_UNIT;

        private float hypUnit = 1f;

        private List<Box> boxes = new List<Box>();

        private void OnDrawGizmos()
        {
            if (gameObject.name == "Block16")
            {
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = Color.red;
                foreach (var box in boxes)
                {

                    Gizmos.DrawLine(WorldPositionToLocal(box.LeftDown + (Vector2)transform.position), WorldPositionToLocal(box.LeftUp + (Vector2)transform.position));
                    Gizmos.DrawLine(WorldPositionToLocal(box.LeftUp + (Vector2)transform.position), WorldPositionToLocal(box.RigthUp + (Vector2)transform.position));
                    Gizmos.DrawLine(WorldPositionToLocal(box.RigthUp + (Vector2)transform.position), WorldPositionToLocal(box.RightDown + (Vector2)transform.position));
                    Gizmos.DrawLine(WorldPositionToLocal(box.RightDown + (Vector2)transform.position), WorldPositionToLocal(box.LeftDown + (Vector2)transform.position));

                }
            }         
        }

        public void OnExplosion(Vector2 pointOfExplosion, Weapon source)
        {
            DestroyTerrain(pointOfExplosion, (int)(source.ExplosionRadius * Terrain.PIXELS_PER_UNIT / transform.lossyScale.x));
        }

        private void DestroyTerrain(Vector2 worldPoint, int radius)
        {
            Vector2 localPoint = WorldPositionToLocal(worldPoint);
            Vector2 uv = LocalPositionToUV(localPoint);
            if (localPoint.magnitude > hypUnit)
            {
                return;
            }
            Vector2Int pixel = UVToPixelPoint(uv);
            List<Vector2Int> pixels = GetCirclePixels(pixel, radius);
            Color[] pixelData = Texture.GetPixels();
            foreach (Vector2Int pixelInCircle in pixels)
            {
                pixelData[pixelInCircle.Vector2IntToArrayIndex(Texture.width)] = Color.clear;
            }
            Texture.SetPixels(pixelData);
            Texture.Apply();

            paths.Clear();
            coloredPixelsGroups.Clear();
            UpdateCollider();           
        }

        private List<Vector2Int> GetCirclePixels(Vector2Int pixel, int radius)
        {
            List<Vector2Int> pixels = new(Texture.width * Texture.height);

            int xMaxIndex = pixel.x + radius - 1 < Texture.width ? pixel.x + radius - 1 : Texture.width - 1;
            int xMinIndex = pixel.x - radius + 1 >= 0 ? pixel.x - radius + 1 : 0;
            int yMaxIndex = pixel.y + radius + 1 < Texture.height ? pixel.y + radius - 1 : Texture.height - 1;
            int yMinIndex = pixel.y - radius - 1 >= 0 ? pixel.y - radius + 1 : 0;

            int i = 0;

            for (int x = xMinIndex; x <= xMaxIndex; x++)
            {
                for (int y = yMinIndex; y <= yMaxIndex; y++)
                {
                    if ((new Vector2Int(x, y) - pixel).magnitude <= radius)
                    {
                        pixels.Add(new Vector2Int(x, y));
                        i++;
                    }
                }
            }
            return pixels;
        }

        private Vector2Int UVToPixelPoint(Vector2 uv)
        {
            return new((int)(Texture.width * uv.x), (int)(Texture.height * uv.y));
        }

        private Vector2 WorldPositionToLocal(Vector2 worldPosition)
        {
            var matrix = transform.worldToLocalMatrix;
            return matrix.MultiplyPoint3x4(worldPosition);
        }

        private Vector2 LocalPositionToUV(Vector2 position)
        {
            float width = Texture.width;
            float height = Texture.height;
            float widthInUnit = width / Terrain.PIXELS_PER_UNIT;
            float heightInUnit = height / Terrain.PIXELS_PER_UNIT;
            position = new Vector2(position.x / widthInUnit, position.y / heightInUnit);
            return position;
        }

        public void SetTexture(Texture2D texture, Vector2 size, Terrain terrain)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size.x, size.y), default);
            terrainSpriteRenderer.sprite = sprite;
            this.sprite = sprite;
            this.terrain = terrain;
            UpdateCollider();
            hypUnit = size.magnitude / Terrain.PIXELS_PER_UNIT;
        }

        private void UpdateCollider()
        {
            List<List<Vector2Int>> paths = new();
            boxes.Clear();

            for (int x = 0; x < Texture.width - checkPixelStep; x += checkPixelStep)
            {
                for (int y = 0; y < Texture.height - checkPixelStep; y += checkPixelStep)
                {
                    Color pixel = Texture.GetPixel(x,y);
                    Vector2Int pixelPos = new(x, y);
                    if (!IsPixelAddedToGroup(pixelPos) && pixel != Color.clear)
                    {
                        List<Vector2Int> addedPixels = CreateBoxPolygon(pixelPos, out var box);

                        if (addedPixels.Count > 1)
                        {
                            List<Vector2Int> path = new List<Vector2Int>()
                            {
                                box.LeftDown,
                                box.LeftUp,
                                box.RigthUp,
                                box.RightDown
                            };
                            boxes.Add(box);

                            paths.Add(path);

                            Dictionary<Vector2Int, Color> group = new();
                            foreach (var addedPixel in addedPixels)
                            {
                                group[addedPixel] = Texture.GetPixel(addedPixel);
                            }
                            coloredPixelsGroups.Add(group);
                        }                     
                    }
                }
            }
            if (paths.Count > 0)
            {
                SetCollider(paths);
            }
            else
            {
                Destroy(gameObject);
            }           
        }

        private bool IsPixelAddedToGroup(Vector2Int pixel)
        {
            return coloredPixelsGroups.Any(g => g.ContainsKey(pixel));
        }

        private List<Vector2Int> CreateBoxPolygon(Vector2Int pos, out Box box)
        {
            List<Vector2Int> pixels = new(Texture.width * Texture.height);
            box = new();
            box.LeftDown = pos + new Vector2Int(0,-1);
            pixels.InsertRange(0, GetPixelsRow(pos, out Vector2Int rightPosition));
            box.RightDown = rightPosition + new Vector2Int(0, -1);
            
            if (pixels.Count == 1)
            {
                return pixels;
            }

            Vector2Int rightBound = rightPosition;

            Vector2Int currentStartPos = pos + new Vector2Int(0, 1);
            List<Vector2Int> row = GetPixelsRow(pos + Vector2Int.up, out rightPosition);
            while (rightPosition.x == rightBound.x && currentStartPos.y < Texture.height)
            {
                pixels.InsertRange(pixels.Count - 1, row);
                row = GetPixelsRow(currentStartPos, out rightPosition);
                currentStartPos += new Vector2Int(0, boxSizingStep);
            }

            box.LeftUp = new Vector2Int(pos.x, rightPosition.y - 1);
            box.RigthUp = new Vector2Int(rightBound.x, rightPosition.y - 1);
          
            return pixels;
        }

        private List<Vector2Int> GetPixelsRow(Vector2Int pos, out Vector2Int rightPosition)
        {
            List<Vector2Int> row = new(Texture.width);
            Vector2Int currentPos = pos;
            rightPosition = pos;
            row.Add(pos);
            for (int x = pos.x + boxSizingStep; x < Texture.width; x++)
            {
                Vector2Int newPos = currentPos + Vector2Int.right;
                rightPosition = currentPos;
                if (!IsTransparentOrOutOfBounds(newPos))
                {
                    row.Add(currentPos);
                    currentPos = newPos;
                }
            }
            return row;
        }

        private struct Box
        {
           public Vector2Int LeftDown, LeftUp, RigthUp, RightDown; 
        }
    
        private void SetCollider(List<List<Vector2Int>> paths)
        {
            polygonCollider.pathCount = paths.Count;
            for (int i = 0; i < paths.Count; i++)
            {
                List<Vector2> pointPath = new();
                foreach (var pathPoint in paths[i])
                {
                    pointPath.Add(PixelToLocalPoint(pathPoint));
                }            
                polygonCollider.SetPath(i, pointPath);            
            }
        }

        private bool IsTransparentOrOutOfBounds(Vector2Int pixel)
        {
            return pixel.IsOutOfBounds(Texture) || Texture.GetPixel(pixel) == Color.clear;
        }

        private Vector2 PixelToLocalPoint(Vector2Int pixel)
        {
            float xUV = (float)pixel.x / Texture.width;
            float yUV = (float)pixel.y / Texture.height;

            Vector2 localPoint = new((xUV) * widthUnit, (yUV) * heightUnit);

            return localPoint;
        }       
    }
}
