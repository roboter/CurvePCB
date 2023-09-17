using ReactiveUI;
using System.Diagnostics;
using System.Reactive;

namespace CurvePCB.Avalonia.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public string ButtonText => "Hello World!";

        public MainViewModel()
        {
            DoTheThing = ReactiveCommand.Create<string>(RunTheThing);
        }

        public ReactiveCommand<string, Unit> DoTheThing { get; }

        void RunTheThing(string parameter)
        {
            // Code for executing the command here.
            Debug.WriteLine(parameter);
        }
    }
}
