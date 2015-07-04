using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField]
    private ActivateExitEffect _exitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tags.Player)
        {
            Debug.Log("EXIT");
            ShowExitEffect();
        }
    }

    private void ShowExitEffect()
    {
        _exitEffect.StartEffect();
    }
}
