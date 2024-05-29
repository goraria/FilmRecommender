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

namespace FilmRecommender.Panel {
    /// <summary>
    /// Interaction logic for SideBar.xaml
    /// </summary>
    public partial class SideBar : UserControl {
        public SideBar() {
            InitializeComponent();
        }

        public event Action NavigateToHomeRequested;
        public event Action NavigateToRecommendRequested;
        public event Action NavigateToOrderRequested;
        public event Action NavigateToManageRequested;
        public event Action NavigateToAccountRequested;

        private void NavigateToHome(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToHomeRequested?.Invoke();
        }

        private void NavigateToRecommend(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToRecommendRequested?.Invoke();
        }

        private void NavigateToOrder(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToOrderRequested?.Invoke();
        }

        private void NavigateToManage(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToManageRequested?.Invoke();
        }

        private void NavigateToAccount(object sender, System.Windows.RoutedEventArgs e) {
            NavigateToAccountRequested?.Invoke();
        }
    }
}
