using UnityEngine;
using System.Collections;

public class PortalController : MonoBehaviour
{
    [SerializeField]
    private PortalTrigger[] _portals;

    [SerializeField]
    private GameController _gameController;

    private void Start()
    {
        //_gameController = transform.Find("GameController").GetComponent<GameController>();
    }

    public void ActivatePortals()
    {
        foreach(var portal in _portals)
        {
            portal.ActivatePortal();
        }
    }

    public void DeactivatePortals()
    {
        foreach(var portal in _portals)
        {
            portal.DeactivatePortal();
        }
    }

    public void PortToNextPortal(PortalTrigger currentPortal)
    {
        foreach(var portal in _portals)
        {
            if(portal.ID == currentPortal.ID + 1)
            {
                if(_gameController != null)
                {
                    _gameController.Player.transform.position = portal.transform.position;
                    portal.PlayerArrive();
                }
                else
                {
                    Debug.Log("GameController is missing on protal");
                }
                break;
            }
        }
    }
}
