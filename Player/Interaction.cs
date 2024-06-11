using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private float checkRate = 0.05f;
    private float lastCheckTime;
    private float maxCheckDistance = 3;
    public LayerMask layerMask;

    private GameObject curInteractGameObject;
    private IInteractable cureInteractable;

    public TextMeshProUGUI promptText;
    private Camera camera;

    // Start is called before the first frame update
    private void Start()
    {
        camera = Camera.main;        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 rayStart = transform.position - ray.origin;
            RaycastHit hit;
            Debug.DrawRay(transform.position + Vector3.up * 0.8f, ray.direction.normalized * maxCheckDistance, Color.red);
            if (Physics.Raycast(transform.position + Vector3.up * 0.8f, ray.direction.normalized, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    cureInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                cureInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = cureInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && cureInteractable != null)
        {
            cureInteractable.OnInteract();
            curInteractGameObject = null;
            cureInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
