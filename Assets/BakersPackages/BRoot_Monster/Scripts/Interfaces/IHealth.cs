
public interface IHealth
{
    public void TakeDamage(int amount);
    public void Heal(int amount);

    public void ResetHealth();
    public int GetHealth();
    public int GetHealthMax();
}
