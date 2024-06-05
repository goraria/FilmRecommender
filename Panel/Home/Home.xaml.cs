using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FilmRecommender.Panel.Home;

namespace FilmRecommender.Panel.Home {
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page {
        public Home() {
            InitializeComponent();
            NavigateToHotRequested += () => NavigateToHot();
            NavigateToNewRequested += () => NavigateToNew();
            HomeFrame.Navigate(new FilmRecommender.Panel.Home.FilmHot());
        }

        public event Action NavigateToHotRequested;
        public event Action NavigateToNewRequested;

        private void NavigateToHot(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToHotRequested?.Invoke();
        }

        private void NavigateToNew(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToNewRequested?.Invoke();
        }

        private void NavigateToHot() {
            HomeFrame.Navigate(new FilmRecommender.Panel.Home.FilmHot());
        }

        private void NavigateToNew() {
            HomeFrame.Navigate(new FilmRecommender.Panel.Home.FilmNew());
        }
    }
}
