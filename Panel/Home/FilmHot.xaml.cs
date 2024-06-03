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
using Microsoft.EntityFrameworkCore;
using FilmRecommender.Panel.Recommend;

namespace FilmRecommender.Panel.Home {
    /// <summary>
    /// Interaction logic for FilmHot.xaml
    /// </summary>
    public partial class FilmHot : Page {
        public FilmHot() {
            InitializeComponent();
        }

        public void FilmNewLoad() {
            using (var context = new CinemaContext()) {
                var queryHotest = context.Phims
                    .Where(p => p.ngayramat >= DateTime.Now.AddMonths(-2) && p.ngayramat <= DateTime.Now)
                    .OrderByDescending(p => p.sove)
                    .Take(10)
                    .ToList();
            }

        }
    }
}
