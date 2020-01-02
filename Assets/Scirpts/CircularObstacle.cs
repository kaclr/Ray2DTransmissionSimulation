using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularObstacle : RayTrigger
{
    public float Radius;

    [SerializeField]
    public TransmitterArg Transmitter;

    public override void FastCast(LineRay ray)
    {
        float sqrRadius = Radius * Radius;
        Vector3 pos = (ray.StartPoint - transform.position);
        Vector3 dir = ray.transform.right;
        if (Vector3.Dot(pos, pos) >= sqrRadius && Vector3.Dot(pos, dir) >= 0)
        {
            return;
        }

        float a = dir.sqrMagnitude;
        float b = Vector3.Dot(dir, pos) * 2f;
        float c = pos.sqrMagnitude - sqrRadius;
        float deta = b * b - 4f * a * c;
        if (deta < float.Epsilon)
        {
            return;
        }
        else
        {
            float t1 = (-b + Mathf.Sqrt(deta)) / 2 / a;
            float t2 = (-b - Mathf.Sqrt(deta)) / 2 / a;

            if (t1 > 0f && ray.Length > t1)
            {
                ray.Length = t1;
                ray.CastTrigger = this;
            }
            if (t2 > 0f && ray.Length > t2)
            {
                ray.Length = t2;
                ray.CastTrigger = this;
            }
        }
    }

    public override void Cast(LineRay ray, out LineRay[] outRays)
    {
        outRays = null;
        List<LineRay> rays = new List<LineRay>();
        if (Transmitter.Reflect)
        {
            if (ray.ReflectionLife > 0)
            {
                ray.ReflectionLife--;
                Reflect(ray, rays);
            }
        }

        if (Transmitter.Refract)
        {
            if (ray.RefractionLife > 0)
            {
                ray.RefractionLife--;
                Refract(ray, rays);
            }
        }
        outRays = rays.ToArray();
    }

    //反射
    private void Reflect(LineRay ray, List<LineRay> outRays)
    {
        Vector3 outDir = Vector3.Reflect(ray.transform.right, (transform.position - ray.EndPoint).normalized);
        LineRay newRay = RayPool.Instance.Get();
        newRay.Angle = Vector3.SignedAngle(outDir, Vector3.right, Vector3.back);
        newRay.StartPoint = ray.EndPoint + newRay.transform.right * 0.001f;
        newRay.ReflectionLife = ray.ReflectionLife;
        newRay.RefractionLife = ray.RefractionLife;
        newRay.RayColor = ray.RayColor;
        newRay.RayColor *= Transmitter.ReflectMultColor;
        newRay.RayColor += Transmitter.ReflectAddColor;
        outRays.Add(newRay);
    }

    //折射
    private void Refract(LineRay ray, List<LineRay> outRays)
    {
        Vector3 normal = (transform.position - ray.EndPoint).normalized;

        float input = Vector3.Angle(ray.transform.right, normal);
        float signInput = Vector3.SignedAngle(ray.transform.right, normal, Vector3.back);
        if (input >= Mathf.Asin(Transmitter.RefractiveIndex) * Mathf.Rad2Deg)
        {
            return;
        }
        LineRay newRay = RayPool.Instance.Get();

        float output;
        if (Vector3.Dot(ray.transform.right, normal) > 0f)
        {
            output = Mathf.Asin(1f / Transmitter.RefractiveIndex * Mathf.Sin(input * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
            newRay.Angle = (normal).ToAngle() + Mathf.Sign(signInput) * output;
        }
        else
        {
            output = Mathf.Asin(Transmitter.RefractiveIndex * Mathf.Sin(input * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

            newRay.Angle = (-normal).ToAngle() - Mathf.Sign(signInput) * output;
        }

        newRay.StartPoint = ray.EndPoint + newRay.transform.right * 0.001f;
        newRay.ReflectionLife = ray.ReflectionLife;
        newRay.RefractionLife = ray.RefractionLife;
        newRay.RayColor = ray.RayColor;
        newRay.RayColor *= Transmitter.RefractMultColor;
        newRay.RayColor += Transmitter.RefractAddColor;

        outRays.Add(newRay);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;//为随后绘制的gizmos设置颜色。
        Gizmos.DrawWireSphere(transform.position, Radius);//使用center和radius参数，绘制一个线框球体。
    }
}