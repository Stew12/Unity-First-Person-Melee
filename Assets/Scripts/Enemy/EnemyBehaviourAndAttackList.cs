using Unity.VisualScripting;
using UnityEngine;

public enum EnemyAttack
    {
        BASICPHYSICAL,
        BASICRANGED,
        BASICAOE,
        RANGEDPILLAR,
        DUPLICATE

    }

public class EnemyBehaviourAndAttackList : MonoBehaviour
{
    Vector3 trajectory = Vector3.zero;
    
    Enemy enemyClass;
    GameObject enemyGameObject;
    GameObject projectile;
    GameObject AOE;
    GameObject weaponSlash;

    private float slashOffset = 0.2f;
    public int attackChoice = -1;

    private float slashDistance = -0.8f;

    public void RoamBehaviourList(Enemy enemyClass, GameObject enemyGameObject)
    {

    }

    public void ChaseBehaviourList(Enemy enemyClass, GameObject enemyGameObject)
    {
        this.enemyClass = enemyClass;
        this.enemyGameObject = enemyGameObject;

        //enemyClass.audioSource.pitch = 1;
        //enemyClass.audioSource.PlayOneShot(enemyClass.enemyAlert);

        if (enemyClass.distanceFromPlayer <= enemyClass.aggroAttackDistance && enemyClass.attackCoolDownTime <= 0)
        {
            enemyClass.enemyState = EnemyState.ATTACKING;
        }

    }

    public void AttackBehaviourList(Enemy enemyClass, GameObject enemyGameObject, GameObject projectile, GameObject AOE, GameObject weaponSlash, float slashOffset)
    {
        this.enemyClass = enemyClass;
        this.enemyGameObject = enemyGameObject;
        this.projectile = projectile;
        this.AOE = AOE;
        this.weaponSlash = weaponSlash;
        this.slashOffset = slashOffset;
    }

    //Pursues the player at a designated speed
    public void FollowPlayerBasic()
    {
         //When chasing, move towards player on X and Z axis
        enemyClass.GetComponent<Enemy>().enemyController.Move(new Vector3(-enemyGameObject.transform.forward.x * enemyClass.chaseSpeed * Time.deltaTime, 0, -enemyGameObject.transform.forward.z * enemyClass.chaseSpeed * Time.deltaTime)); 
    }

    //the enemy moves quickly towards the player
    public void BasicPhysicalAttack()
    {
        if (!enemyClass.attackTrajectorySet)
        {
            //Point towards player pos upon spawning
            trajectory = new Vector3(-enemyGameObject.transform.forward.x * enemyClass.attackMoveTowardsSpeed, 0, -enemyGameObject.transform.forward.z * enemyClass.attackMoveTowardsSpeed);
            enemyClass.attackTrajectorySet = true;
        }

        enemyClass.audioSource.pitch = 1;
        enemyClass.audioSource.PlayOneShot(enemyClass.enemyAttack);
        WeaponSlashEffect();

        enemyClass.enemyController.Move(trajectory * Time.deltaTime); 
    }

    public void BasicRangedAttack()
    {
        if (enemyClass.canFireProjectile)
        {
            enemyClass.canFireProjectile = false;
            GameObject spawnedProjectile = Instantiate(projectile, enemyClass.projectileSpawn.transform.position, Quaternion.identity);
            spawnedProjectile.GetComponent<EnemyProjectile>().enemyCasterClass = enemyClass;

            Debug.Log("Projectile fired!");
        }
    }

    public void BasicAOEAttack()
    {
        if (enemyClass.canFireProjectile)
        {
            enemyClass.canFireProjectile = false;

            GameObject spawnedAOE = Instantiate(AOE, enemyGameObject.transform.position, enemyGameObject.transform.rotation);
            spawnedAOE.GetComponent<EnemyAttackDisjoint>().enemyCasterClass = enemyClass;
        }
    }

    private void WeaponSlashEffect()
    {
        if (weaponSlash != null)
        {
            if (enemyClass.canFireProjectile)
            {
                enemyClass.canFireProjectile = false;

                Vector3 slashPos = enemyGameObject.transform.localPosition - enemyGameObject.transform.forward - enemyGameObject.transform.forward * slashDistance;
                slashPos = new Vector3(slashPos.x, slashPos.y + slashOffset, slashPos.z);

                GameObject spawnedSlash = Instantiate(weaponSlash, slashPos, enemyGameObject.transform.rotation);
                spawnedSlash.transform.parent = enemyGameObject.transform;

                spawnedSlash.GetComponent<EnemyAttackDisjoint>().enemyCasterClass = enemyClass;
                spawnedSlash.GetComponent<EnemyAttackDisjoint>().rangeTime = enemyClass.attackDuration;
                spawnedSlash.GetComponent<EnemyAttackDisjoint>().projectileDamage = enemyClass.attackDamage;
            }
        }
    }

    private void DuplicateSelf()
    {
        //if (enemyClass.canFireProjectile)
        //{
        //  enemyClass.canFireProjectile = false;

        //GameObject spawnedEnemy = Instantiate(AOE, enemyGameObject.transform.position, enemyGameObject.transform.rotation);
        //}
    }

    public EnemyAttack EnemyAttackRandomChoice(EnemyAttack[] enemyAttacks)
    {
        if (attackChoice == -1)
        {
            int maxRandom = enemyAttacks.Length;

            attackChoice = Random.Range(0, maxRandom);
        }

        return enemyAttacks[attackChoice];
    }

}
