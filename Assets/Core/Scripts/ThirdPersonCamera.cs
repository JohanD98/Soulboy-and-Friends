using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Camera m_cam;

    [SerializeField] float m_maxHeight;
    [SerializeField] float m_minHeight;
    private float m_currentHeight;

    [SerializeField] float m_maxDistance;
    [SerializeField] float m_minDistance;
    private float m_currentDistance;


    [SerializeField] float m_maxAngle;
    [SerializeField] float m_minAngle;

    [SerializeField] float m_xSensitivity;
    [SerializeField] float m_ySensitivity;
    [SerializeField] float m_scrollSensitivity;

    [SerializeField] bool m_hideInterferingMesh;
    [SerializeField] float m_hideMeshUpdateTimer;
    [SerializeField] float m_hideMeshDistance;
    [SerializeField] int m_hideMeshHorizontalRays;
    [SerializeField] int m_hideMeshVerticalRays;
    [SerializeField] [Range(0f, 1f)] float m_hideMeshHorizontalCenterDistance;
    [SerializeField] [Range(0f, 1f)] float m_hideMeshVerticalCenterDistance;
    [SerializeField] LayerMask m_hideMeshLayerMask;
    private float m_hideMeshCurrentTimer;
    private List<GameObject> m_currentlyHiddenGameobjects;

    [SerializeField] Transform m_parent;

    public Vector3 m_dir;

    private Vector2 m_prevMousePos;
    private Vector2 m_mouseMovementDelta;

    // Start is called before the first frame update
    void Start()
    {
        m_cam = this.GetComponent<Camera>();
        m_currentlyHiddenGameobjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZoom();
        CheckMouseMovement();
        AttemptMoveCamera();
        UpdateCameraPosition();
        HideInterferingMesh();
        LastUpdate();
    }

    private void UpdateZoom()
    {
        m_currentDistance = Mathf.Clamp(m_currentDistance + Input.mouseScrollDelta.y * m_scrollSensitivity * -1, m_minDistance, m_maxDistance);
        m_currentHeight = Mathf.Clamp(m_currentHeight + Input.mouseScrollDelta.y * m_scrollSensitivity * -1, m_minHeight, m_maxHeight);
    }

    void CheckMouseMovement()
    {
        m_mouseMovementDelta = m_prevMousePos - (Vector2)Input.mousePosition;
    }

    void AttemptMoveCamera()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
            m_cam.transform.localEulerAngles = new Vector3(Mathf.Clamp(m_cam.transform.localEulerAngles.x + m_mouseMovementDelta.y * m_ySensitivity, m_minAngle, m_maxAngle), m_cam.transform.localEulerAngles.y + -1 * m_mouseMovementDelta.x * m_xSensitivity, 0);
            m_dir = m_cam.transform.localEulerAngles;
        }
        else
        {
            Cursor.visible = true;
        }

    }

    void UpdateCameraPosition()
    {
        float yAngle = ((m_dir.y + 180) + 90) % 360;
        yAngle *= Mathf.PI / 180;
        float xAngle = (m_dir.x + 180) % 360;
        xAngle *= Mathf.PI / 180;
        m_cam.transform.position = new Vector3(m_parent.position.x + Mathf.Cos(yAngle) * Mathf.Cos(xAngle) * m_currentDistance, m_parent.position.y + m_currentDistance * Mathf.Sin(xAngle) * -1, m_parent.position.z + Mathf.Sin(yAngle) * Mathf.Cos(xAngle) * m_currentDistance * -1);
    }

    void HideInterferingMesh()
    {
        if (!m_hideInterferingMesh) { return; }

        m_hideMeshCurrentTimer += Time.deltaTime;
        if (m_hideMeshCurrentTimer >= m_hideMeshUpdateTimer)
        {
            ShowAllHiddenMeshes();
            m_hideMeshCurrentTimer -= m_hideMeshUpdateTimer;
            for (int y = 0; y < m_hideMeshVerticalRays; y++)
            {
                for (int x = 0; x < m_hideMeshHorizontalRays; x++)
                {
                    RaycastHit hit;
                    float xRay = (Screen.width * m_hideMeshHorizontalCenterDistance) * ((float)x / (float)m_hideMeshHorizontalRays) + (Screen.width * (1 - m_hideMeshHorizontalCenterDistance) * 0.5f);
                    float yRay = (Screen.height * m_hideMeshVerticalCenterDistance) * ((float)y / (float)m_hideMeshVerticalRays) + (Screen.height * (1 - m_hideMeshVerticalCenterDistance) * 0.5f);
                    Ray ray = m_cam.ScreenPointToRay(new Vector2(xRay, yRay));
                    if (Physics.Raycast(ray, out hit, m_hideMeshDistance, m_hideMeshLayerMask))
                    {
                        Debug.DrawRay(m_cam.transform.position, (m_cam.transform.position - hit.transform.position).normalized, Color.red, m_hideMeshUpdateTimer);
                        HideMesh(hit.transform.gameObject);
                    }
                }
            }
        }

    }

    private void HideMesh(GameObject GO)
    {
        Debug.Log(GO.name);
        if (m_currentlyHiddenGameobjects.Contains(GO)) { return; }
        MeshRenderer mr = GO.GetComponent<MeshRenderer>();
        if (mr == null) { return; }

        if (mr.enabled)
        {
            mr.enabled = false;
            m_currentlyHiddenGameobjects.Add(GO);
        }
    }

    private void ShowAllHiddenMeshes()
    {
        for (int i = 0; i < m_currentlyHiddenGameobjects.Count; i++)
        {
            m_currentlyHiddenGameobjects[i].GetComponent<MeshRenderer>().enabled = true;
        }
        m_currentlyHiddenGameobjects.Clear();
    }

    void LastUpdate()
    {
        m_prevMousePos = Input.mousePosition;
    }
}
