using System.Windows;

namespace VECTO3GUI2020.Views
{
    /// <summary>
    /// Interaktionslogik für Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public string TestProperty { get; set; } = "hi";


        private enum TestEnum
        {
            Hallo = 0, Welt, ich, bin, eine, Aufzählung
        }

    

        public Test()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
