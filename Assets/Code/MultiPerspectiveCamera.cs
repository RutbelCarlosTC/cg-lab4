using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MultiPerspectiveCamera : MonoBehaviour
{
    
    public bool tPerson = true;
    [Header("Objetivos de cámara")]
    public Transform tpTarget;
    public Transform fpTarget;
    [Header("Visibilidad de Jugador")]
    public bool disablePlayerMesh = true;
    [Space(2)]
    public GameObject playerMesh;
    [Space(5)]
    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad, 0);
    private new Camera camera;
    private Vector2 nearPlaneSize;
    private Transform follow;
    private float defaultDistance;
    private float newDistance;
    [Header("Ajustes de Cámara")]
    public float maxDistace = 7f;
    public float minDistance = 2f;
    [Space(6)]
    public int zoomVelocity = 300;
    public float zoomSmoth = 0.1f;
    public Vector2 sensitivity = new Vector2(1,1);
    [Header("Tecla para cambiar perspectiva")]
    public KeyCode keyCode = KeyCode.Q;

    // Start is called before the first frame update
    void Start()
    {

        ChangePerspective(tPerson);


        defaultDistance = (maxDistace + minDistance) / 2;
        newDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();

        CalculateNearPlaneSize();
    }

    void ChangePerspective(bool ThirdPerson)
    {
        if (ThirdPerson)
        {
            follow = tpTarget;

            if (disablePlayerMesh)
                playerMesh.SetActive(true);


            tPerson = true;
        }
        else if(!ThirdPerson)
        {
            follow = fpTarget;

            if(disablePlayerMesh)
            playerMesh.SetActive(false);


            tPerson = false;


        }

    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.nearClipPlane;
        float width = height * camera.aspect;

        nearPlaneSize = new Vector2(width, height);
    }

    private Vector3[] GetCameraCollisionPoints(Vector3 direction)
    {
        Vector3 position = follow.position;
        Vector3 center = position + direction * (camera.nearClipPlane + 0.4f);

        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        return new Vector3[]
        {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };
    }

    void Update()
    {
        float hor = Input.GetAxis("Mouse X");

        if (hor != 0)
        {
            angle.x += hor * Mathf.Deg2Rad * sensitivity.x;
        }

        float ver = Input.GetAxis("Mouse Y");

        if (ver != 0)
        {
            angle.y += ver * Mathf.Deg2Rad * sensitivity.y;
            angle.y = Mathf.Clamp(angle.y, -80 * Mathf.Deg2Rad, 80 * Mathf.Deg2Rad);
        }

        if (tPerson)
        {

            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

            if (scrollDelta > 0)
            {
                newDistance -= 0.1f * (Time.deltaTime * zoomVelocity);
            }
            else if (scrollDelta < 0)
            {
                newDistance += 0.1f * (Time.deltaTime * zoomVelocity);
            }
            newDistance = Mathf.Clamp(newDistance, minDistance, maxDistace);
            defaultDistance = Mathf.Lerp(defaultDistance, newDistance, zoomSmoth);
        }
        else if (!tPerson)
        {
            defaultDistance = 0.1f;


        }


        if (Input.GetKeyDown(keyCode))
        {
            if (tPerson)
                ChangePerspective(false);
            else
                ChangePerspective(true);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(angle.x) * Mathf.Cos(angle.y),
            -Mathf.Sin(angle.y),
            -Mathf.Sin(angle.x) * Mathf.Cos(angle.y)
            );

        RaycastHit hit;
        float distance = defaultDistance;
        Vector3[] points = GetCameraCollisionPoints(direction);

        foreach (Vector3 point in points)
        {
            if (Physics.Raycast(point, direction, out hit, defaultDistance))
            {
                distance = Mathf.Min((hit.point - follow.position).magnitude, distance);
            }
        }

        transform.position = follow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
    }
}