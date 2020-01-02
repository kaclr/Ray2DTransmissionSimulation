using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPool : MonoBehaviour
{
    public GameObject Prefab;

    private static RayPool _instance;

    public static RayPool Instance
    {
        get
        {
            return _instance;
        }
    }

    private RayPool()
    {
        _instance = this;
    }

    private List<LineRay> _unactiveList = new List<LineRay>();

    public LineRay Get()
    {
        LineRay ray;
        if (_unactiveList.Count == 0)
        {
            ray = GameObject.Instantiate(Prefab, transform).GetComponent<LineRay>();
        }
        else
        {
            ray = _unactiveList[_unactiveList.Count - 1];
            _unactiveList.RemoveAt(_unactiveList.Count - 1);
            ray.gameObject.SetActive(true);
        }

        return ray;
    }

    public void Disable(LineRay ray)
    {
        _unactiveList.Add(ray);
        ray.gameObject.SetActive(false);
    }
}