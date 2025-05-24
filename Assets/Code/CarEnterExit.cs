
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CarEnterExit : MonoBehaviour
{

    [Header("Camara")]
    public GameObject carCamera;
    [Header("Posición de Salida")]
    public Transform posicionDeSalida;
    [Header("Contiene Conductor?")]
    [HideInInspector]
    public bool contieneConductor = false;
    [HideInInspector]
    [Tooltip("Objeto/Jugador que se activará o desactivará al salir y entrar del vehiculo")]
    public GameObject conductor;

    private PrometeoCarController carController;
    private Rigidbody rigid;
    private GameObject player;
    [HideInInspector]
    private bool insideCar = false;
    void Start()
    {
        insideCar = false;
        carController = this.GetComponent<PrometeoCarController>();
        rigid = this.GetComponent<Rigidbody>();
        carCamera.gameObject.SetActive(false);
        carController.enabled = false;

        if (contieneConductor && conductor != null)
        {
            conductor.SetActive(false);

        }
    }



        private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (Input.GetKeyDown("e") && !insideCar)
            {
                carController.enabled = true;
                if (carController.useSounds)
                {
                    carController.carEngineSound.Play();
                    carController.tireScreechSound.Play();
                }
                if (contieneConductor && conductor != null)
                {
                    conductor.SetActive(true);
                }
                if (player == null)
                    player = other.gameObject;
                player.SetActive(false);
                player.transform.parent = posicionDeSalida.transform;
                player.transform.position = posicionDeSalida.position;
                carCamera.gameObject.SetActive(true);
                StartCoroutine(EnableInsideCar(true));
            }

        }
    }

    private void Update()
    {
        if (insideCar)
        {
            if (Input.GetKeyDown("e"))
            {
                carController.Brakes();
                carController.enabled = false;
                if (carController.useSounds)
                {
                    carController.carEngineSound.Stop();
                    carController.tireScreechSound.Stop();
                }
                if (contieneConductor && conductor != null)
                {
                    conductor.SetActive(false);
                }
                player.SetActive(true);
                player.transform.position = posicionDeSalida.position;
                player.transform.parent = null;
                carCamera.gameObject.SetActive(false);
                StartCoroutine(EnableInsideCar(false));
            }
        }
    }


    private IEnumerator EnableInsideCar(bool option)
    {
        yield return new WaitForSeconds(0.2f);
        insideCar = option;

    }




#if UNITY_EDITOR
    [CustomEditor(typeof(CarEnterExit))]
    public class EditorCarEnterExit : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // for other non-HideInInspector fields

            CarEnterExit script = (CarEnterExit)target;

            // draw checkbox for the bool
            EditorGUILayout.LabelField("Opciones de Conductor", EditorStyles.boldLabel);
            script.contieneConductor = EditorGUILayout.Toggle("Hay un conductor?", script.contieneConductor);
            if (script.contieneConductor) // if bool is true, show other fields
            {
                script.conductor = EditorGUILayout.ObjectField("Conductor", script.conductor, typeof(GameObject), true) as GameObject;
            }
            else
            {
                script.conductor = null;
            }
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);

            GUIStyle coloredLabelStyle;
            if (script.insideCar)
            {
                coloredLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                coloredLabelStyle.normal.textColor = Color.green;
                EditorGUILayout.LabelField("Dentro del Vehiculo", coloredLabelStyle);
            }
            else
            {
                coloredLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                coloredLabelStyle.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Fuera del Vehículo", coloredLabelStyle);
            }
        }
    }
#endif

}
