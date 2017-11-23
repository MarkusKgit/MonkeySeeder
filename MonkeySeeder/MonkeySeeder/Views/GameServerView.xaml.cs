using System.Windows.Controls;

namespace MonkeySeeder.Views
{
    /// <summary>
    /// Interaction logic for GameServerView.xaml
    /// </summary>
    public partial class GameServerView : UserControl
    {
        public GameServerView()
        {
            InitializeComponent();
        }

        private void expand_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var header = expand.Template.FindName("HeaderSite", expand) as Control;
            var height = e.NewSize.Height;
            if (header != null)
                height = height - header.ActualHeight;
            gridColorzone.Height = height;
        }
    }
}