
using System.Runtime.CompilerServices;
using System.ComponentModel;


namespace wpf.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ?PropertyChanged; 

        public void OnPropertyChanged([CallerMemberName]string prop = "")  
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(prop));
        }
    }
    public abstract class EntityViewModel : BaseViewModel
    {
        public string name {get;}

        protected EntityViewModel(string name)
        {
            this.name = name;
        }
    }
    public sealed class ViewModel : EntityViewModel
    {
        public ViewModel(string name) : base(name){}
    }
}