using System.Collections.Generic;
using UnityEngine;


public class FluidManager : MonoBehaviour
{
    private List<LineRenderer> _lineRenderers;
    private List<Transform> _dropletTransforms;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private int lineSubdivisions;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Gradient gradient;

    private (float, float) _sizes;
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _dropletTransforms = new List<Transform>();
        _lineRenderers = new List<LineRenderer>();
    }

    public void AddDroplet(Droplet droplet)
    {
        Debug.Log("Added Droplet");
        _dropletTransforms.Add(droplet.gameObject.transform);
        
        GameObject fluidObject = new GameObject("Fluid Line");
        fluidObject.transform.parent = this.gameObject.transform;
        
        LineRenderer lineRenderer = fluidObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.numCornerVertices = 0;
        lineRenderer.numCapVertices = 0;
        lineRenderer.positionCount = lineSubdivisions;
        lineRenderer.material = lineMaterial;
        lineRenderer.colorGradient = gradient;
        
        _lineRenderers.Add(lineRenderer);
    }

    public void RemoveDroplet(Droplet droplet)
    {
        Debug.Log("Removed Droplet");
        GameObject fluidObject = _lineRenderers[^1].gameObject;
        _dropletTransforms.Remove(droplet.gameObject.transform);
        _lineRenderers.RemoveAt(_lineRenderers.Count - 1);
        Destroy(fluidObject);
    }

    private void Update()
    {
        if (_dropletTransforms.Count < 1) return;
        
        for (int i = 0; i < _dropletTransforms.Count; i++)
        {
            float distance = Vector3.Distance(playerTransform.position, _dropletTransforms[i].position);
            _sizes = CalculateSizes(i);
            
            if (ShouldDisableLineRenderer(distance))
            {
                _lineRenderers[i].enabled = false;
                return;
            }

            _lineRenderers[i].enabled = true;
            _lineRenderers[i].positionCount = lineSubdivisions;

            UpdateLineRendererPositions(i);
            UpdateLineRendererWidth(distance, i);
        }
    }

    private bool ShouldDisableLineRenderer(float distance)
    {
        float playerSize = _sizes.Item1;
        float dropletSize = _sizes.Item2;

        bool isRendered = distance < Mathf.Min(playerSize, dropletSize) / 2f || CalculateMidpointWidth(distance, playerSize, dropletSize) < 0.01f;
        return isRendered;
    }

    private (float, float) CalculateSizes(int i)
    {
        Vector3 direction = (_dropletTransforms[i].position - playerTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle;

        float playerSize = Mathf.Lerp(playerTransform.localScale.x, playerTransform.localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));
        float dropletSize = Mathf.Lerp(_dropletTransforms[i].localScale.x, _dropletTransforms[i].localScale.y, Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)));

        return (playerSize, dropletSize);
    }

    private void UpdateLineRendererPositions(int i)
    {
        for (int j = 0; j < lineSubdivisions; j++)
        {
            float t = (float)j / (lineSubdivisions - 1);

            Vector3 point = Vector3.Lerp(playerTransform.position, _dropletTransforms[i].position, t);

            _lineRenderers[i].SetPosition(j, point);
        }
    }


    private void UpdateLineRendererWidth(float distance, int i)
    {
        float playerSize = _sizes.Item1;
        float dropletSize = _sizes.Item2;

        float midpointWidth = CalculateMidpointWidth(distance, playerSize, dropletSize);
        float totalSize = playerSize + dropletSize;
        float midpointTime = playerSize / totalSize;

        AnimationCurve widthCurve = new AnimationCurve();
        Keyframe key0 = new Keyframe(0f, playerSize);
        Keyframe keyMid = new Keyframe(midpointTime, midpointWidth);
        Keyframe key1 = new Keyframe(1f, dropletSize);

        SetFlatTangents(key0);
        SetFlatTangents(keyMid);
        SetFlatTangents(key1);

        widthCurve.AddKey(key0);
        widthCurve.AddKey(keyMid);
        widthCurve.AddKey(key1);

        widthCurve.MoveKey(0, key0);
        widthCurve.MoveKey(1, keyMid);
        widthCurve.MoveKey(2, key1);

        _lineRenderers[i].widthCurve = widthCurve;
    }

    private float CalculateMidpointWidth(float distance, float playerSize, float dropletSize)
    {
        return Mathf.Min((playerSize + dropletSize) / (2 * Mathf.Pow(distance, 2f)), Mathf.Min(playerSize, dropletSize));
    }

    private void SetFlatTangents(Keyframe key)
    {
        key.inTangent = 0f;
        key.outTangent = 0f;
    }
}
