using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordProjectile_001 : Item, ICanCauseDamage
{
    private SwordData swordData;
    private Vector3 startRotationAngle;
    private float rotationTime;
    const float rotationMax = 177.0f; // degrees
    private float timer = 0.0f;


    private EdgeCollider2D edgeCollider2D;
    private int playerFacingDirectionCached;


    protected override void Start()
    {
        base.Start();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        
        this.swordData = (SwordData)ItemData;
        
        rotationTime = swordData.duration;

        edgeCollider2D.offset = Model.transform.localPosition;
        transform.localScale += new Vector3(swordData.swingSwordIncreaseSize, swordData.swingSwordIncreaseSize, 1);

        SetSwingDirection();
    }



    
    
    void Update()
    {
        
        // increment timer
        timer += Time.deltaTime;
        // rotate the object
        if (timer < rotationTime)
            Swing();
        else
            Destroy(gameObject);
    }


    private void SetSwingDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x - transform.position.x > 0)
            playerFacingDirectionCached = 1;
        else
            playerFacingDirectionCached = -1;


        if (playerFacingDirectionCached == 1)
        {
            startRotationAngle = new Vector3(0, 0, 90);
        }
        else
        {
            startRotationAngle = new Vector3(0, 0, 0);         
        }

        transform.rotation = Quaternion.Euler(startRotationAngle);

    }

    private void Swing()
    {
        if(playerFacingDirectionCached == 1)
            transform.Rotate(-Vector3.forward * Time.deltaTime * rotationMax / rotationTime);
        else
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationMax / rotationTime);
    }


    public int GetDamage()
    {
        return ((SwordData)ItemData).damage;
    }
}
