using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    [SerializeField] private GameObject _ball, _coin, _obsticle, _gem, _heart;
    public static GameObject ball, coin, obsticle, gem, heart;

    private void Awake()
    {
        gem = _gem;
        ball = _ball;
        coin = _coin;
        obsticle = _obsticle;
        heart = _heart;
    }
}
