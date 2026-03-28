namespace MobaRoguelike.Core.Abilities
{
    public interface IDamageable
    {
        void TakeDamage(float amount, int sourceId);
    }
}
