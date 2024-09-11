using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private Camera mainCamera;
    public LayerMask sushiLayerMask; // Capa del sushi para detectar con el Raycast
    public LayerMask tableLayerMask; // Capa de la mesa para detectar cuando el cuchillo toca la mesa
    public float moveSpeed = 5f; // Velocidad de movimiento del cuchillo
    public float descendAmount = 0.02f; // Cantidad de descenso por ciclo
    public float zMinLimit = -2f; // Límite mínimo en el eje Z
    public float zMaxLimit = 2f; // Límite máximo en el eje Z
    public float xMinLimit = -1.5f; // Límite mínimo en el eje X
    public float xMaxLimit = 1.5f; // Límite máximo en el eje X
    public float raycastMaxDistance = 0.1f; // Distancia máxima del Raycast para detectar la mesa
    private bool isMovingForward = false; // Controla si el cuchillo se está moviendo hacia adelante
    private bool canDescend = true; // Controla si el cuchillo puede seguir descendiendo
    private Vector3 startPosition;

    public GameObject cuttingBoard; // GameObject a desactivar
    public GameObject newObjectPrefab; // Prefab del nuevo GameObject a instanciar
    public GameObject particleEffectPrefab; // Prefab del sistema de partículas a instanciar
    private bool objectInstantiated = false; // Variable para controlar si el objeto ya fue instanciado

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position; // Guardamos la posición inicial del cuchillo
        newObjectPrefab.SetActive(false);
        cuttingBoard.SetActive(true);
        particleEffectPrefab.SetActive(false);
    }

    void Update()
    {
        MoveKnife();
        CheckCut();
    }

    void MoveKnife()
    {
        if (Input.GetMouseButton(0)) // Si se mantiene presionado el botón izquierdo del mouse
        {
            // Obtener la posición del mouse en el mundo
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero); // Plano horizontal para calcular el movimiento del mouse
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                Vector3 targetPosition = ray.GetPoint(distance); // Obtener el punto en el plano donde el mouse apunta
                Vector3 newPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

                // Limitar el movimiento en el eje X y Z
                newPosition.x = Mathf.Clamp(newPosition.x, xMinLimit, xMaxLimit);
                newPosition.z = Mathf.Clamp(newPosition.z, zMinLimit, zMaxLimit);

                // Movimiento del cuchillo hacia la posición del mouse
                transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveSpeed);

                // Controlar el descenso solo cuando el cuchillo se mueve hacia adelante y luego hacia atrás
                if (canDescend && isMovingForward && transform.position.x < startPosition.x)
                {
                    transform.position -= new Vector3(0, descendAmount, 0); // Descenso controlado del cuchillo
                    isMovingForward = false; // Restablecemos el estado de movimiento hacia adelante
                }
                else if (!isMovingForward && transform.position.x > startPosition.x)
                {
                    isMovingForward = true; // Ahora el cuchillo se ha movido hacia adelante
                }
            }
        }
    }

    public float leftOffset = 0.2f;
    public float leftOffset2 = 0.8f;
    void CheckCut()
    {
        // Definir el desplazamiento hacia la izquierda
        Vector3 rayOrigin = transform.position + transform.right * -leftOffset; // Restar para mover a la izquierda

        Vector3 rayOrigin2 = transform.position + transform.right * -leftOffset2;

        Ray ray = new Ray(rayOrigin, Vector3.down); // Lanzar el Raycast hacia abajo desde la nueva posición
        RaycastHit hit;

        Ray ray2 = new Ray(rayOrigin2, Vector3.down);


        // Visualizar el Raycast en la escena
        Debug.DrawRay(rayOrigin, Vector3.down * raycastMaxDistance, Color.red);
        Debug.DrawRay(rayOrigin2, Vector3.down * raycastMaxDistance, Color.blue);

        // Usar raycastMaxDistance para limitar la distancia de detección
        if (Physics.Raycast(ray, out hit, raycastMaxDistance, sushiLayerMask))
        {
            Debug.Log("¡Sushi detectado! Preparado para cortar.");
        }
        else if (Physics.Raycast(ray, out hit, raycastMaxDistance, tableLayerMask))
        {
            Debug.Log("¡Corte realizado en la mesa!");

            if (cuttingBoard != null)
            {
                cuttingBoard.SetActive(false); // Desactiva el GameObject de la tabla de cortar
                particleEffectPrefab.SetActive(true);
            }

            if (!objectInstantiated && newObjectPrefab != null)
            {
                // Instanciar el nuevo GameObject en la posición del Raycast (posición de la mesa)
                Instantiate(newObjectPrefab, hit.point, Quaternion.identity);
                objectInstantiated = true; // Marcar como instanciado
                newObjectPrefab.SetActive(true);

                // Instanciar el sistema de partículas en la misma posición
                if (particleEffectPrefab != null)
                {
                    Instantiate(particleEffectPrefab, hit.point, Quaternion.identity);
                }
            }

            canDescend = false; // Detener el descenso al detectar la mesa
        }

        if (Physics.Raycast(ray2, out hit, raycastMaxDistance, sushiLayerMask))
        {
            Debug.Log("¡Sushi detectado! Preparado para cortar.");
        }
        else if (Physics.Raycast(ray2, out hit, raycastMaxDistance, tableLayerMask))
        {
            Debug.Log("¡Corte realizado en la mesa!");

            if (cuttingBoard != null)
            {
                cuttingBoard.SetActive(false); // Desactiva el GameObject de la tabla de cortar
                particleEffectPrefab.SetActive(true);
            }

            if (!objectInstantiated && newObjectPrefab != null)
            {
                // Instanciar el nuevo GameObject en la posición del Raycast (posición de la mesa)
                Instantiate(newObjectPrefab, hit.point, Quaternion.identity);
                objectInstantiated = true; // Marcar como instanciado
                newObjectPrefab.SetActive(true);

                // Instanciar el sistema de partículas en la misma posición
                if (particleEffectPrefab != null)
                {
                    Instantiate(particleEffectPrefab, hit.point, Quaternion.identity);
                }
            }

            canDescend = false; // Detener el descenso al detectar la mesa
        }
    }
}
