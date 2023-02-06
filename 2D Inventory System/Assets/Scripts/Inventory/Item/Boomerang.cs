using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Boomerang : Item, ICanCauseDamage
{
    private Rigidbody2D rb;
    private EdgeCollider2D edgeCollider2D;
    Player player;
    private BoomerangData boomerangData;


    private bool isCollide = false;


    private bool isReturning = true;
    private float timeRelease;
    private float timeToReturn;
    private float rotateSpeed;

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

        isReturning = false;
        gameObject.SetActive(true);
        
        Debug.Log("Use");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (edgeCollider2D == null)
            edgeCollider2D = GetComponent<EdgeCollider2D>();
        if(boomerangData == null)
            boomerangData = (BoomerangData)ItemData;

        rb.velocity = direction.normalized * boomerangData.releaseSpeed;
        this.player = player;
   
        return true;
    }


    private void Update()
    {
        if (isReturning == true) return;
        if (player == null)
        {
            Debug.Log("Player == null");
            return;
        }
        
        RotateBoomerang();

        

        timeRelease += Time.deltaTime;

        if (timeRelease > timeToReturn)
        {
            BoomerangReturn();
            if (Vector2.Distance(player.transform.position, transform.position) < 1.0f)
            {
                isReturning = true;
                isCollide = false;             
                timeRelease = 0f;
                gameObject.SetActive(false);
            }

        }

        if(isCollide && timeRelease > 0.1f)
        {
            BoomerangReturn();
            if (Vector2.Distance(player.transform.position, transform.position) < 1.0f)
            {
                isCollide = false;
                isReturning = true;      
                timeRelease = 0f;
                gameObject.SetActive(false);
            }
        }
        

    }


    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        float speed = velocity.magnitude;
        if (speed > boomerangData.releaseSpeed)
        {
            velocity = velocity.normalized * boomerangData.releaseSpeed;
            rb.velocity = velocity;
        }
    }

    private void RotateBoomerang()
    {
        this.transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime * rotateSpeed);
    }


    private void BoomerangReturn()
    {
        Vector2 direction = player.transform.position - transform.position;
        rb.velocity = direction.normalized * boomerangData.releaseSpeed * 2f;
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
