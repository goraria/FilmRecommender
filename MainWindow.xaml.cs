using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilmRecommender {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Đăng ký sự kiện điều hướng từ sidebar
            sidebar.NavigateToHomeRequested += () => contentbar.NavigateToHome();
            sidebar.NavigateToRecommendRequested += () => contentbar.NavigateToRecommend();
            sidebar.NavigateToOrderRequested += () => contentbar.NavigateToOrder();
            sidebar.NavigateToManageRequested += () => contentbar.NavigateToManage();
            sidebar.NavigateToAccountRequested += () => contentbar.NavigateToAccount();
        }
    }
}