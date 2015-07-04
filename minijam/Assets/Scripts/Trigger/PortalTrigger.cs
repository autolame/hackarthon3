using UnityEngine;
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField]
    private int _id;

    [SerializeField]
    private PortalController _portalController;

    [SerializeField]
    private GameObject _effectObject;

    private Collider _collider;

    public int ID
    {
        get
        {
            return _id;
        }

        private set
        {
            _id = value;
        }
    }

    private void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _effectObject.SetActive(false);

        DeactivatePortal();
    }

    public void ActivatePortal()
    {
        _collider.enabled = true;
    }

    public void DeactivatePortal()
    {
        _collider.enabled = false;
    }

    private void ShowPortalEffect()
    {
        _effectObject.SetActive(true);
    }

    public void PlayerArrive()
    {
        
    }

    private void OnTriggerEnter()
    {
        _portalController.PortToNextPortal(this);

        ShowPortalEffect();

        Debug.Log("portal enter");
    }
}
