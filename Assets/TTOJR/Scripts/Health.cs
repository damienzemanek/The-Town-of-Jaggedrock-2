using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _health;
    public float health { get => _health; private set => _health = Mathf.Clamp(value, 0, 9999f);}
    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if (health <= 0) Die();
        print($"Health: {gameObject.name} has taken damage {dmg}, its new healht is {health}");
    }

    void Die()
    {
        print($"Health: {gameObject.name} has died");
    }

}
