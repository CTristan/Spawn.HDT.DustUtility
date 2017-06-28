namespace Spawn.HDT.Build.Action
{
    interface IAction<T>
    {
        bool Execute(T parameters);
    }
}
