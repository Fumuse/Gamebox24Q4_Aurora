using JetBrains.Annotations;

public interface IAction
{
    public void Execute([CanBeNull] ActionSettings settings);
}