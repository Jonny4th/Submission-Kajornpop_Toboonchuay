using UnityEngine;

public class DespawnLine : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDespawnable obj))
        {
            obj.Despawn();
        }
    }
}
