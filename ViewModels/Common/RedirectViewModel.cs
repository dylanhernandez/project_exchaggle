namespace Exchaggle.ViewModels.Common
{
    public class RedirectViewModel
    {
        public RedirectViewModel(string action, string controller, string parameter)
        {
            Action = action;
            Controller = controller;
            Parameter = parameter;
        }

        public string Action { get; }
        public string Controller { get; }
        public string Parameter { get; }
    }
}