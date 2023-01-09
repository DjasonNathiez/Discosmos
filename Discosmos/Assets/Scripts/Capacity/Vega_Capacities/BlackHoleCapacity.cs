public class BlackHoleCapacity : ActiveCapacity
{
    public BlackHoleCapacitySO bhSO;
    
    public override void InitializeCapacity(ActiveCapacitySO so)
    {
        base.InitializeCapacity(so);
        bhSO = (BlackHoleCapacitySO)so;
    }
    
    public override void Active()
    {
        base.Active();
        owner.OnCastEnded?.Invoke(Capacities.VEGA_Blackhole);
    }
}
