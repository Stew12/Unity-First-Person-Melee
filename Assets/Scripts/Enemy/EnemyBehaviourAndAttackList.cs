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

    EnemyType enemyType;
    Enemy enemyClass;
    GameObject enemyGameObject;
    GameObject projectile;
    GameObject AOE;
    GameObject weaponSlash;

    [HideInInspector] public int attackChoice = -1;

    private float slashDistance = -0.8f;
    private float slashOffset = 0.2f;

    public void RoamBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {

    }

    public void ChaseBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
        this.enemyType = enemyType;
        this.enemyClass = enemyClass;
        this.enemyGameObject = enemyGameObject;

        //enemyClass.audioSource.pitch = 1;
        //enemyClass.audioSource.PlayOneShot(enemyClass.enemyAlert);

        switch (enemyType)
        {
            //SKELETON CHASE BEHAVIOR: PURSUE PLAYER 
            case EnemyType.SKELETON:
               FollowPlayerBasic();
            break;

            case EnemyType.SORCERESS:
                //Do Nothing (doesn't chase player)
            break;

            //IMP CHASE BEHAVIOR: PURSUE PLAYER 
            case EnemyType.IMP:
               FollowPlayerBasic();
            break;

            default:

            break;

        }

        if (enemyClass.distanceFromPlayer <= enemyClass.aggroAttackDistance && enemyClass.attackCoolDownTime <= 0)
        {
            enemyClass.enemyState = EnemyState.ATTACKING;
        }

    }

    public void AttackBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject, GameObject projectile, GameObject AOE, GameObject weaponSlash)
    {
        this.enemyType = enemyType;
        this.enemyClass = enemyClass;
        this.enemyGameObject = enemyGameObject;
        this.projectile = projectile;
        this.AOE = AOE;
        this.weaponSlash = weaponSlash;

        switch (enemyType)
        {
            //SKELETON ATTACKS: RUSH AT PLAYER 
            case EnemyType.SKELETON:
                //TODO: set trajectory towards player then move ONLY on that trqjectory for rest of attack
                EnemyAttackListSelect(EnemyAttack.BASICPHYSICAL);
                break;

            //SKELETON ATTACKS: SHOOT FIREBALL AT PLAYER 
            case EnemyType.SORCERESS:
                EnemyAttackListSelect(EnemyAttack.BASICRANGED);
                break;


            case EnemyType.IMP:
                EnemyAttackRandomChoice(new EnemyAttack[] { EnemyAttack.BASICPHYSICAL, EnemyAttack.BASICAOE });

                break;

            default:

                break;
        }
    }

    //Pursues the player at a designated speed
    private void FollowPlayerBasic()
    {
         //When chasing, move towards player on X and Z axis
        enemyClass.GetComponent<Enemy>().enemyController.Move(new Vector3(-enemyGameObject.transform.forward.x * enemyClass.chaseSpeed * Time.deltaTime, 0, -enemyGameObject.transform.forward.z * enemyClass.chaseSpeed * Time.deltaTime)); 
    }

    //the enemy moves quickly towards the player
    private void BasicPhysicalAttack()
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

    private void BasicRangedAttack()
    {
        if (enemyClass.canFireProjectile)
        {
            enemyClass.canFireProjectile = false;
            GameObject spawnedProjectile = Instantiate(projectile, enemyClass.projectileSpawn.transform.position, Quaternion.identity);
            spawnedProjectile.GetComponent<EnemyProjectile>().enemyCasterClass = enemyClass;
        }
    }

    private void BasicAOEAttack()
    {
        if (enemyClass.canFireProjectile)
        {
            enemyClass.canFireProjectile = false;

            GameObject spawnedAOE = Instantiate(AOE, enemyGameObject.transform.position, enemyGameObject.transform.rotation);
            spawnedAOE.GetComponent<EnemyAOEAttack>().enemyCasterClass = enemyClass;
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
                slashPos = new Vector3(slashPos.x + slashOffset, slashPos.y + slashOffset, slashPos.z);

                GameObject spawnedSlash = Instantiate(weaponSlash, slashPos, enemyGameObject.transform.rotation);
                spawnedSlash.transform.parent = enemyGameObject.transform;

                spawnedSlash.GetComponent<EnemyAOEAttack>().enemyCasterClass = enemyClass;
                spawnedSlash.GetComponent<EnemyAOEAttack>().rangeTime = enemyClass.attackDuration;
                spawnedSlash.GetComponent<EnemyAOEAttack>().projectileDamage = enemyClass.attackDamage;
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

    private void EnemyAttackRandomChoice(EnemyAttack[] enemyAttacks)
    {
        if (attackChoice == -1)
        {
            int maxRandom = enemyAttacks.Length;

            attackChoice = Random.Range(0, maxRandom);
        }
        
        EnemyAttackListSelect(enemyAttacks[attackChoice]);
    }

    private void EnemyAttackListSelect(EnemyAttack enemyAttack)
    {
        switch (enemyAttack)
        {
            case EnemyAttack.BASICPHYSICAL:
                BasicPhysicalAttack();
            break;

            case EnemyAttack.BASICRANGED:
                BasicRangedAttack();
            break;

            case EnemyAttack.BASICAOE:
                BasicAOEAttack();
            break;

        }
    }
}
