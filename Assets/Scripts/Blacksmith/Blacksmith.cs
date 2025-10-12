using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class Blacksmith : MonoBehaviour, IInteractable
{
    [SerializeField] private BlacksmithItemList _componentsHeld;
    private ShopSystem _shopSystem;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }

}
