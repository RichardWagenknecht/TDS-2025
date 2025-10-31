using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BlinkDroga : MonoBehaviour
{
    public Camera PlayerCamera;
    public Camera initialCamera;
    public Volume volume;             // Arraste aqui seu Box Volume
    private Vignette vignette;

    public float maxIntensidade = 0.8f; // Quão “fechado” o olho fica
    public float duracaoFechar = 1.5f;  // Tempo para fechar (olho pesado)
    public float duracaoAbrir = 2.5f;   // Tempo para abrir de novo
    public float intervalo = 4f;        // Tempo entre piscadas

    private float timer = 0f;
    private float cicloTotal;           // duração total de um ciclo (fechar + abrir + intervalo)
    private enum EstadoPiscar { Esperando, Fechando, Abrindo }
    private EstadoPiscar estado = EstadoPiscar.Abrindo; // começa abrindo o olho

    void Start()
    {
        if (volume != null && volume.profile != null)
            volume.profile.TryGet(out vignette);

        if (vignette != null)
        {
            // Começa com o olho "fechado"
            vignette.intensity.value = maxIntensidade;
            PlayerCamera.gameObject.SetActive(false);
        }

        cicloTotal = duracaoFechar + duracaoAbrir + intervalo;

        // ativa o player após alguns segundos (se necessário)
        Invoke(nameof(DesativarCamera), 10f);
    }

    private void Update()
    {
        if (vignette == null)
            return;

        timer += Time.deltaTime;

        switch (estado)
        {
            case EstadoPiscar.Esperando:
                if (timer >= intervalo)
                {
                    estado = EstadoPiscar.Fechando;
                    timer = 0f;
                }
                break;

            case EstadoPiscar.Fechando:
                float tFechar = timer / duracaoFechar;
                vignette.intensity.value = Mathf.Lerp(0, maxIntensidade, tFechar);

                if (tFechar >= 1f)
                {
                    estado = EstadoPiscar.Abrindo;
                    timer = 0f;
                }
                break;

            case EstadoPiscar.Abrindo:
                float tAbrir = timer / duracaoAbrir;
                vignette.intensity.value = Mathf.Lerp(maxIntensidade, 0, tAbrir);

                if (tAbrir >= 1f)
                {
                    estado = EstadoPiscar.Esperando;
                    timer = 0f;
                }
                break;
        }
    }

    private void DesativarCamera()
    {
        initialCamera.gameObject.SetActive(false);
        PlayerCamera.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
