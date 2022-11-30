public class LaserCapacity : ActiveCapacity
{
    private LaserCapacitySO laserSO;
    public override void TryCast()
    { 
        base.TryCast();
    }

    public override void CastCapacity()
    {
        laserSO = (LaserCapacitySO)activeCapacitySo;
        
        base.CastCapacity();
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }

    public override void SendFeedback()
    {
        base.SendFeedback();
    }

    public override void CooldownCapacity()
    {
        base.CooldownCapacity();
    }
    public override void CastTimer()
    {
        base.CastTimer();
    }
}
