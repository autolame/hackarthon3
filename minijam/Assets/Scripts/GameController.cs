using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player, _startPosition;

    public GameObject Player
    {
        get
        {
            return _player;
        }

        private set
        {
            _player = value;
        }
    }

    private void Start()
    {
        PlayerToStartPosition();
    }

    private void PlayerToStartPosition()
    {
        if(_player != null && _startPosition != null)
        {
            _player.transform.position = _startPosition.transform.position;
        }
    }
}
