using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    public Transform[] Points;

    public GameObject StartPrefab;
    public GameObject EndPrefab;

    private GameObject _startObj;
    private GameObject _endObj;

    // Update is called once per frame
    private void LateUpdate()
    {
        for (int i = 0; i < Points.Length; i++)
        {
            if (Points[i] != null && Points[i].gameObject.activeInHierarchy)
            {
                GetComponent<LineRenderer>().SetPosition(i, Points[i].position);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (Points != null && StartPrefab != null && _startObj == null)
        {
            _startObj = GameObject.Instantiate(StartPrefab, Points[0]) as GameObject;
            _startObj.transform.eulerAngles = _startObj.transform.eulerAngles.SetZ((-Points[0].position + Points[1].position).ToAngle());
            _startObj.transform.localPosition = Vector3.zero;
        }
        if (Points != null && EndPrefab != null && _endObj == null)
        {
            _endObj = GameObject.Instantiate(EndPrefab, Points[1]) as GameObject;
            _endObj.transform.eulerAngles = _endObj.transform.eulerAngles.SetZ((-Points[0].position + Points[1].position).ToAngle());
            _endObj.transform.localPosition = Vector3.zero;
        }
    }

    public void OnDisable()
    {
        if (_startObj != null)
        {
            Destroy(_startObj);
        }
        if (_endObj != null)
        {
            Destroy(_endObj);
        }
    }
}