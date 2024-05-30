using System;
using System.Collections.Generic;
using System.IO;
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
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;

namespace FilmRecommender.Panel.Recommend
{
    /// <summary>
    /// Interaction logic for Recommend.xaml
    /// </summary>
    public partial class Recommend : Page {
        private readonly CinemaContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        //private List<MovieData> ketqua;

        public Recommend() {
            InitializeComponent();
            //LoadImageNames();
            _context = new CinemaContext();
            _mlContext = new MLContext();
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
                return "";
            }
        }

        private void LoadImageNames() {
            // Thêm tên ảnh vào ListBox (cần điều chỉnh đường dẫn ảnh)
            ListBoxKetQua.Items.Add("PM00000020");
            ListBoxKetQua.Items.Add("PM00000021");
            ListBoxKetQua.Items.Add("PM00000022");
        }

        private void ListBoxImages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ListBoxKetQua.SelectedItem != null) {
                string selectedImage = ListBoxKetQua.SelectedItem.ToString();
                SetImageSource(ImagePath(selectedImage));
            }
        }

        private void SetImageSource(string imagePath) {
            try {
                imageDisplay.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            } catch (Exception ex) {
                // Xử lý lỗi khi không thể tải ảnh
                MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            }
        }

        private ITransformer TrainModel(int numberOfTrees) {
            var data = _context.Phims
                .Where(p => p.sove.HasValue && p.theloai != null && p.ngayramat.HasValue)
                .Select(p => new MovieData {
                    NumberOfViewers = p.sove.GetValueOrDefault(),
                    Genre = p.theloai ?? "",
                    ReleaseDateDays = (float)((p.ngayramat.GetValueOrDefault() - new DateTime(2000, 1, 1)).TotalDays),
                    IsNew = (p.ngayramat.GetValueOrDefault() > DateTime.Now.AddMonths(-3)) ? 1 : 0,
                    IsHot = (p.sove.GetValueOrDefault() > 10) ? 1 : 0,
                    Rating = p.sove.GetValueOrDefault(),
                    Label = 1
                }).ToList();

            if (data.Count == 0) {
                throw new InvalidOperationException("Không có dữ liệu để huấn luyện mô hình.");
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var preview = dataView.Preview(maxRows: 10);
            foreach (var row in preview.RowView) {
                foreach (var column in row.Values) {
                    Console.WriteLine($"{column.Key}: {column.Value}");
                }
            }

            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("Genre")
                .Append(_mlContext.Transforms.Concatenate("Features", "NumberOfViewers", "Genre", "ReleaseDateDays", "IsNew", "IsHot", "Rating"))
                .Append(_mlContext.Regression.Trainers.FastForest(new FastForestRegressionTrainer.Options {
                    NumberOfTrees = numberOfTrees,
                    NumberOfLeaves = 20,
                    FeatureColumnName = "Features",
                    LabelColumnName = "Label"
                }));

            try {
                var model = pipeline.Fit(dataView);
                return model;
            } catch (Exception ex) {
                throw new InvalidOperationException("Có lỗi xảy ra khi huấn luyện mô hình: " + ex.Message);
            }
        }

        private void ButtonTimPhim_Click(object sender, RoutedEventArgs e) {
            string? soNguoi = (ComboBoxSoNguoi.SelectedItem as ComboBoxItem)?.Content.ToString();
            //if (!int.TryParse(TextBoxTuoi.Text, out int tuoi)) {
            //    MessageBox.Show("Tuổi không hợp lệ.");
            //    return;
            //}
            int tuoi = int.Parse(TextBoxTuoi.Text);
            string? gioiTinh = (ComboBoxGioiTinh.SelectedItem as ComboBoxItem)?.Content.ToString();
            string? soThich = (ComboBoxSoThich.SelectedItem as ComboBoxItem)?.Content.ToString();
            string? theLoai = (ComboBoxTheLoai.SelectedItem as ComboBoxItem)?.Content.ToString();

            _model = TrainModel(100); // Thêm tham số số lượng cây vào đây, bạn có thể thay đổi số lượng cây tùy ý.

            //try {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MovieData, MoviePrediction>(_model);

            var phimQuery = _context.Phims.AsQueryable();

            //if (tuoi < 5 || tuoi > 95) {
            //    return;
            //}

            // Điều kiện cho số người xem
            if (soNguoi == "Gia đình") {
                phimQuery = phimQuery.Where(p => p.theloai.Contains("Hài hước") || p.theloai.Contains("Hoạt hình"));
            } else if (soNguoi == "Cặp đôi") {
                phimQuery = phimQuery.Where(p => p.theloai.Contains("Tình cảm") || p.theloai.Contains("Hài hước"));
                //if (tuoi < 18 && tuoi > 15) {
                //    phimQuery = phimQuery.Where(p => p.mac != "PG");
                //}
            } else {
                if (tuoi < 15) {
                    phimQuery = phimQuery.Where(p => p.mac != "R" && (p.theloai.Contains("Hài hước") || p.theloai.Contains("Hoạt hình")));
                }
            }

            // Điều kiện cho số người xem
            //if (soNguoi == "Gia đình") {
            //    phimQuery = phimQuery.Where(p => p.theloai.Contains("Hài hước") || p.theloai.Contains("Hoạt hình"));
            //} else if (soNguoi == "Cặp đôi") {
            //    phimQuery = phimQuery.Where(p => p.theloai.Contains("Tình cảm"));
            //} else if (soNguoi == "1 mình") {
            //    phimQuery = phimQuery.Where(p => p.theloai.Contains("Phim mới"));
            //} else if (soNguoi == "Nhóm bạn") {
            //    phimQuery = phimQuery.Where(p => p.theloai.Contains("Phim hot"));
            //} else if (tuoi < 18) {
            //    phimQuery = phimQuery.Where(p => p.mac != "PG");
            //}

            //var ketQua = phimQuery
            var ketQua = phimQuery
                .Where(p => ((theLoai == "Phim mới" && p.ngayramat.HasValue && p.ngayramat > DateTime.Now.AddMonths(-3)) ||
                            (theLoai == "Phim hot" && p.sove.HasValue && p.sove > 10)) &&
                            (p.theloai != null && soThich != null && p.theloai.Contains(soThich)))
                .ToList()
                .Select(p => new MovieData {
                    Id = p.id_phim,
                    Name = p.tenphim,
                    Mac = p.mac,
                    NumberOfViewers = p.sove.GetValueOrDefault(),
                    Genre = p.theloai ?? "Unknown",
                    ReleaseDateDays = p.ngayramat.HasValue ? (float)((p.ngayramat.Value - new DateTime(2000, 1, 1)).TotalDays) : 0,
                    IsNew = p.ngayramat.HasValue && p.ngayramat.Value > DateTime.Now.AddMonths(-3) ? 1 : 0,
                    IsHot = p.sove.GetValueOrDefault() > 10 ? 1 : 0,
                    Rating = p.sove.GetValueOrDefault(),
                })
                .Select(md => new {
                    Movie = md,
                    Prediction = predictionEngine.Predict(md)
                })
                .OrderByDescending(result => result.Prediction.Score)
                .Select(result => result.Movie)
                .ToList();

            // Kiểm tra và bổ sung phim nếu không đủ 3 phim
            //if (ketQua.Count < 3) {
            //    var additionalMovies = _context.Phims
            //        .Where(p => !ketQua.Select(k => k.Id).Contains(p.id_phim))
            //        .OrderByDescending(p => p.ngayramat)
            //        .ThenByDescending(p => p.sove)
            //        .Take(5 - ketQua.Count)
            //        .Select(p => new MovieData {
            //            Id = p.id_phim,
            //            Name = p.tenphim,
            //            Genre = p.theloai ?? "Unknown",
            //            Mac = p.mac,
            //            ReleaseDate = p.ngayramat ?? DateTime.MinValue,
            //            NumberOfViewers = p.sove ?? 0,
            //            ReleaseDateDays = (float)((p.ngayramat ?? DateTime.Now) - new DateTime(2000, 1, 1)).TotalDays,
            //            IsNew = ((p.ngayramat ?? DateTime.Now) > DateTime.Now.AddMonths(-1)) ? 1 : 0,
            //            IsHot = (p.sove ?? 0) > 100 ? 1 : 0,
            //            Rating = p.sove ?? 0,
            //        })
            //        .ToList();

            //    ketQua.AddRange(additionalMovies);
            //}

            // Giới hạn kết quả tối đa là 5 phim
            ketQua = ketQua.Take(5).ToList();

            ListBoxKetQua.Items.Clear();
            //foreach (var phim in ketQua) {
            //ListBoxKetQua.Items.Add($"{phim.Genre} - {phim.ReleaseDateDays} ngày từ 0 - {phim.NumberOfViewers} lượt xem");
            //ListBoxKetQua.Items.Add($"{phim.Genre} - {phim.ReleaseDateDays} ngày từ 0 - {phim.NumberOfViewers} lượt xem");
            //}
            //} catch (Exception ex) {
            //    MessageBox.Show(ex.Message);
            //}
            foreach (var phim in ketQua) {
                ListBoxKetQua.Items.Add($"Tên phim: {phim.Name}, Thể loại: {phim.Genre}, Mã: {phim.Mac}, Ngày ra mắt: {phim.ReleaseDate.ToShortDateString()}, Tổng số vé: {phim.NumberOfViewers}");
            }
        }
    }
}
