using JetBrains.Annotations;

public interface IAction
{
    public void Execute(ActionSettings settings);
}