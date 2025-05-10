namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal class ListPageStack<T> where T : class
{
    private readonly Stack<T> _modelStack = [];
    public T? Model => _modelStack.Count > 0 ? _modelStack.Peek() : null;

    public void PushModel(T model)
    {
        _modelStack.Push(model);
    }

    public T? PopModel()
    {
        return _modelStack.Count > 0 ? _modelStack.Pop() : null;
    }

    public void ClearStack()
    {
        _modelStack.Clear();
    }
}