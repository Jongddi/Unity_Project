using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{

    private Vector3 m_Offset;
    private float m_Coordinate;

    void OnMouseDown()
    {
        m_Coordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        m_Offset = gameObject.transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + m_Offset;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = m_Coordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
