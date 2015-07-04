using UnityEngine;
using System.Collections;

public class KeyTrigger : MonoBehaviour
{
    [SerializeField]
    private PortalController _portal;

    private void RemoveKeyAndActivatePortal()
    {
        _portal.ActivatePortals();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        RemoveKeyAndActivatePortal();   
    }
}
