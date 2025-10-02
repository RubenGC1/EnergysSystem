using UnityEngine;
using UnityEngine.UI;

public class ResetTransformManager : MonoBehaviour
{
    [Header("Objetos a restablecer")]
    public GameObject[] objetosAReiniciar;

    [Header("Botón de reinicio")]
    public Button botonReiniciar;

    private Vector3[] posicionesOriginales;
    private Quaternion[] rotacionesOriginales;

    void Start()
    {
        // Validación de referencias
        if (objetosAReiniciar == null || objetosAReiniciar.Length == 0)
        {
            Debug.LogWarning("No se han asignado objetos para reiniciar.");
            return;
        }

        if (botonReiniciar == null)
        {
            Debug.LogWarning("No se ha asignado el botón de reinicio.");
            return;
        }

        // Inicializar arrays
        posicionesOriginales = new Vector3[objetosAReiniciar.Length];
        rotacionesOriginales = new Quaternion[objetosAReiniciar.Length];

        // Almacenar posiciones y rotaciones originales
        for (int i = 0; i < objetosAReiniciar.Length; i++)
        {
            if (objetosAReiniciar[i] != null)
            {
                posicionesOriginales[i] = objetosAReiniciar[i].transform.position;
                rotacionesOriginales[i] = objetosAReiniciar[i].transform.rotation;
            }
            else
            {
                Debug.LogWarning($"El objeto en la posición {i} no está asignado.");
            }
        }

        // Asignar evento al botón
        botonReiniciar.onClick.AddListener(ReiniciarObjetos);
    }

    public void ReiniciarObjetos()
    {
        for (int i = 0; i < objetosAReiniciar.Length; i++)
        {
            if (objetosAReiniciar[i] != null)
            {
                objetosAReiniciar[i].transform.position = posicionesOriginales[i];
                objetosAReiniciar[i].transform.rotation = rotacionesOriginales[i];
            }
        }
    }
}
