using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Boomerang : Item, ICanCauseDamage
{
    private Rigidbody2D rb;
    private EdgeCollider2D edgeCollider2D;
    
    private BoomerangData boomerangData;


    private const float scopeRecall = 1.0f;
    private Vector3 rotateVector = new Vector3(0,0,360);

    // Cached
    private Player player;
    private bool isCollide = false;
    private bool isReturning = true;
    private float timeRelease;
    private float timeToReturn;
    private float rotateSpeed;
    private float currentBoomerangSpeed;
    private Vector2 mousePosition;
    private Vector2 direction;
    private Vector2 currentVelocity;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        edgeCollider2D.isTrigger = true;
        boomerangData = (BoomerangData)ItemData;
        timeToReturn = boomerangData.timeToReturn;
        rotateSpeed = boomerangData.rotateSpeed;
    }


    public override bool Use(Player player)
    {
        if (isReturning == false) return false;

        // Rereference if null
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (edgeCollider2D == null)
            edgeCollider2D = GetComponent<EdgeCollider2D>();
        if(boomerangData == null)
            boomerangData = (BoomerangData)ItemData;


        isReturning = false;
        this.player = player;
        gameObject.SetActive(true);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - (Vector2)transform.position;
        rb.velocity = direction.normalized * boomerangData.releaseSpeed;
        

        return true;
    }


    private void Update()
    {
        if (isReturning == true) return;
        if (player == null) return;
        
        

        timeRelease += Time.deltaTime;
        if (timeRelease > timeToReturn)
        {
            BoomerangReturn();
            if (Vector2.Distance(player.transform.position, transform.position) < scopeRecall)
            {
                ResetBoomerangState();
            }

        }

        if(isCollide && timeRelease > 0.1f)
        {
            BoomerangReturn();
            if (Vector2.Distance(player.transform.position, transform.position) < scopeRecall)
            {
                ResetBoomerangState();
            }
        }


        RotateBoomerang();
    }


    private void FixedUpdate()
    {
        // Clamp velocity not larger boomerang release speed.
        currentVelocity = rb.velocity;
        currentBoomerangSpeed = currentVelocity.magnitude;
        if (currentBoomerangSpeed > boomerangData.releaseSpeed)
        {
            currentVelocity = currentVelocity.normalized * boomerangData.releaseSpeed;
            rb.velocity = currentVelocity;
        }
    }

    private void RotateBoomerang()
    {
        this.transform.Rotate(rotateVector * Time.deltaTime * rotateSpeed);
    }


    private void BoomerangReturn()
    {
        direction = player.transform.position - transform.position;
        rb.velocity = direction.normalized * boomerangData.releaseSpeed * 2f;
    }

    private void ResetBoomerangState()
    {
        isCollide = false;
        isReturning = true;
        timeRelease = 0f;
        gameObject.SetActive(false);
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing if collider with player tag.
        if (collision.CompareTag("Player") == true) return;
        if (collision.CompareTag("Untagged") == true) return;
        
        isCollide = true;       
    }

    public int GetDamage()
    {
        return boomerangData.damage;
    }
}
