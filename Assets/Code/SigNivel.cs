using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SigNivel : MonoBehaviour
{
    public int escena;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el que entra es el jugador (por etiqueta, por ejemplo)
        if (other.CompareTag("Nivel"))
        {
            SceneManager.LoadScene(escena);
        }
    }
}
