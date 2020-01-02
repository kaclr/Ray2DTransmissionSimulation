using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emiter : MonoBehaviour
{
    public Color EmitColor;

    public int ReflectionLife;

    public int RefractionLife;

    public LineRay[] Emit()
    {
        LineRay ray = RayPool.Instance.Get();
        ray.RayColor = EmitColor;
        ray.StartPoint = transform.position;
        ray.Angle = transform.eulerAngles.z;
        ray.ReflectionLife = ReflectionLife;
        ray.RefractionLife = RefractionLife;
        return new LineRay[] { ray };
    }

    // Use this for initialization
    private void OnEnable()
    {
        RayManager.Instance.AddEmiter(this);
    }

    // Update is called once per frame
    private void OnDisable()
    {
        RayManager.Instance.RemoveEmiter(this);
    }
}