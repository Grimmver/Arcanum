using System.Collections;
using UnityEngine;

public class BoneBerserker : Skeleton
{
	[SerializeField] private int maxAttackCount = 10;

	public override void TakeTurn()
	{
		if (statusEffectManager.IsStunned())
			StartCoroutine(SkipTurnCoroutine());
		else
			StartCoroutine(AttackUntilDamage(Player.Instance));
	}

	private IEnumerator SkipTurnCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		InvokeTurnFinished();
	}

	private IEnumerator AttackUntilDamage(Character target)
	{
		int attempt = 0;

		while (target != null && !target.IsDying() && !target.IsDead() && attempt < maxAttackCount)
		{
			int damage = Random.Range(minAttackDamage, maxAttackDamage + 1);
			int dealt = 0;

			if (statusEffectManager.IsTricked())
			{
				dealt = DealDamage(this, new Damage(damage, damageType, this));
			}
			else
			{
				dealt = DealDamage(target, new Damage(damage, damageType, this));
			}

			AudioManager.Instance?.PlaySound(attackSound);

			if (targetAnimator != null)
			{
				targetAnimator.SetTrigger("Attack");
				while (targetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Berserk_attack"))
				{
					yield return null;
				}
			}

			if (dealt > 0)
			{
				yield return new WaitForSeconds(0.5f);
				break;
			}

			attempt++;
			yield return new WaitForSeconds(0.5f);
		}

		InvokeTurnFinished();
	}
}
