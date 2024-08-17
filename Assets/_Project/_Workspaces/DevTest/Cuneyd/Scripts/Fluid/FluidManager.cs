using System.Collections;
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

    // This method initializes the LineRenderer and ball transforms
    private void Initialize()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Transform[] allTransforms = GetComponentsInChildren<Transform>();

        _ballTransforms = new List<Transform>();
        foreach (var t in allTransforms)
        {
            if (t != transform)
            {
                _ballTransforms.Add(t);
            }
        }

        if (_ballTransforms.Count > 2)
        {
            _lineRenderer.loop = true;
        }
        else
        {
            _lineRenderer.loop = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_ballTransforms == null || _ballTransforms.Count < 2)
        {
            Initialize();
            if (_ballTransforms.Count < 2) return;
        }

        float distance = Vector3.Distance(_ballTransforms[0].position, _ballTransforms[1].position);

        // Calculate the angle between the balls
        Vector3 direction = (_ballTransforms[1].position - _ballTransforms[0].position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle;

        // Calculate the size based on the angle
        float size0 = Mathf.Lerp(_ballTransforms[0].localScale.x, _ballTransforms[0].localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));
        float size1 = Mathf.Lerp(_ballTransforms[1].localScale.x, _ballTransforms[1].localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));
        
        float totalSize = size0 + size1;
        float midpointTime = size0 / totalSize;

        // Disable the line renderer if the distance is too small
        if (distance < Mathf.Min(size0, size1) / 2f)
        {
            _lineRenderer.enabled = false;
            return;
        }
        else
        {
            _lineRenderer.enabled = true;
        }

        _lineRenderer.positionCount = lineSubdivisions;

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

        // Calculate the width at the midpoint based on the distance and sizes
        float midpointWidth = Mathf.Min((size0 + size1) / (2 * Mathf.Pow(distance, 2f)), Mathf.Min(size0, size1));

        if (midpointWidth < 0.1f) // You can adjust the threshold as needed
        {
            _lineRenderer.enabled = false;
            return;
        }
        
        AnimationCurve widthCurve = new AnimationCurve();
        Keyframe key0 = new Keyframe(0f, size0);
        Keyframe keyMid = new Keyframe(midpointTime, midpointWidth);
        Keyframe key1 = new Keyframe(1f, size1);

        // Set tangents to be flat
        key0.inTangent = 0f;
        key0.outTangent = 0f;
        keyMid.inTangent = 0f;
        keyMid.outTangent = 0f;
        key1.inTangent = 0f;
        key1.outTangent = 0f;

        widthCurve.AddKey(key0);
        widthCurve.AddKey(keyMid);
        widthCurve.AddKey(key1);

        // Move keys to ensure tangents are updated
        widthCurve.MoveKey(0, key0);
        widthCurve.MoveKey(1, keyMid);
        widthCurve.MoveKey(2, key1);

        _lineRenderer.widthCurve = widthCurve;
    }
}
