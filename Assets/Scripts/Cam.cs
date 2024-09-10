using UnityEngine;

public class CameraViewController : MonoBehaviour
{
    public Transform[] views; // Referencias a las posiciones de las vistas
    public float transitionSpeed = 5f; // Velocidad de transición
    private Transform currentView; // Vista actual
    public PlayerController playerController; // Referencia al controlador del jugador
    public GameObject targetObject;
    public MonoBehaviour scriptToDisable;

    private bool shouldChangeView = false; // Nueva variable para controlar el cambio de vista

    void Start()
    {
        // Establecer la vista inicial
        currentView = views[0];
        targetObject.SetActive(true);
        scriptToDisable.enabled = true;

        // Suscribirse al evento del jugador
        if (playerController != null)
        {
            playerController.OnDestinationReached += OnPlayerReachedDestination; // Suscribirse al evento
        }
    }

    void OnPlayerReachedDestination()
    {
        // Solo cambiar la vista si se ha hecho clic en el objeto correcto
        if (shouldChangeView)
        {
            currentView = views[1]; // Cambiar a la vista 2
            shouldChangeView = false; // Reiniciar la variable para la próxima vez
            targetObject.SetActive(false);
            scriptToDisable.enabled = false;
        }
    }

    void Update()
    {
        // Detectar cuando se presiona la tecla 'E' para cambiar a la vista 1
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentView = views[0]; // Cambiar a la vista 1
            targetObject.SetActive(true);
            scriptToDisable.enabled = true;
        }

        // Detectar clic con el botón izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verificar si el raycast impacta contra un objeto
            if (Physics.Raycast(ray, out hit))
            {
                // Comprobar si el objeto impactado tiene la etiqueta deseada para la vista 2
                if (hit.collider.CompareTag("ViewB"))
                {
                    shouldChangeView = true; // Marcar para cambiar la vista cuando el jugador llegue al destino
                }
            }
        }
    }

    void LateUpdate()
    {
        // Mover la posición de la cámara suavemente hacia la vista seleccionada
        transform.position = Vector3.Lerp(transform.position, currentView.position, Time.deltaTime * transitionSpeed);
        Vector3 currentAngle = new Vector3(
            Mathf.Lerp(transform.rotation.eulerAngles.x, currentView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
            Mathf.Lerp(transform.rotation.eulerAngles.y, currentView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
            Mathf.Lerp(transform.rotation.eulerAngles.z, currentView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed)
        );
        transform.eulerAngles = currentAngle;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar errores
        if (playerController != null)
        {
            playerController.OnDestinationReached -= OnPlayerReachedDestination;
        }
    }
}
