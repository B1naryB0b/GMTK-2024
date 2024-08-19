using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FluidManager : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<Transform> _ballTransforms;

    [SerializeField] private int lineSubdivisions;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        CollectBallTransforms();
        UpdateLineRendererLoop();
    }

    private void CollectBallTransforms()
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        _ballTransforms = new List<Transform>();

        foreach (var t in allTransforms)
        {
            if (t != transform)
            {
                _ballTransforms.Add(t);
            }
        }
    }

    private void UpdateLineRendererLoop()
    {
        _lineRenderer.loop = _ballTransforms.Count > 2;
    }

    private void Update()
    {
        if (_ballTransforms == null || _ballTransforms.Count < 2)
        {
            Initialize();
            if (_ballTransforms.Count < 2) return;
        }

        float distance = Vector3.Distance(_ballTransforms[0].position, _ballTransforms[1].position);

        if (ShouldDisableLineRenderer(distance))
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = lineSubdivisions;

        UpdateLineRendererPositions();
        UpdateLineRendererWidth(distance);
    }

    private bool ShouldDisableLineRenderer(float distance)
    {
        var sizes = CalculateSizes();
        float size0 = sizes.Item1;
        float size1 = sizes.Item2;

        return distance < Mathf.Min(size0, size1) / 2f || CalculateMidpointWidth(distance, size0, size1) < 0.1f;
    }

    private (float, float) CalculateSizes()
    {
        Vector3 direction = (_ballTransforms[1].position - _ballTransforms[0].position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle;

        float size0 = Mathf.Lerp(_ballTransforms[0].localScale.x, _ballTransforms[0].localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));
        float size1 = Mathf.Lerp(_ballTransforms[1].localScale.x, _ballTransforms[1].localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));

        return (size0, size1);
    }

    private void UpdateLineRendererPositions()
    {
        int numBalls = _ballTransforms.Count;
        for (int i = 0; i < lineSubdivisions; i++)
        {
            float t = (float)i / (lineSubdivisions - 1);
            int startIndex = Mathf.FloorToInt(t * (numBalls - 1));
            int endIndex = (startIndex + 1) % numBalls;

            float segmentT = (t * (numBalls - 1)) - startIndex;
            Vector3 point = Vector3.Lerp(_ballTransforms[startIndex].position, _ballTransforms[endIndex].position, segmentT);
            _lineRenderer.SetPosition(i, point);
        }
    }

    private void UpdateLineRendererWidth(float distance)
    {
        var sizes = CalculateSizes();
        float size0 = sizes.Item1;
        float size1 = sizes.Item2;

        float midpointWidth = CalculateMidpointWidth(distance, size0, size1);
        float totalSize = size0 + size1;
        float midpointTime = size0 / totalSize;

        AnimationCurve widthCurve = new AnimationCurve();
        Keyframe key0 = new Keyframe(0f, size0);
        Keyframe keyMid = new Keyframe(midpointTime, midpointWidth);
        Keyframe key1 = new Keyframe(1f, size1);

        SetFlatTangents(key0);
        SetFlatTangents(keyMid);
        SetFlatTangents(key1);

        widthCurve.AddKey(key0);
        widthCurve.AddKey(keyMid);
        widthCurve.AddKey(key1);

        widthCurve.MoveKey(0, key0);
        widthCurve.MoveKey(1, keyMid);
        widthCurve.MoveKey(2, key1);

        _lineRenderer.widthCurve = widthCurve;
    }

    private float CalculateMidpointWidth(float distance, float size0, float size1)
    {
        return Mathf.Min((size0 + size1) / (2 * Mathf.Pow(distance, 2f)), Mathf.Min(size0, size1));
    }

    private void SetFlatTangents(Keyframe key)
    {
        key.inTangent = 0f;
        key.outTangent = 0f;
    }
}
