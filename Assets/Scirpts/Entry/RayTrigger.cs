using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTrigger : MonoBehaviour
{
    public virtual void FastCast(LineRay ray)
    {
    }

    public virtual void Cast(LineRay ray, out LineRay[] outRays)
    {
        outRays = null;
    }

    /// <summary>
    /// 介质参数
    /// </summary>
    [System.Serializable]
    public struct TransmitterArg
    {
        public bool Reflect;

        public Color ReflectAddColor;

        public Color ReflectMultColor;

        public bool Refract;

        public float RefractiveIndex;

        public Color RefractAddColor;

        public Color RefractMultColor;
    }

    private void OnEnable()
    {
        RayManager.Instance.AddRayTrigger(this);
    }

    // Update is called once per frame
    private void OnDisable()
    {
        RayManager.Instance.RemoveRayTrigger(this);
    }
}