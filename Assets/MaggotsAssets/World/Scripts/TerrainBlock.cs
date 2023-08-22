using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class TerrainBlock : MonoBehaviour
    {

        [SerializeField] private SpriteRenderer terrainSpriteRenderer;
        [SerializeField] private PolygonCollider2D polygonCollider;
        private Sprite sprite;

        private readonly List<Dictionary<Vector2Int, Vector2>> paths = new();

        private readonly List<Dictionary<Vector2Int, Color>> coloredPixelsGroups = new();
         
        private Texture2D Texture => sprite.texture;
        private Terrain terrain;

        private readonly int step = 5;

        private float widthUnit => (float)Texture.width / (float)Terrain.PIXELS_PER_UNIT;
        private float heightUnit => (float)Texture.height / (float)Terrain.PIXELS_PER_UNIT;

        private float hypUnit = 1f;

        public void DestroyTerrain(Vector2 worldPoint, int radius)
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
            SetCollider();           
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
            SetCollider();
            hypUnit = size.magnitude / Terrain.PIXELS_PER_UNIT;

            foreach (var pixelsGroup in coloredPixelsGroups)
            {
                foreach (var pixelInGroup in pixelsGroup)
                {
                    Texture.SetPixel(pixelInGroup.Key.x, pixelInGroup.Key.y, Color.red);
                }
            }
            Texture.Apply();
        }

        private void SetCollider()
        {
            for (int x = 0; x < Texture.width - step; x += step)
            {
                for (int y = 0; y < Texture.height - step; y += step)
                {
                    Color pixel = Texture.GetPixel(x,y);
                    Vector2Int pixelPos = new(x, y);
                    if (!IsPixelAddedToGroup(pixelPos) && pixel != Color.clear)
                    {                        
                        coloredPixelsGroups.Add(GetColorerPixelGroup(pixelPos));
                    }
                }
            }

            if (!coloredPixelsGroups.Any())
            {
                Destroy(gameObject);
            }
            else
            {
                int i = 0;
                
                foreach (Dictionary<Vector2Int, Color> colorPixel in coloredPixelsGroups)
                {
                    SetColliderByPixels(colorPixel.First().Key, i);
                    i++;
                }
                RefreshCollider();
            }          
        }

        private bool IsSimilar(Dictionary<Vector2Int, Vector2> p1, Dictionary<Vector2Int, Vector2> p2)
        {
            foreach (var point in p1)
            {
                if (p2.ContainsKey(point.Key))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPixelAddedToGroup(Vector2Int pixel)
        {
            return coloredPixelsGroups.Any(g => g.ContainsKey(pixel));
        }

        private Dictionary<Vector2Int, Color> GetColorerPixelGroup(Vector2Int startPos)
        {
            Queue<Vector2Int> pixelsToReplace = new();

            pixelsToReplace.Enqueue(startPos);

            Dictionary<Vector2Int, Color> pixelsGroup = new();

            while (pixelsToReplace.Count > 0)
            {
                Vector2Int pixel = pixelsToReplace.Dequeue();
                if (!IsTransparentOrOutOfBounds(pixel, pixelsGroup))
                {
                    Vector2Int west = pixel + new Vector2Int(-step, 0); 
                    Vector2Int east = pixel;
                    while (!IsTransparentOrOutOfBounds(west, pixelsGroup))
                    {
                        pixelsGroup[west] = Texture.GetPixel(west);
                        west += new Vector2Int(-step, 0);
                        CheckNorthAndSouth(west, pixelsToReplace, pixelsGroup);
                    }
                    while (!IsTransparentOrOutOfBounds(east, pixelsGroup))
                    {
                        pixelsGroup[east] = Texture.GetPixel(east);
                        east += new Vector2Int(step, 0);
                        CheckNorthAndSouth(east, pixelsToReplace, pixelsGroup);
                    }
                }            
            }
            return pixelsGroup;
        }

        private void CheckNorthAndSouth(Vector2Int start, Queue<Vector2Int> queue, Dictionary<Vector2Int, Color> addedPixels)
        {
            Vector2Int north = start + new Vector2Int(0, step);
            if (!IsTransparentOrOutOfBounds(north, addedPixels))
            {
                queue.Enqueue(north);
            }
            Vector2Int south = start + new Vector2Int(0, -step);
            if (!IsTransparentOrOutOfBounds(south, addedPixels))
            {
                queue.Enqueue(south);
            }
        }      

        private void RefreshCollider()
        {
            polygonCollider.pathCount = paths.Count;
            int i = 0;
            while (i < paths.Count - 1)
            {
                if (IsSimilar(paths[i], paths[i + 1]))
                {
                    paths.Remove(paths[i + 1]);
                }
                i++;
            }

            for (i = 0; i < paths.Count; i++)
            {
                List<Vector2> pointPath = new();
                foreach (var path in paths[i])
                {
                    pointPath.Add(path.Value);
                }
                polygonCollider.SetPath(i, pointPath);            
            }
        }

        private bool IsTransparentOrOutOfBounds(Vector2Int pixel)
        {
            return pixel.IsOutOfBounds(Texture) || Texture.GetPixel(pixel) == Color.clear;
        }

        private bool IsTransparentOrOutOfBounds(Vector2Int pixel, Dictionary<Vector2Int, Color> exceptions)
        {
            return pixel.IsOutOfBounds(Texture) || Texture.GetPixel(pixel) == Color.clear || exceptions.ContainsKey(pixel);
        }

        private Vector2Int[] GetNeighbors(Vector2Int pixel)
        {
            Vector2Int north = new(pixel.x, pixel.y + 1);
            Vector2Int east = new(pixel.x + 1, pixel.y);
            Vector2Int south = new(pixel.x, pixel.y - 1);
            Vector2Int west = new(pixel.x - 1, pixel.y);

            return new Vector2Int[] { north, east, south, west };
        }

        private void SetColliderByPixels(Vector2Int shapePixel, int pathIndex = 0)
        {
            Vector2Int borderPixel = FindBoundPixel(shapePixel);
            Vector2Int v1, v2, v3;
            Vector2Int p1, p2;
            List<Vector2Int> borderPixels = new((int)(Terrain.PIXELS_PER_UNIT * widthUnit * 2 + Terrain.PIXELS_PER_UNIT * heightUnit * 2));
            if (TryGetPairTransparentNeighborPixel(borderPixel, out p1, out p2, out v1, out v2))
            {
                borderPixels.Add(p1);
                borderPixels.Add(p2);
                Vector2Int startPixel = p1;
                v3 = v1;
                while (p2 != startPixel)
                {
                    Vector2Int p3 = p2 + v2;
                    if (IsTransparentOrOutOfBounds(p3))
                    {
                        p1 = p2;
                        p2 = p3;
                        Vector2Int v4 = v2;
                        v3 = v1;
                        v2 = -v1;
                        v1 = v4;
                        borderPixels.Add(p2);
                    }
                    else
                    {
                        p3 = p2 + v1;
                        if (IsTransparentOrOutOfBounds(p3))
                        {
                            p1 = p2;
                            p2 += v1;
                        }
                        else
                        {
                            p1 = p2;
                            TryGetPairTransparentNeighborPixel(p3, p1, out p2, out v1, out v2, v3);
                            borderPixels.Add(p2);

                        }
                    }
                }
            }

            paths.Insert(pathIndex, GetColliderPath(borderPixels));
        }

        private Dictionary<Vector2Int, Vector2> GetColliderPath(List<Vector2Int> pixels)
        {
            Dictionary<Vector2Int, Vector2> path =new();
            float minDistanceBetweenColliderPoints = 4f;

            float step = 1.41421356237f * minDistanceBetweenColliderPoints;

            step *= step;

            foreach (Vector2Int pixel in pixels)
            {
                if (path.Count == 0 || (path.Last().Key - pixel).sqrMagnitude > step)
                {
                    path[pixel] = PixelToLocalPoint(pixel);
                }
            }
            return path;
        }

        private Vector2Int FindBoundPixel(Vector2Int startPixel)
        {
            Vector2Int move = new Vector2Int(1, 0);
            Vector2Int nextPixel = startPixel + move;
            while (!IsTransparentOrOutOfBounds(nextPixel))
            {
                nextPixel += move;
            }
            return nextPixel - move;
        }


        private bool TryGetPairTransparentNeighborPixel(Vector2Int borderPixel, out Vector2Int transparentPixel1, out Vector2Int transparentPixel2, out Vector2Int directionByBorder, out Vector2Int directionTowardBorder, Vector2Int oldDirection = default)
        {
            Vector2Int[] directions = GetNeighbors(borderPixel);
            for (int i = 0; i < directions.Length; i++)
            {
                if (IsTransparentOrOutOfBounds(directions[i]))
                {
                    if (TryGetPairTransparentNeighborPixel(borderPixel, directions[i], out transparentPixel2, out directionByBorder, out directionTowardBorder, oldDirection))
                    {
                        transparentPixel1 = directions[i];
                        return true;
                    }
                }
            }
            transparentPixel1 = default;
            transparentPixel2 = default;
            directionTowardBorder = default;
            directionByBorder = default;
            return false;
        }

        private bool TryGetPairTransparentNeighborPixel(Vector2Int borderPixel, Vector2Int startTransparentPixel1, out Vector2Int transparentPixel2, out Vector2Int directionByBorder, out Vector2Int directionTowardBorder, Vector2Int oldDirection = default)
        {
            directionTowardBorder = borderPixel - startTransparentPixel1;
            Vector2Int directionToNeighborPixel = directionTowardBorder.Rotate90Degree(false);
            if (IsTransparentOrOutOfBounds(startTransparentPixel1 + directionToNeighborPixel))
            {
                transparentPixel2 = startTransparentPixel1 + directionToNeighborPixel;
                directionByBorder = directionToNeighborPixel;
                return true;
            }

            directionToNeighborPixel = directionTowardBorder.Rotate90Degree(true);
            if (IsTransparentOrOutOfBounds(startTransparentPixel1 + directionToNeighborPixel))
            {
                transparentPixel2 = startTransparentPixel1 + directionToNeighborPixel;
                directionByBorder = directionToNeighborPixel;
                return true;
            }

            if (IsTransparentOrOutOfBounds(startTransparentPixel1 - directionTowardBorder))
            {
                transparentPixel2 = startTransparentPixel1 - directionTowardBorder;
                directionByBorder = -directionTowardBorder;

                if (oldDirection != default)
                {
                    directionTowardBorder = oldDirection;
                }
                else
                {
                    directionTowardBorder = directionByBorder.Rotate90Degree(true);
                }
                return true;
            }

            transparentPixel2 = default;
            directionTowardBorder = default;
            directionByBorder = default;
            return false;
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
