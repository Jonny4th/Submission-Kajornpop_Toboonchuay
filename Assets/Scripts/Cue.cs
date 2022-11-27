using UnityEngine;

public class Cue : MonoBehaviour, IDespawnable
{
    Rigidbody2D _rigidbody;

    [SerializeField] float baseScore;
    [SerializeField] float speed; // <-Highway
    [SerializeField] Vector3 indicatorPosition; // <-Highway
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
        GetComponentInParent<NoteHighway>().CuePrepared -= OnStart;
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
            _rigidbody.velocity = - speed * transform.up ;
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
