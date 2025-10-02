using UnityEngine;

public class TeleportOnTrigger : MonoBehaviour
{
    [Header("Objeto destino (Empty o cualquier GameObject)")]
    public Transform teleportTarget;

    [Header("Jugador a teletransportar")]
    public Transform player;

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra es el jugador
        if (other.transform == player)
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (teleportTarget != null && player != null)
        {
            player.position = teleportTarget.position;
            player.rotation = teleportTarget.rotation; // opcional, para que rote igual
        }
        else
        {
            Debug.LogWarning("TeleportOnTrigger: faltan referencias en el inspector.");
        }
    }
}
