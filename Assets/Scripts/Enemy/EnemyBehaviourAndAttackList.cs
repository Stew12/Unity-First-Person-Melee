using UnityEngine;

public class EnemyBehaviourAndAttackList : MonoBehaviour
{
    Vector3 trajectory = Vector3.zero;

    public void RoamBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {

    }

    public void ChaseBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
        switch (enemyType)
        {
            //SKELETON CHASE BEHAVIOR: PURSUE PLAYER 
            case EnemyType.SKELETON:
               FollowPlayerBasic(enemyClass, enemyGameObject);
            break;

            case EnemyType.SORCERESS:
                //Do Nothing (doesn't chase player)
            break;

            default:

            break;

        }

        if (enemyClass.distanceFromPlayer <= enemyClass.aggroAttackDistance && enemyClass.attackCoolDownTime <= 0)
        {
            enemyClass.enemyState = EnemyState.ATTACKING;
        }

    }

    public void AttackBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject, GameObject projectile)
    {
        switch (enemyType)
        {
            //SKELETON ATTACKS: RUSH AT PLAYER 
            case EnemyType.SKELETON:
                //TODO: set trajectory towards player then move ONLY on that trqjectory for rest of attack
               BasicPhysicalAttack(enemyClass, enemyGameObject);
            break;

            case EnemyType.SORCERESS:
                BasicRangedAttack(enemyClass, enemyGameObject, projectile);
            break;

            default:

            break;
        }
    }

    //Pursues the player at a designated speed
    private void FollowPlayerBasic(Enemy enemyClass, GameObject enemyGameObject)
    {
         //When chasing, move towards player on X and Z axis
        enemyClass.GetComponent<Enemy>().enemyController.Move(new Vector3(enemyGameObject.transform.forward.x * enemyClass.chaseSpeed * Time.deltaTime, 0, enemyGameObject.transform.forward.z * enemyClass.chaseSpeed * Time.deltaTime)); 
    }

    //the enemy moves quickly towards the player
    private void BasicPhysicalAttack(Enemy enemyClass, GameObject enemyGameObject)
    {
        if (!enemyClass.attackTrajectorySet)
        {
            //Point towards player pos upon spawning
            trajectory = new Vector3(enemyGameObject.transform.forward.x * enemyClass.attackMoveTowardsSpeed * Time.deltaTime, 0, enemyGameObject.transform.forward.z * enemyClass.attackMoveTowardsSpeed * Time.deltaTime);
            enemyClass.attackTrajectorySet = true;
        }

        enemyClass.enemyController.Move(trajectory); 
    }

    private void BasicRangedAttack(Enemy enemyClass, GameObject enemyGameObject, GameObject projectile)
    {
        if (enemyClass.canFireProjectile)
        {
            enemyClass.canFireProjectile = false;
            GameObject spawnedProjectile = Instantiate(projectile, enemyGameObject.transform.position, enemyGameObject.transform.rotation);
            spawnedProjectile.GetComponent<EnemyProjectile>().enemyCasterClass = enemyClass;
        }
    }
}
