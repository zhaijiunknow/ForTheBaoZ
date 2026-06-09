using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Vector3 PointA;
    public Vector3 PointB;
    public int Speed = 1;

    private Vector3 m_Target;
    void Start()
    {
        m_Target = PointA;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_Target, Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, m_Target) < 0.001f)
        {
            if (m_Target == PointA)
            {
                m_Target = PointB;
            }
            else
            {
                m_Target = PointA;
            }
        }
    }
}
