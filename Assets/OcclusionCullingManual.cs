using System.Collections.Generic;
using UnityEngine;

public class OcclusionCullingManual : MonoBehaviour
{
    [Header("Configurações")]
    public Camera cameraPrincipal;
    public LayerMask layerDosObjetos;        // Quais objetos serão verificados (Ex: "Cenário" ou "Objetos")
    public float distanciaMaxima = 100f;     // Até onde verificar objetos
    public float raioDeteccao = 0.5f;        // Tolerância de visibilidade (para não desativar por pequenos obstáculos)
    public float intervaloAtualizacao = 0.2f;// Frequência da checagem

    private List<Renderer> objetosRenderizaveis = new List<Renderer>();
    private float temporizador;

    void Start()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        // Coleta todos os objetos renderizáveis da cena
        Renderer[] todosRenderers = FindObjectsOfType<Renderer>();
        foreach (Renderer r in todosRenderers)
        {
            // Só adiciona os que estão na camada definida
            if (((1 << r.gameObject.layer) & layerDosObjetos) != 0)
                objetosRenderizaveis.Add(r);
        }
    }

    void Update()
    {
        temporizador += Time.deltaTime;
        if (temporizador >= intervaloAtualizacao)
        {
            temporizador = 0;
            AtualizarVisibilidade();
        }
    }

    void AtualizarVisibilidade()
    {
        Plane[] planosCamera = GeometryUtility.CalculateFrustumPlanes(cameraPrincipal);

        foreach (Renderer r in objetosRenderizaveis)
        {
            if (r == null) continue;
            GameObject go = r.gameObject;

            // 1️⃣ Verifica se está dentro do campo de visão da câmera
            bool visivelPelaCamera = GeometryUtility.TestPlanesAABB(planosCamera, r.bounds);

            if (!visivelPelaCamera)
            {
                go.SetActive(false);
                continue;
            }

            // 2️⃣ Verifica se há algo bloqueando a visão (oclusão real)
            Vector3 direcao = (r.bounds.center - cameraPrincipal.transform.position).normalized;
            float distancia = Vector3.Distance(cameraPrincipal.transform.position, r.bounds.center);

            if (Physics.SphereCast(cameraPrincipal.transform.position, raioDeteccao, direcao, out RaycastHit hit, distancia, layerDosObjetos))
            {
                // Se o raycast atingiu algo e esse algo não for o próprio objeto → está ocluído
                if (hit.collider.gameObject != go)
                {
                    go.SetActive(false);
                    continue;
                }
            }

            // Se passou por tudo → está visível
            go.SetActive(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Apenas para debug visual no editor
        if (cameraPrincipal == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cameraPrincipal.transform.position, distanciaMaxima);
    }
}
