using UnityEngine;
using System.Collections;

public class SetPlayerToStartPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _startPosition;

    [SerializeField]
    private GameObject _player;

    public void PlayerToStartPosition()
    {
        if(_startPosition != null)
        {
            _player.transform.position = _startPosition.position;
        }
    }
}
