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

namespace FilmRecommender.Panel.Home {
    /// <summary>
    /// Interaction logic for FilmImage.xaml
    /// </summary>
    public partial class FilmImage : UserControl {
        public FilmImage() {
            InitializeComponent();
        }


    }

    public class FilmImageTop {
        public string id { get; set; }
        public System.Windows.Controls.Image img { get; set; }
    }
}
