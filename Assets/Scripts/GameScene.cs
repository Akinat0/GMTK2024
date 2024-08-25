
using UnityEngine;

public class GameScene : MonoBehaviour
{
    static GameScene Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
