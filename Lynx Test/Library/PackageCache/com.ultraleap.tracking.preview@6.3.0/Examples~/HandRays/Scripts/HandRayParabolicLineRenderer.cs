/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2022.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using Leap.Unity.Preview.HandRays;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity
{
    public class HandRayParabolicLineRenderer : HandRayRenderer
    {
        [Tooltip("The scale of the projection of any hand distance from the approximated "
               + "shoulder beyond the handMergeDistance.")]
        [Range(0f, 35f)]
        public float projectionScale = 10f;

        [Tooltip("The distance from the approximated shoulder beyond which any additional "
               + "distance is exponentiated by the projectionExponent.")]
        [Range(0f, 1f)]
        public float handMergeDistance = 0.7f;

        private List<Vector3> _parabolaPositions = new List<Vector3>();

        Vector3 evaluateParabola(Vector3 position, Vector3 velocity, Vector3 acceleration, float time)
        {
            return position + (velocity * time) + (0.5f * acceleration * (time * time));
        }

        protected override bool UpdateLineRendererLogic(HandRayDirection handRayDirection, out RaycastHit raycastResult)
        {
            _parabolaPositions.Clear();

            // Calculate the projection of the hand if it extends beyond the handMergeDistance.
            Vector3 handProjection = handRayDirection.Direction;
            float handShoulderDist = handProjection.magnitude;
            float projectionDistance = Mathf.Max(0.0f, handShoulderDist - handMergeDistance);
            float projectionAmount = (projectionDistance + 0.15f) * projectionScale;

            raycastResult = new RaycastHit();
            bool hit = false;
            if (projectionDistance > 0f)
            {
                Vector3 startPos = handRayDirection.AimPosition;
                _parabolaPositions.Add(handRayDirection.VisualAimPosition);
                Vector3 velocity = handProjection * projectionAmount;
                for (float i = 0; i < 8f; i += 0.1f)
                {
                    Vector3 segmentStart = evaluateParabola(startPos, velocity, Physics.gravity * 0.25f, i);
                    Vector3 segmentEnd = evaluateParabola(startPos, velocity, Physics.gravity * 0.25f, i + 0.1f);
                    _parabolaPositions.Add(segmentEnd);
                    if (Physics.Raycast(new Ray(segmentStart, segmentEnd - segmentStart), out raycastResult, Vector3.Distance(segmentStart, segmentEnd), _layerMask))
                    {
                        hit = true;
                        _parabolaPositions.Add(raycastResult.point);
                    }

                    if (hit) { break; }
                }
            }

            UpdateLineRendererPositions(_parabolaPositions.Count, _parabolaPositions.ToArray(), hit);
            return hit;
        }
    }
}