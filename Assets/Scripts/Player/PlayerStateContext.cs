using UnityEngine;

public class PlayerStateContext
{
    private float Speed;

    public PlayerStateContext(
        float _speed)
    {
        Speed = _speed;
    }

    public float GetSpeed()
    {
        return Speed;
    }
}