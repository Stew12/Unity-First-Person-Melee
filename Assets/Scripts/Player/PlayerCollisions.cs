using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisions : MonoBehaviour
{

    public bool canTakeDamage = true;

    public bool attackBlocked;

    public bool attackParried;

    public float blockDefenseFactor = 0.65f;

    public Image hurtFlash;

    public float hurtFlashTime = 0.1f;

    private IEnumerator coroutine;

    private void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Enemy":
                //Take damage from enemy
                if (canTakeDamage)
                {
                    if (!attackParried)
                    {
                        TakeDamage(col);
                    }
                    else 
                    {
                        attackParried = false;

                        //Larger momentum increase when parrying.
                        GetComponent<PlayerController>().ParryMomentumIncrease();

                        //Knock ENEMY backward (relative to the player)
                        col.GetComponent<Enemy>().EnemyKnockBack(gameObject.transform);
                    }
                    

                    canTakeDamage = false;

                    GetComponent<PlayerController>().DontTakeDamage();

                }

                col.GetComponent<Enemy>().Wait();

            break;

            case "Detection Radius Enter":
                Debug.Log("Entered detection radius!");
                col.GetComponent<DetectionRadius>().PlayerEntered();
            break;
            
            default:

            break;
        }
    }

    private void TakeDamage(Collider enemy)
    {   
        // If attack is not blocked or parried, take full damage, if blocked, take a percentage of the damage,
        if (!attackBlocked)
        {
            GetComponent<PlayerValues>().currentHealth -= (int)enemy.GetComponent<Enemy>().attackDamage;
        }
        else
        {
            GetComponent<PlayerValues>().currentHealth -= (int)(enemy.GetComponent<Enemy>().attackDamage * blockDefenseFactor);
            attackBlocked = false;
            GetComponent<PlayerController>().StopBlocking();
        }

        hurtFlash.enabled = true;
        coroutine = HurtFlashDisappear(hurtFlashTime);
        StartCoroutine(coroutine);

        //Knock player backward (relative to the enemy, not the player)
        GetComponent<PlayerController>().KnockBack(enemy.gameObject.transform);
    }

    IEnumerator HurtFlashDisappear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        hurtFlash.enabled = false;
    }

    private void OnTriggerExit(Collider col)
    {
        switch (col.tag)
        {
            case "Detection Radius Exit":
                Debug.Log("Exited detection radius...");
                col.GetComponent<DetectionRadius>().PlayerExited();
            break;

            default:

            break;
        }
    }
}
