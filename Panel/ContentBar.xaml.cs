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
using FilmRecommender.Panel.Account;
using FilmRecommender.Panel.Home;
using FilmRecommender.Panel.Manage;
using FilmRecommender.Panel.Order;
using FilmRecommender.Panel.Recommend;

namespace FilmRecommender.Panel {
    /// <summary>
    /// Interaction logic for ContentBar.xaml
    /// </summary>
    public partial class ContentBar : UserControl {
        public ContentBar() {
            InitializeComponent();
            MainFrame.Navigate(new FilmRecommender.Panel.Home.Home());
        }

        private void NavigateToHome() {
            MainFrame.Navigate(new FilmRecommender.Panel.Home.Home());
        }

        private void NavigateToRecommend() {
            MainFrame.Navigate(new FilmRecommender.Panel.Recommend.Recommend());
        }

        private void NavigateToOrder() {
            MainFrame.Navigate(new FilmRecommender.Panel.Order.Order());
        }

        private void NavigateToManage() {
            MainFrame.Navigate(new FilmRecommender.Panel.Manage.Manage());
        }

        private void NavigateToAccount() {
            MainFrame.Navigate(new FilmRecommender.Panel.Account.Account());
        }
    }
}
