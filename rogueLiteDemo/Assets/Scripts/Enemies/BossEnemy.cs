using UnityEngine;

public class BossEnemy : EnemyBase
{
    public float specialCooldown = 4f;
    private float timer;

    protected override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (timer >= specialCooldown)
        {
            SpecialAttack();
            timer = 0f;
        }
    }

    void SpecialAttack()
    {
        Debug.Log("Boss special attack");
    }
}
