using UnityEngine;

public class RoomAudioTrigger : MonoBehaviour
{
    [Header("Referências de Áudio")]
    public AudioSource musicSource;    // Trilha sonora ambiente da sala
    public AudioSource sfxSource;      // Som pontual (ex: porta, grito, etc)

    [Header("Configurações")]
    public bool playOnce = true;       // Tocar apenas na primeira vez que o player entra
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Garante que apenas o player acione
        if (!other.CompareTag("Player"))
            return;

        // Impede repetição, se configurado
        if (playOnce && hasPlayed)
            return;

        if (musicSource != null)
        {
            // Se quiser que a música comece do zero
            if (!musicSource.isPlaying)
                musicSource.Play();
        }

        if (sfxSource != null)
            sfxSource.PlayOneShot(sfxSource.clip);

        hasPlayed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Se quiser que a trilha pare ao sair da sala, descomente a linha abaixo:
        // if (musicSource != null) musicSource.Stop();
    }
}
