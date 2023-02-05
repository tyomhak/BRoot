using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] public float scrollSpeedX = 1f;
    [SerializeField] public float scrollSpeedY = 1f;

    LineRenderer lineRenderer;
    Material material;

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        material = lineRenderer.material;
    }


    void Update()
    {
        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];

        lineRenderer.GetPositions(linePositions);

        Vector3 lineVec = (linePositions[linePositions.Length-1] - linePositions[0]).normalized;

        float OffsetX = -(Time.deltaTime * lineVec.x * scrollSpeedX);
        float OffsetY = Time.deltaTime * lineVec.y * scrollSpeedY;

        OffsetY = 0;

        lineRenderer.material.mainTextureOffset += new Vector2(OffsetX, OffsetY);
    }
}
