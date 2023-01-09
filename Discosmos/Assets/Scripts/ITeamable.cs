using Toolbox.Variable;

public interface ITeamable
{
   public Enums.Teams CurrentTeam();
   public void SetTeam(Enums.Teams team);
}
