using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class PlayerCollisions : MonoBehaviour
{

    public bool canTakeDamage = true;
    public bool attackBlocked;
    public bool attackParried;
    public float blockDefenseFactor = 0.65f;
    public float enemyColDelay = 0.00001f;
    public Image hurtFlash;
    public float hurtFlashTime = 0.1f;
    private IEnumerator coroutine;

    private void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Enemy":

                //TODO: Only hurt player when attacking. However, right now is buggy
                //if (col.GetComponent<Enemy>().enemyAttacking)
                //{
                coroutine = EnemyCollision(col, enemyColDelay);
                StartCoroutine(coroutine);
                //}

                break;

            case "Enemy Projectile":
                if (canTakeDamage)
                {
                    if (col.GetComponent<EnemyProjectile>() != null)
                        col.GetComponent<EnemyProjectile>().EnemyCasterCanFire();

                    else if (col.GetComponent<EnemyAOEAttack>() != null)
                    {
                        col.GetComponent<EnemyAOEAttack>().EnemyCasterCanFire();
                        GetComponent<PlayerController>().KnockBack(col.GetComponent<EnemyAOEAttack>().enemyCasterClass.gameObject.transform);
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
                        col.GetComponent<EnemyAOEAttack>().enemyCasterClass.GetComponent<Enemy>().EnemyKnockBack(gameObject, true);

                        GetComponent<PlayerController>().audioSource.pitch = 1;
                        GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().parrySound);
                    }
                }
                break;

            case "Trap":
                if (canTakeDamage)
                {
                    TakeDamage(col, false);
                }

                break;

            case "Detection Radius Enter":
                Debug.Log("Entered detection radius!");
                col.GetComponent<DetectionRadius>().PlayerEntered();
                break;

            // Load into next scene
            case "Scene Transition":
    
                StartCoroutine(LoadScene(col.GetComponent<SceneChange>()));

                break;

            // case "Enemy Wall":
            //     //Ignore the collision of any 'enemy wall' objects
            //     Physics.IgnoreCollision(GetComponent<PlayerController>().controller, col.GetComponent<Collider>());
            // break;


            default:

                break;
        }
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

    private IEnumerator EnemyCollision(Collider col, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        //Take damage from enemy
        if (canTakeDamage)
        {
            //TODO: make it that player can't parry enemy while they're winding up

            //BUG TBF: enemy hitting parry and player hitbox causes the hitbox collision to return a parry, but 
            //since it happens same time as enemy hitting player hitbox, seems to not be ready by playercontroller, causing a 'false parry'

            //if (col.GetComponent<Enemy>().enemyAttacking)
            //{
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
                }
                else
                {
                    TakeDamage(col, true);
                }
            }

            attackParried = false;

            canTakeDamage = false;

            GetComponent<PlayerController>().DontTakeDamage();
            //}

        }

        col.GetComponent<Enemy>().Wait();
    }

    private void TakeDamage(Collider enemy, bool canBlock)
    {
        // If attack is not blocked or parried, take full damage, if blocked, take a percentage of the damage,
        if (!attackBlocked || !canBlock)
        {
            //Full damage
            GetComponent<PlayerValues>().currentHealth -= CalculateDamage(enemy, false);
        }
        else
        {
            //Blocked damage
            GetComponent<PlayerValues>().currentHealth -= CalculateDamage(enemy, true);
            attackBlocked = false;
            GetComponent<PlayerController>().StopBlocking();

            //Reduce durability on weapon
            GetComponent<PlayerController>().equippedWeapon.GetComponent<PlayerWeaponValues>().currentWeaponDurability -= GetComponent<PlayerController>().weaponDurabilityLossBlock;
        }

        // Player death
        if (GetComponent<PlayerValues>().currentHealth <= 0)
        {
            PlayerDeath();
        }

        hurtFlash.enabled = true;
        coroutine = HurtFlashDisappear(hurtFlashTime);
        StartCoroutine(coroutine);

        //Knock player backward (relative to the enemy, not the player)
        if (enemy.gameObject != null)
            GetComponent<PlayerController>().KnockBack(enemy.gameObject.transform);
        else if (enemy.GetComponent<EnemyAOEAttack>() != null)
            GetComponent<PlayerController>().KnockBack(enemy.GetComponent<EnemyAOEAttack>().enemyCasterClass.gameObject.transform);

        GetComponent<PlayerController>().audioSource.pitch = 1;
        GetComponent<PlayerController>().audioSource.PlayOneShot(GetComponent<PlayerController>().hurtSound);
    }

    private void PlayerDeath()
    {
        //For now, just restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
        Destroy(GameObject.Find("Canvas"));
    }

    IEnumerator LoadScene(SceneChange sceneChange)
    {
        GameObject loadingScreen = Instantiate(sceneChange.loadingScreen, Vector3.zero, Quaternion.identity);
        loadingScreen.transform.parent = GameObject.Find("Canvas").transform;
        GetComponent<PlayerController>().waiting = true;

        SceneManager.LoadScene(sceneChange.nextScene.name);

        yield return new WaitForSeconds(sceneChange.loadTime);

        //Destroy(duplicatePlayer);
        Destroy(loadingScreen);

        transform.position = sceneChange.playerSpawnPos;
        //transform.rotation = spawnRot; 

        GetComponent<PlayerController>().waiting = false;
    }

    private int CalculateDamage(Collider harmfulEntity, bool blocked)
    {
        int totalDamage = 0;
        if (harmfulEntity != null)
        {
            switch (harmfulEntity.tag)
            {
                case "Enemy":
                    totalDamage = (int)harmfulEntity.GetComponent<Enemy>().attackDamage;
                    break;

                case "Enemy Projectile":
                    if (harmfulEntity.GetComponent<EnemyProjectile>() != null)
                        totalDamage = (int)harmfulEntity.GetComponent<EnemyProjectile>().projectileDamage;
                    else if (harmfulEntity.GetComponent<EnemyAOEAttack>() != null)
                        totalDamage = (int)harmfulEntity.GetComponent<EnemyAOEAttack>().projectileDamage;
                    break;

                case "Trap":
                    totalDamage = harmfulEntity.GetComponent<Trap>().damage;
                    break;

                default:
                    Debug.Log("Correct gameObject tag not found! Check spelling or that it has been assigned to the object");
                    break;
            }

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
