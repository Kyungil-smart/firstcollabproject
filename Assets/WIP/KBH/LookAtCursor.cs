using System;
using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    [SerializeField] private bool _isLookAt = true;

    private void Update()
    {
        if (!_isLookAt) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 direction = ray.GetPoint(distance) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
        
        



    }
}
