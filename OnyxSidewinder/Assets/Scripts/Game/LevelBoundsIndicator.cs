using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LevelBoundsIndicator : MonoBehaviour
{
    [Range(0.0f, 50.0f)]
    public float Width;

    [Range(0.0f, 200.0f)]
    public float Height;

    private LineRenderer _line;
    private Color _startColor = Color.green;
    private Color _endColor = Color.green;
    private float _bevel = 0.3f;

	void Start (){}

    public void Initialize()
    {
        _line = gameObject.GetComponent<LineRenderer>();
        if (_line == null)
        {
            _line = gameObject.AddComponent<LineRenderer>();
        }

        _line.material = new Material(Shader.Find("Particles/Additive"));
        _line.SetColors(_startColor, _endColor);
        _line.SetWidth(0.1F, 0.1F);
        _line.SetVertexCount(9);
        _line.useWorldSpace = true;
        Width = 10.0f;
        Height = 20.0f;
    }

    public void Terminate()
    {
        _line = null;
        //Game.Instance.CurrentLevel.SetBounds(new Rect(0, Height * 0.5f, Width, Height));
        Destroy(this);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isPlaying) { return; }
        if (_line == null) { Initialize(); }
        Vector2 min = new Vector2(-Width * 0.5f, 0f);
        Vector2 max = new Vector2(Width * 0.5f, Height);

        _line.SetPosition(0, new Vector3(min.x, min.y+_bevel, 0));
        _line.SetPosition(1, new Vector3(min.x, max.y-_bevel, 0));
        _line.SetPosition(2, new Vector3(min.x+_bevel, max.y, 0));
        _line.SetPosition(3, new Vector3(max.x-_bevel, max.y, 0));
        _line.SetPosition(4, new Vector3(max.x, max.y-_bevel, 0));
        _line.SetPosition(5, new Vector3(max.x, min.y+_bevel, 0));
        _line.SetPosition(6, new Vector3(max.x-_bevel, min.y, 0));
        _line.SetPosition(7, new Vector3(min.x+_bevel, min.y, 0));
        _line.SetPosition(8, new Vector3(min.x, min.y+_bevel, 0));
    }
}
