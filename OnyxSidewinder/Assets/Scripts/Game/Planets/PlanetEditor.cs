using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlanetEditor : MonoBehaviour
{
    // Public Variables
    public GameObject PlanetObject;
    public GameObject GravityObject;
    public PlanetType Type;
    [Range(0.1f, 6.0f)]
    public float PlanetRadius;

    [Range(0.1f, 10.0f)]
    public float GravityDepth;

    // Private Variables
    private PlanetType _type;
    private float _planet;
    private float _gravity;

    // Use this for initialization
    void Start ()
    {
        // TODO: Planet Manager does not exist yet. Need to find another way to register planets.
        //Global.Level.Planets.RegisterPlanet(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnValidate()
    {
        if (Application.isPlaying) { return; }
        if (Type != _type)
        {
            Debug.Log("Type Changed from " + _type + " to " + Type);
            _type = Type;
            SetSprite();
        }

        if (PlanetRadius != _planet) { _planet = PlanetRadius; SetScale(); }
        if (GravityDepth != _gravity) { _gravity = GravityDepth; SetScale(); }
    }

    private void SetSprite()
    {
        PlanetTypeData data = Global.GetPlanetTypeData(_type);
        if (PlanetObject == null) { return; }

        SpriteRenderer sr = PlanetObject.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) { return; }

        sr.sprite = Resources.Load<Sprite>("Sprites/" + data.Sprite);
    }

    public virtual void SetScale()
    {
        PlanetObject.transform.localScale = Vector3.one * _planet * 2.0f;
        GravityObject.transform.localScale = Vector3.one * _gravity * 2.0f;
    }
}
