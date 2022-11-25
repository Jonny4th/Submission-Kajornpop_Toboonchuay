using UnityEngine;

public class Cue : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    [SerializeField] float baseScore;
    float speed; // <-Highway
    Vector3 indicatorPosition; // <-Highway
    public bool IsWithinHitRegion { get; set; }

    #region MonoBehaviours
    void Awake()
    {
        //Cache
        _rigidbody = GetComponent<Rigidbody2D>();

        //Get parameters
        var highway = GetComponentInParent<NoteHighway>();
        speed = highway.Speed;
        indicatorPosition = highway.IndicatorPosition;

        //Subscription
        highway.CuePrepared += OnStart;
    }
    void OnDisable()
    {
        //Unsubscription
        GetComponentInParent<NoteHighway>().CuePrepared-= OnStart;
    }
    #endregion

    public void SetColor(Color cueColor)
    {
        GetComponent<SpriteRenderer>().color = cueColor;
    }

    private void OnStart()
    {
        if(_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.down * speed;
        }
    }
    public void OnHit()
    {
        IsWithinHitRegion = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = indicatorPosition;
        GetComponent<Animator>().SetTrigger("Hit");
        Destroy(gameObject, 1f);
    }
    public float GetScore()
    {
        return baseScore;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
