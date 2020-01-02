using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineController))]
public class LineRay : MonoBehaviour
{
    private readonly float MAX_LENGTH = 20000f;

    public int ReflectionLife;

    public int RefractionLife;

    public Color RayColor
    {
        get
        {
            return _rayColor;
        }
        set
        {
            _rayColor = value;
        }
    }

    private Color _rayColor;

    public SpriteRenderer LightSprite;

    [HideInInspector]
    //最前接触到的触发器
    public RayTrigger CastTrigger;

    public Vector3 StartPoint
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    public float Angle
    {
        get
        {
            return transform.eulerAngles.z;
        }

        set
        {
            transform.eulerAngles = transform.eulerAngles.SetZ(value);
        }
    }

    public float Length
    {
        get
        {
            return _length;
        }

        set
        {
            _length = value;
        }
    }

    public Vector3 EndPoint
    {
        get
        {
            return StartPoint + transform.right * _length;
        }
    }

    private float _length = 50f;

    // Use this for initialization
    private void OnEnable()
    {
        //测试
        _length = MAX_LENGTH;
    }

    private void LateUpdate()
    {
        GetComponent<LineController>().Points[0] = transform;
        GetComponent<LineController>().Points[1] = transform.GetChild(0);

        if (GetComponent<LineRenderer>().startColor != _rayColor)
        {
            GetComponent<LineRenderer>().startColor = _rayColor;
            GetComponent<LineRenderer>().endColor = _rayColor;

            LightSprite.color = _rayColor;
        }
        transform.GetChild(0).localPosition = Vector3.right * _length;
    }
}