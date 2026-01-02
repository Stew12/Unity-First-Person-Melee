using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System;
using UnityEngine.InputSystem.Controls;

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
        if (col.GetComponent<Collision>() != null)
            col.GetComponent<Collision>().collidedWith(col, this);
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

    public IEnumerator EnemyCollision(Collider col, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        //Take damage from enemy
        if (canTakeDamage)
        {
            //TODO: make it that player can't parry enemy while they're winding up

            //BUG TBF: enemy hitting parry and player hitbox causes the hitbox collision to return a parry, but 
            //since it happens same time as enemy hitting player hitbox, seems to not be ready by playercontroller, causing a 'false parry'

            if (!attackParried)
            {
                TakeDamage(col, true);
            }
            else
            {
                // Attack parry
                if (col.GetComponent<Enemy>().enemyAttacking)
                {
                    //Larger momentum increase when parrying.
                    GetComponent<PlayerController>().MomentumIncrease(true);

                    //Knock ENEMY backward (relative to the player)
                    col.GetComponent<Enemy>().EnemyKnockBack(gameObject, true);

                    GetComponent<PlayerController>().audioSource.pitch = 1;
                    GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().parrySound);

                    GetComponent<PlayerController>().DontTakeDamage();
                }
                else
                {
                    TakeDamage(col, true);
                }

                attackParried = false;
            }
        }

        col.GetComponent<Enemy>().Wait();
    }

    public void EnemyProjectileCollision(Collider col)
    {
        if (canTakeDamage)
        {
            if (col.GetComponent<EnemyProjectile>() != null)
                col.GetComponent<EnemyProjectile>().EnemyCasterCanFire();

            else if (col.GetComponent<EnemyAttackDisjoint>() != null)
            {
                col.GetComponent<EnemyAttackDisjoint>().EnemyCasterCanFire();
                GetComponent<PlayerController>().KnockBack(col.GetComponent<EnemyAttackDisjoint>().enemyCasterClass.gameObject.transform);
            }

            //Projectiles can be blocked, but not parried. However sword slashes can be parried, which are classified as projectiles but fucntionally aren't.
            if (!attackParried)
            {
                TakeDamage(col, true);
                canTakeDamage = false;

                GetComponent<PlayerController>().DontTakeDamage();

                if (col.GetComponent<EnemyProjectile>() != null)
                    Destroy(col.gameObject);
            }
            else
            {
                //Larger momentum increase when parrying.
                GetComponent<PlayerController>().MomentumIncrease(true);

                //Knock ENEMY backward (relative to the player)
                col.GetComponent<EnemyAttackDisjoint>().enemyCasterClass.GetComponent<Enemy>().EnemyKnockBack(gameObject, true);

                GetComponent<PlayerController>().audioSource.pitch = 1;
                GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().parrySound);

                attackParried = false;
            }
        }
    }

    public void TrapCollision(Collider col)
    {
        if (canTakeDamage)
        {
            if (col.transform.parent.gameObject.GetComponent<SpikeTrap>() == null || col.transform.parent.gameObject.GetComponent<SpikeTrap>().spikesUp)
            {
                //Console.WriteLine("AAAAAA");
                TakeDamage(col, false);
                canTakeDamage = false;
            }
        }
    }

    private void TakeDamage(Collider enemy, bool canBlock)
    {
        // If attack is not blocked or parried, take full damage, if blocked, take a percentage of the damage,
        if (!attackBlocked || !canBlock)
        {
            //Full damage
            GetComponent<PlayerValues>().currentHealth -= CalculateDamage(enemy, false);
            GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().hurtSound);
        }
        else
        {
            //Blocked damage
            GetComponent<PlayerValues>().currentHealth -= CalculateDamage(enemy, true);
            attackBlocked = false;
            GetComponent<PlayerController>().StopBlocking();

            //Reduce durability on weapon
            GetComponent<PlayerController>().equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability -= GetComponent<PlayerController>().weaponDurabilityLossBlock;

            //TODO figure out blocking bug
            GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().blockSound);
            //GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().hurtSound);
        }

        canTakeDamage = false;
        GetComponent<PlayerController>().DontTakeDamage();

        // Check for player death
        if (GetComponent<PlayerValues>().currentHealth <= 0)
        {
            PlayerDeath();
        }

        hurtFlash.enabled = true;
        coroutine = HurtFlashDisappear(hurtFlashTime);
        StartCoroutine(coroutine);

        //Knock player backward (relative to the enemy, not the player)
        if (enemy.GetComponent<Enemy>() != null)
        {
            GetComponent<PlayerController>().KnockBack(enemy.gameObject.transform);
        }
        else if (enemy.GetComponent<EnemyAttackDisjoint>() != null)
        {
            GetComponent<PlayerController>().KnockBack(enemy.GetComponent<EnemyAttackDisjoint>().enemyCasterClass.gameObject.transform);
        }

        // Play player getting hurt sound
        GetComponent<PlayerController>().audioSource.pitch = 1;
    }

    private void PlayerDeath()
    {
        //For now, just restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
        Destroy(GameObject.Find("Canvas"));
    }

    public IEnumerator LoadScene(SceneChange sceneChange)
    {
        if (sceneChange.loadingScreen != null)
        {
            GameObject loadingScreen = Instantiate(sceneChange.loadingScreen, Vector3.zero, Quaternion.identity);
            loadingScreen.transform.parent = GameObject.Find("Canvas").transform;
        }

        GetComponent<PlayerController>().waiting = true;

        //SceneManager.LoadScene(sceneChange.nextScene.name);
        SceneManager.LoadScene("Dungeon Test 1");

        yield return new WaitForSeconds(sceneChange.loadTime);

        //Destroy(duplicatePlayer);
        //Destroy(loadingScreen);

        transform.position = sceneChange.playerSpawnPos;
        //transform.rotation = spawnRot; 

        GetComponent<PlayerController>().waiting = false;

        //Debug
    }

    private int CalculateDamage(Collider col, bool blocked)
    {
        int totalDamage = 0;

        if (col != null)
        {
            totalDamage = col.GetComponent<Collision>().damageValue(col);

            if (blocked)
            {
                totalDamage = (int)(totalDamage * blockDefenseFactor);
            }
        }

        return totalDamage;
    }

    public void CoinPickup(int coins)
    {
        GetComponent<PlayerController>().BronzeCollect(coins);
        GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().coinPickupSound);
    }

    IEnumerator HurtFlashDisappear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        hurtFlash.enabled = false;
    }
}
