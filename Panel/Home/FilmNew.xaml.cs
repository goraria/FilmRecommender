using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore;
using FilmRecommender.Panel.Recommend;
using FilmRecommender.Panel.Home;
using System.IO;

namespace FilmRecommender.Panel.Home {
    /// <summary>
    /// Interaction logic for FilmNew.xaml
    /// </summary>
    public partial class FilmNew : Page {
        public FilmNew() {
            InitializeComponent();
            FilmNewLoad();
        }

        //List<System.Windows.Controls.Image> ImageList;
        public List<Image> ImageList { get; set; } = new List<Image>();

        private void CreateImageList() {
            ImageList.Add(FilmImage0);
            ImageList.Add(FilmImage1);
            ImageList.Add(FilmImage2);
            ImageList.Add(FilmImage3);
            ImageList.Add(FilmImage4);
            ImageList.Add(FilmImage5);
            ImageList.Add(FilmImage6);
            ImageList.Add(FilmImage7);
            ImageList.Add(FilmImage8);
            ImageList.Add(FilmImage9);
        }

        private string ImagePath(string filename) {
            filename += ".jpg";
            try {
                // Lấy đường dẫn thư mục gốc của ứng dụng (thường là thư mục chứa tập tin exe của ứng dụng)
                //string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //string rootDirectory = Directory.GetCurrentDirectory();
                string rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
                // Xây dựng đường dẫn tương đối tới tập tin mong muốn
                string relativePath = Path.Combine("img", filename);
                // Kết hợp đường dẫn thư mục gốc với đường dẫn tương đối để tạo đường dẫn hoàn chỉnh
                string fullPath = Path.Combine(rootDirectory, relativePath);

                // Sử dụng đường dẫn hoàn chỉnh để thực hiện các thao tác với tập tin
                if (File.Exists(fullPath)) {
                    return fullPath;
                } else {
                    return "";
                }
            } catch (Exception ex) {
                MessageBox.Show($"Lỗi khi lấy đường dẫn ảnh: {ex.Message}");
                return "";
            }
        }

        private void SetImageSource(Image image, string imagePath) {
            try {
                image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            } catch (Exception ex) {
                // Xử lý lỗi khi không thể tải ảnh
                MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            }
        }

        public void FilmNewLoad() {
            CreateImageList();
            using (var context = new CinemaContext()) {
                var queryHotest = context.Phims
                    .Where(p => p.ngayramat >= DateTime.Now.AddMonths(-2) && p.ngayramat <= DateTime.Now)
                    .OrderByDescending(p => p.ngayramat)
                    .Take(10)
                    .ToList();

                //foreach (var item in queryHotest) {
                //    SetImageSource(ImagePath(item.id_phim.ToString()));
                //}

                try {
                    for (int i = 0; i < queryHotest.Count; i++) {
                        if (queryHotest[i] == null) {
                            break;
                        }
                        SetImageSource(ImageList[i], ImagePath(queryHotest[i].id_phim.ToString()));
                    }
                } catch (Exception e) {
                    MessageBox.Show($"Lỗi khi load ảnh: {e.Message}");
                }
            }
        }
    }
}
