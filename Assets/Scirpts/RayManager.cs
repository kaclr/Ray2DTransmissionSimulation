using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    private static RayManager _instance;

    public static RayManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private RayManager()
    {
        _instance = this;
    }

    public List<Emiter> Emiters = new List<Emiter>();

    public List<RayTrigger> Triggers = new List<RayTrigger>();

    [Header("==Rays==")]
    public List<LineRay> Rays = new List<LineRay>();

    private Queue<LineRay> _waitRay = new Queue<LineRay>();

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateRay();
    }

    #region 公共方法

    public void AddEmiter(Emiter emiter)
    {
        if (!Emiters.Contains(emiter))
        {
            Emiters.Add(emiter);
        }
    }

    public void RemoveEmiter(Emiter emiter)
    {
        if (Emiters.Contains(emiter))
        {
            Emiters.Remove(emiter);
        }
    }

    public void AddRayTrigger(RayTrigger rt)
    {
        if (!Triggers.Contains(rt))
        {
            Triggers.Add(rt);
        }
    }

    public void RemoveRayTrigger(RayTrigger rt)
    {
        if (Triggers.Contains(rt))
        {
            Triggers.Remove(rt);
        }
    }

    public void EnqueueWaitRay(LineRay[] rays)
    {
        for (int i = 0; i < rays.Length; i++)
        {
            _waitRay.Enqueue(rays[i]);
        }
    }

    #endregion 公共方法

    private void UpdateRay()
    {
        for (int i = 0; i < Rays.Count; i++)
        {
            RayPool.Instance.Disable(Rays[i]);
        }
        Rays.Clear();
        _waitRay.Clear();
        for (int i = 0; i < Emiters.Count; i++)
        {
            EnqueueWaitRay(Emiters[i].Emit());
        }

        while (_waitRay.Count > 0)
        {
            LineRay ray = _waitRay.Dequeue();
            ray.CastTrigger = null;
            for (int i = 0; i < Triggers.Count; i++)
            {
                Triggers[i].FastCast(ray);
            }
            if (ray.CastTrigger != null)
            {
                LineRay[] newRays;
                ray.CastTrigger.Cast(ray, out newRays);
                EnqueueWaitRay(newRays);
            }
            Rays.Add(ray);
        }
    }
}