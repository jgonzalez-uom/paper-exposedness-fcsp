using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    //public Transform targetObject;
    public Collider target;
    public bool lookAtEnabled = true;
    public bool autoZoom;
    public float dummy;
    float distance;
    Vector3 point;

    private void Start()
    {
        distance = ((target.bounds.max) - target.bounds.center).magnitude;
        //target = targetObject.GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtEnabled)
        {
            point = target.bounds.center;

            transform.LookAt(point);

            ColorCapture.distanceFromTarget = Vector3.Distance(point, this.transform.position) - target.bounds.size.magnitude / 2;

            if (!autoZoom)
                return;

            Vector3 a = (target.bounds.center + transform.right * distance) - transform.position;
            Vector3 b = (target.bounds.center - transform.right * distance) - transform.position;
            Debug.DrawLine(transform.position, (target.bounds.center + transform.right * distance), Color.red, 0.1f);
            Debug.DrawLine(transform.position, (target.bounds.center - transform.right * distance), Color.red, 0.1f);
            float angle = Vector3.Angle(a, b);
            Camera.main.fieldOfView = angle * dummy;
        }
    }

    public void SetTarget(Collider to)
    {
        target = to;
    }
}
