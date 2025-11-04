using DependencyInjection;
using UnityEngine;

public class PlaceLocation : MonoBehaviour, IDetectorBuilder
{

    #region Privates
    [Inject] CallbackDetector cbd;
    [Inject] Interactor interactor;
    [SerializeField] Transform loc;
    #endregion

    private void Awake()
    {
        BuildDetector();
    }

    public void Place(GameObject go)
    {
        GameObject spawned = UnityEngine.Object.Instantiate(
            go,
            loc.position,
            Quaternion.identity,
            loc
        );
    }



    #region Methods

    public void BuildDetector()
    {
        cbd = new CallbackDetector.Builder(gameObject)
            .WithEventHooks(stay: true, exit: true)
            .WithInteractAssignments(interactor, "Place (E)")
            .WithRaycast()
            .Build();
    }

    #endregion

}
