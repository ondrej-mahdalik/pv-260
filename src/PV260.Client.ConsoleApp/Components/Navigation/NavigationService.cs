using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;

namespace PV260.Client.ConsoleApp.Components.Navigation;

internal class NavigationService : INavigationService
{
    private readonly Stack<IRenderableComponent> _stack = new();
    public IRenderableComponent Current => _stack.Peek();

    public IRenderableComponent? LastNavigationComponent =>
        _stack.FirstOrDefault(component => component is INavigationComponent);

    public bool CanGoBack => _stack.Count > 1;

    public void Push(IRenderableComponent component)
    {
        if (!_stack.Any())
        {
            _stack.Push(component);
            return;
        }

        var isComponentWithIdenticalType = component.GetType() == Current.GetType();

        if (!isComponentWithIdenticalType)
        {
            _stack.Push(component);
        }
    }

    public void Pop()
    {
        if (_stack.Count > 1)
        {
            _stack.Pop();
        }
    }
}