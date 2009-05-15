using System.Windows.Input;

namespace LinqTwit.Infrastructure
{
    public class MenuViewModel
    {
        public MenuViewModel(string text, ICommand command)
        {
            Text = text;
            Command = command;
        }

        public string Text { get; private set; }
        public ICommand Command { get; private set; }
    }
}
