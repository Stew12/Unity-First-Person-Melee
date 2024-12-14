using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourAndAttackList : MonoBehaviour
{

    public void RoamBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {

    }

    public void ChaseBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
        switch (enemyType)
        {
            //SKELETON CHASE BEHAVIOR: PURSUE PLAYER 
            case EnemyType.SKELETON:
               FollowPlayerBasic(enemyType, enemyClass, enemyGameObject);
            break;

            case EnemyType.SORCERESS:

            break;

            default:

            break;

        }

    }

    public void AttackBehaviourList(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
        switch (enemyType)
        {
            //SKELETON ATTACKS: RUSH AT PLAYER 
            case EnemyType.SKELETON:
               BasicAttack(enemyType, enemyClass, enemyGameObject);
            break;

            case EnemyType.SORCERESS:

            break;

            default:

            break;
        }
    }

    //Pursues the player at a designated speed
    private void FollowPlayerBasic(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
         //When chasing, move towards player on X and Z axis
        enemyGameObject.transform.position += new Vector3(enemyGameObject.transform.forward.x * enemyClass.chaseSpeed * Time.deltaTime, 0, enemyGameObject.transform.forward.z * enemyClass.chaseSpeed * Time.deltaTime); 

        if (enemyClass.distanceFromPlayer <= enemyClass.aggroAttackDistance && enemyClass.attackCoolDownTime <= 0)
        {
            enemyClass.enemyState = EnemyState.ATTACKING;
        }
    }

    //the enemy moves quickly towards the player
    private void BasicAttack(EnemyType enemyType, Enemy enemyClass, GameObject enemyGameObject)
    {
        enemyGameObject.transform.position += new Vector3(enemyGameObject.transform.forward.x * enemyClass.attackMoveTowardsSpeed * Time.deltaTime, 0, enemyGameObject.transform.forward.z * enemyClass.attackMoveTowardsSpeed * Time.deltaTime); 
    }
}
