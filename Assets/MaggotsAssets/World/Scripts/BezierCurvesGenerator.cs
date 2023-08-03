using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Maggots
{
    public class BezierCurvesGenerator : MonoBehaviour
    {
        [SerializeField] private float maxXUnitPerCurve;
        [SerializeField] private float maxYUnitPerCurve;
        [SerializeField] private Vector2 xRange;
        [SerializeField] private Vector2 yRange;
        [SerializeField] private Vector2Int curveCountRange;
        [SerializeField] GameObject pointSpritePrefab;
        [SerializeField] private Vector2 clampX;
        [SerializeField] private Vector2 clampY;

        [SerializeField] Terrain terrain;

        public int seed = 544;

        public Vector2 XBorders
        {
            get;
            private set;
        }
        public Vector2 YBorders
        {
            get;
            private set;
        }

        private BezierCurve2D[] curves;


        private int shapeIndex = 0;

        private void OnValidate()
        {
            Generate();
            terrain.Draw();
        }

        public void OnDrawGizmos()
        {
            if (curves == null)
            {
                Generate();
            }
            else
            {
                Render(curves);
            }
        }
        [ContextMenu("Generate")]
        public BezierCurve2D[] Generate()
        {
            Random.InitState(seed);
           

            BezierCurve2D[] curves = new BezierCurve2D[Random.Range(curveCountRange.x, curveCountRange.y + 1)];
            Vector2 enterPoint = transform.position;
            Matrix4x4 matrix = Matrix4x4.identity;
            for (int i = 0; i < curves.Length; i++)
            {
                float curveMidXPoint = Random.Range(0.2f, 0.8f);
                float secondPointX = curveMidXPoint - Random.Range(0.01f, 0.5f);
                float thirdPointX = curveMidXPoint + Random.Range(0.01f, 0.5f);
                float curveLength = Random.Range(xRange.x * maxXUnitPerCurve, xRange.y * maxXUnitPerCurve);
                float curveHeight = Random.Range(yRange.x * maxYUnitPerCurve, yRange.y * maxYUnitPerCurve);
                Vector2 secondPoint = matrix.MultiplyPoint3x4(new(enterPoint.x + curveLength * secondPointX, enterPoint.y + curveHeight));
                Vector2 thirdPoint = matrix.MultiplyPoint3x4(new(enterPoint.x + curveLength * thirdPointX, enterPoint.y + curveHeight));

                Vector3 exitPoint = new(enterPoint.x + curveLength, enterPoint.y);
                Vector2 transformedExitPoint = matrix.MultiplyPoint3x4(exitPoint);
                if (i == curves.Length - 1)
                {
                    transformedExitPoint = curves[0].Start;
                }
                curves[i] = new BezierCurve2D(AlignPoint(matrix.MultiplyPoint3x4(enterPoint)), AlignPoint(secondPoint), AlignPoint(thirdPoint), AlignPoint(transformedExitPoint), clampX, clampY);
                enterPoint = exitPoint;
                float startXPosition = enterPoint.x;
                matrix = GetMatrix((float)i / (float)curves.Length, matrix, enterPoint, ref startXPosition);

                //GameObject p1 = Instantiate(pointSpritePrefab);
                //p1.name = $"C{i}-P1";
                //p1.transform.position = curves[i].point1;
                //p1.transform.parent = transform;

                //GameObject p2 = Instantiate(pointSpritePrefab);
                //p2.name = $"C{i}-P2";
                //p2.transform.position = curves[i].point2;
                //p2.transform.parent = transform;

                //GameObject p3 = Instantiate(pointSpritePrefab);
                //p3.name = $"C{i}-P3";
                //p3.transform.position = curves[i].point3;
                //p3.transform.parent = transform;

                //GameObject p4 = Instantiate(pointSpritePrefab);
                //p4.name = $"C{i}-P4";
                //p4.transform.position = curves[i].point4;
                //p4.transform.parent = transform;

                enterPoint.x = startXPosition;
            }

            float LeftBorder = curves.ToList().Select(v => v.LeftPoint).ToArray().SelectFarthestVector(VectorExtensions.LeftVector()).x;
            float RightBorder = curves.ToList().Select(v => v.RightPoint).ToArray().SelectFarthestVector(VectorExtensions.RightVector()).x;
            float UpBorder = curves.ToList().Select(v => v.UpPoint).ToArray().SelectFarthestVector(VectorExtensions.UpVector()).y;
            float DownBorder = curves.ToList().Select(v => v.DownPoint).ToArray().SelectFarthestVector(VectorExtensions.DownVector()).y;

            XBorders = new Vector2(LeftBorder, RightBorder);
            YBorders = new Vector2(DownBorder, UpBorder);

            for (int i = 0; i < curves.Length; i++)
            {
                curves[i].SetBorders(XBorders, YBorders);
            }
            
            this.curves = curves;
            return curves;
        }

        private Vector2 AlignPoint(Vector2 globalSpacePoint)
        {
            Vector2 clamped = new(Mathf.Clamp(globalSpacePoint.x, clampX.x, clampX.y), Mathf.Clamp(globalSpacePoint.y, clampY.x, clampY.y));          
            return clamped;
        }

        private Matrix4x4 GetMatrix(float progress, Matrix4x4 currentMatrix, Vector2 startPoint, ref float xProgress)
        {
            int currentShapeIndex;
            if (progress > 0.4f && progress < 0.5f)
            {
                currentShapeIndex = 1;
            }
            else if (progress > 0.5f && progress < 0.9f)
            {
                currentShapeIndex = 2;
            }
            else if (progress > 0.9f && progress < 1f)
            {
                currentShapeIndex = 3;
            }
            else
            {
                currentShapeIndex = 0;
            }

            return TranslateRotateMatrix(currentMatrix, currentShapeIndex, startPoint, ref xProgress);
        }

        private Matrix4x4 TranslateRotateMatrix(Matrix4x4 currentMatrix, int currentShapeIndex, Vector2 startPoint, ref float xProgress)
        {
            if (currentShapeIndex != shapeIndex)
            {
                currentMatrix *= Matrix4x4.Translate(transform.position);
                shapeIndex = currentShapeIndex;
                Matrix4x4 matrix = currentMatrix * Matrix4x4.Translate(startPoint);
                switch (currentShapeIndex)
                {
                    case 0:
                        matrix *= Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 0f));
                        break;
                    case 1:
                        matrix *= Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
                        break;
                    case 2:
                        matrix *= Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
                        break;
                    case 3:
                        matrix *= Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, -90f));
                        break;
                }
                xProgress = 0f;
                return matrix;
            }
            return currentMatrix;
        }

        private void Render(BezierCurve2D[] curves)
        {
            Vector2 previousPoint = transform.position;
            foreach (BezierCurve2D curve in curves)
            {
                float t = 0f;
                while (t < 1f)
                {
                    t += 0.1f;
                    Vector2 point = curve.GetPoint(t);
                    Gizmos.DrawLine(previousPoint, point);
                    previousPoint = point;
                }
            }
        }
    }
}

