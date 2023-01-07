public class LaserCapacity : ActiveCapacity
{
    public LaserCapacitySO laserSO;

    public override void InitializeCapacity(ActiveCapacitySO so)
    {
        base.InitializeCapacity(so);
        laserSO = (LaserCapacitySO)so;
    }

    public override void Cast()
    {
        base.Cast();
    }

    public override void CastRoutine()
    {
        base.CastRoutine();
    }

    public override void Active()
    {
        base.Active();
        owner.OnCastEnded?.Invoke(Capacities.MIMI_Laser);
    }
    

    public override void Cooldown()
    {
       base.Cooldown();
        
    }
}
