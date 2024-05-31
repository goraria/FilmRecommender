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
using FilmRecommender.Panel;
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
            //MainFrame.Navigate(new FilmRecommender.Panel.Recommend.Recommend());
            //MainFrame.Navigate(new FilmRecommender.Panel.Recommend.RebuildGrid());
            MainFrame.Navigate(new FilmRecommender.Panel.Recommend.RetrainGrid());
        }

        public void NavigateToHome() { // private
            MainFrame.Navigate(new FilmRecommender.Panel.Home.Home());
        }

        public void NavigateToRecommend() {
            MainFrame.Navigate(new FilmRecommender.Panel.Recommend.Recommend());
        }

        public void NavigateToOrder() {
            MainFrame.Navigate(new FilmRecommender.Panel.Order.Order());
        }

        public void NavigateToManage() {
            MainFrame.Navigate(new FilmRecommender.Panel.Manage.Manage());
        }

        public void NavigateToAccount() {
            MainFrame.Navigate(new FilmRecommender.Panel.Account.Account());
        }
    }
}
