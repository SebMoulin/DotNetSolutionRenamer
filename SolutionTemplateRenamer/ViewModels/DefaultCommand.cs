using System;
using System.Windows.Input;
using SolutionTemplateRenamer.Annotations;

namespace SolutionTemplateRenamer.ViewModels
{
    public class DefaultCommand : ICommand
    {
        private readonly Action<object> _actionToExecute;
        private readonly Func<object, bool> _canExecutePredicate;

        public DefaultCommand([NotNull] Action<object> actionToExecute, [NotNull] Func<object,bool> canExecutePredicate )
        {
            if (actionToExecute == null) throw new ArgumentNullException(nameof(actionToExecute));
            if (canExecutePredicate == null) throw new ArgumentNullException(nameof(canExecutePredicate));
            _actionToExecute = actionToExecute;
            _canExecutePredicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecutePredicate(parameter);
        }

        public void Execute(object parameter)
        {
            _actionToExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}