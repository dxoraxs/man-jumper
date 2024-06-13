using System.Collections.Generic;
using Level;
using Level.ObstaclePatterns;
using UnityEngine;

namespace Helpers
{
    public static class PlayerPathCalculator
    {
        public static Vector3[] Calculate(PatternCubeResult[] path, Vector3 startPosition)
        {
            var index = 0;
            var timer = 0f;

            var newCalculatePath = new List<Vector3> { startPosition };
            while (index < path.Length)
            {
                var currentPoint = path[index];

                switch (currentPoint.Type)
                {
                    case CubeTypes.Turn:
                        var previewIndex = Mathf.Clamp(index - 1, 0, path.Length - 1);
                        var targetIndex = Mathf.Clamp(index + 1, 0, path.Length - 1);
                        var previewPoint = path[previewIndex].Position;
                        var targetPosition = path[targetIndex].Position;

                        while (Vector3.Distance(newCalculatePath[newCalculatePath.Count - 1], targetPosition) > 0.1f)
                        {
                            timer += .1f;
                            var newTransformPosition =
                                BezierCurve.CalculateBezierPoint(previewPoint, currentPoint.Position, targetPosition,
                                    timer);
                            newCalculatePath.Add(newTransformPosition);
                        }

                        timer = 0;
                        index += 2;
                        break;
                    default:
                        newCalculatePath.Add(currentPoint.Position);
                        index++;
                        break;
                }
            }

            return newCalculatePath.ToArray();
        }
    }
}