using UnityEngine;
using System.Collections;

public class ActivateExitEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject _exitEffectObject;

    private void Start()
    {
        _exitEffectObject.SetActive(false);
    }

    public void StartEffect()
    {
        _exitEffectObject.SetActive(true);
    }
}
