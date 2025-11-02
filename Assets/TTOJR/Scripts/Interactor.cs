using System;
using System.Collections;
using DependencyInjection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Extensions;

[DefaultExecutionOrder(400)]
public class Interactor : MonoBehaviour, IDependencyProvider
{
    [Provide] public Interactor Provide() => this;
    public bool holdInteraction;

    [Inject] MainCamera mainCamera;
    [Inject] EntityControls controls;
    public LayerMask interactionMask;
    public GameObject holdDisplay;

    public bool canInteract;
    public GameObject interactDisplay;
    public TMP_Text interactText;

    public UnityEvent InteractEvent;
    public UnityEvent InteractHoldEvent;
    public UnityEvent InteractHoldCanceledEvent;

    public Action<Ray, RaycastHit> RaycasterEvent;
    public Action FailedRaycast;

    private void OnEnable()
    {
        controls.interact += Interact;
        controls.interactHold += InteractHold;
        controls.interactHoldCancel += InteractHoldCanceled;
        RaycasterEvent += InteractorRaycast;
    }

    private void OnDisable()
    {
        controls.interact -= Interact;
        controls.interactHold -= InteractHold;
        controls.interactHoldCancel -= InteractHoldCanceled;
        RaycasterEvent -= InteractorRaycast;
    }

    private void Start()
    {
        interactDisplay.SetActive(false);
    }
    private void Update()
    {
        CastInteractorRacyast();
    }

    public void ToggleCanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
        interactDisplay.SetActive(canInteract);
    }
    public void SetInteractText(string text)
    {
        interactDisplay.SetActive(canInteract);
        interactText.text = text;
    }

    public void SetInteractEvent(UnityEvent callback)
    {
        InteractEvent = callback;
    }
    public void SetInteracHoldEvent(UnityEvent callback)
    {
        InteractHoldEvent = callback;
    }
    public void SetInteractHoldCancledEvent(UnityEvent callback)
    {
        InteractHoldCanceledEvent = callback;
    }

    public void ClearAllInteractEvent()
    {
        InteractEvent = null;
        InteractHoldEvent = null;
        InteractHoldCanceledEvent = null;
    }
    public void Interact()
    {
        if (canInteract)
        {
            InteractEvent?.Invoke();
            print("Interactor: Interacted callback");
        }
    }
    public void InteractHold()
    {
        if (canInteract && holdInteraction)
        {
            InteractHoldEvent?.Invoke();
            print("Interactor: Interact HOLD callback");
            holdDisplay.SetActive(value: true);
        }
    }

    public void SetHoldingInteraction(bool val) => holdInteraction = val;

    public void InteractHoldCanceled()
    {
        InteractHoldCanceledEvent?.Invoke();
        print("Interactor: HOLD CANCLED callback");
        holdDisplay.SetActive(value: false);
    }

    public void CastInteractorRacyast()
    {
        Ray ray = new Ray(mainCamera.cam.transform.position, mainCamera.cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, mainCamera.castDist, interactionMask))
        {
            RaycasterEvent?.Invoke(ray, hit);
            print(hit);
            print(hit.transform);
            print(hit.transform.gameObject);
            print(hit.transform.gameObject.TryGet<Detector>().name);
            hit.transform.gameObject.TryGet<Detector>().OnRaycastedStay(gameObject);
        }
        else
            FailedRaycast?.Invoke();

    }

    void InteractorRaycast(Ray ray, RaycastHit hit)
    {
        Debug.DrawLine(ray.origin, hit.point, Color.red);
    }



}