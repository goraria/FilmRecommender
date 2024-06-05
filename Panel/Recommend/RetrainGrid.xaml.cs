using Microsoft.ML;
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
using FilmRecommender.Panel.Recommend;

namespace FilmRecommender.Panel.Recommend {
    /// <summary>
    /// Interaction logic for RetrainGrid.xaml
    /// </summary>
    public partial class RetrainGrid : Page {
        private readonly CinemaContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        //private List<MovieData> ketqua;

        public RetrainGrid() {
            InitializeComponent();
            //LoadImageNames();
            _context = new CinemaContext();
            _mlContext = new MLContext();
            LoadGenreNames();
            TrainModel(10);
        }

        private void LoadGenreNames() {
            var data = _context.Phims
                .Where(p => p.theloai != null)
                .Select(p => new Phim {
                    theloai = p.theloai ?? "",
                }).ToList();
            List<string> genreList = new List<string>();
            foreach (var phim in data) {
                List<string> list = new List<string>(phim.theloai.ToString().Split(", "));
                foreach (var genre in list) {
                    if (!genreList.Contains(genre)) {
                        genreList.Add(genre);
                        ComboBoxSoThich.Items.Add(new ComboBoxItem { Content = genre });
                    }
                }
            }
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

        private void SetImageSource(string imagePath) {
            try {
                imageDisplay.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            } catch (Exception ex) {
                // Xử lý lỗi khi không thể tải ảnh
                MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            }
        }

        private void DataGridKetQua_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                if (DataGridKetQua.SelectedItem != null) {
                    var selectedImageItem = DataGridKetQua.SelectedItem as MovieData;
                    if (selectedImageItem != null) {
                        SetImageSource(ImagePath(selectedImageItem.Id.ToString()));
                    }
                } else {
                    ClearImageSource();
                }
            } catch {
                ClearImageSource();
            }
        }

        private void ClearImageSource() {
            imageDisplay.Source = null;
        }

        private ITransformer TrainModel(int numberOfTrees) {
            // Kết hợp dữ liệu từ các bảng Ve, Phim và Khachhang

            var data = _context.Phims
                .GroupJoin(
                    _context.Ves,
                    phim => phim.id_phim,
                    ve => ve.id_phim,
                    (phim, ves) => new { phim, ves })
                .SelectMany(
                    x => x.ves.DefaultIfEmpty(),
                    (x, ve) => new { x.phim, ve })
                .GroupJoin(
                    _context.Khachhangs,
                    x => x.ve.id_khachhang,
                    khachhang => khachhang.id_khachhang,
                    (x, khachhangs) => new { x.phim, x.ve, khachhangs })
                .SelectMany(
                    x => x.khachhangs.DefaultIfEmpty(),
                    (x, khachhang) => new {
                        x.phim,
                        khachhang,
                        gioitinh = khachhang != null ? khachhang.gioitinh : "0",
                        ngaysinh = khachhang != null && khachhang.ngaysinh.HasValue ? khachhang.ngaysinh.Value : (DateTime?)null
                    })
                .ToList()
                .Select(x => new MovieData {
                    Id = x.phim.id_phim,
                    Name = x.phim.tenphim,
                    NumberOfViewers = x.phim.sove ?? 0,
                    Genre = x.phim.theloai ?? "Unknown",
                    ReleaseDateDays = (float)((x.phim.ngayramat ?? DateTime.Now) - new DateTime(2000, 1, 1)).TotalDays,
                    IsNew = ((x.phim.ngayramat ?? DateTime.Now) > DateTime.Now.AddMonths(-3)) ? 1 : 0,
                    IsHot = (x.phim.sove ?? 0) > 20 ? 1 : 0,
                    Rating = x.phim.sove ?? 0,
                    Label = 1, // Dummy label for regression
                    CustomerAge = x.ngaysinh.HasValue ? (float)(DateTime.Now.Year - x.ngaysinh.Value.Year) : 0,
                    CustomerGender = x.gioitinh == "Nam" ? 1 : 0 // Assuming "Nam" và "Nữ"
                })
                .ToList();


            if (data.Count == 0) {
                throw new InvalidOperationException("Không có dữ liệu để huấn luyện mô hình.");
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("Genre")
                .Append(_mlContext.Transforms.Concatenate("Features", "NumberOfViewers", "Genre", "ReleaseDateDays", "IsNew", "IsHot", "Rating", "CustomerAge", "CustomerGender"))
                .Append(_mlContext.Regression.Trainers.FastForest(new FastForestRegressionTrainer.Options {
                    NumberOfTrees = numberOfTrees,
                    FeatureColumnName = "Features",
                    LabelColumnName = "Label"
                }));

            try {
                var model = pipeline.Fit(dataView);
                return model;
            } catch (Exception ex) {
                throw new InvalidOperationException("Có lỗi xảy ra khi huấn luyện mô hình: " + ex.Message);
            }

            DataGridKetQua.ItemsSource = data.Take(5).ToList();
        }

        private void TrainResult() {
            //DataGridKetQua.ItemsSource = null;
            string? soNguoi = (ComboBoxSoNguoi.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (!int.TryParse(TextBoxTuoi.Text, out int tuoi)) {
                MessageBox.Show("Vui lòng nhập tuổi hợp lệ.");
                return;
            }
            string? gioiTinh = (ComboBoxGioiTinh.SelectedItem as ComboBoxItem)?.Content.ToString();
            string? soThich = (ComboBoxSoThich.SelectedItem as ComboBoxItem)?.Content.ToString();
            string? theLoai = (ComboBoxTheLoai.SelectedItem as ComboBoxItem)?.Content.ToString();

            _model = TrainModel(200);
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MovieData, MoviePrediction>(_model);
            var phimQuery = _context.Phims.AsQueryable();

            //if (tuoi < 5 || tuoi > 95) {
            //    return;
            //}

            if (soNguoi == "Gia đình") {
                phimQuery = phimQuery.Where(p => p.theloai.Contains("Hài hước") || p.theloai.Contains("Hoạt hình"));
            } else if (soNguoi == "Cặp đôi") {
                phimQuery = phimQuery.Where(p => p.theloai.Contains("Tình cảm") || p.theloai.Contains("Hài hước"));
            } else {
                if (tuoi <= 15 && tuoi > 13) {
                    phimQuery = phimQuery.Where(p => p.mac != "R");
                } else if (tuoi <= 13) {
                    phimQuery = phimQuery.Where(p => p.mac == "G");
                }
            }

            //var phimList = phimQuery
            //    .Where(p => ((theLoai == "Phim mới" && p.ngayramat.HasValue && p.ngayramat > DateTime.Now.AddMonths(-2)) ||
            //                (theLoai == "Phim hot" && p.sove.HasValue && p.sove > 20)) &&
            //                (p.theloai != null && soThich != null && p.theloai.Contains(soThich)))
            //    .ToList();

            //if (phimList.Count == 0) {
            //    MessageBox.Show("Không tìm thấy phim nào phù hợp với các tiêu chí.");
            //    return;
            //}

            var ketQua = phimQuery
                .Where(p => ((theLoai == "Phim mới" && p.ngayramat.HasValue && p.ngayramat > DateTime.Now.AddMonths(-2)) ||
                            (theLoai == "Phim hot" && p.sove.HasValue && p.sove > 20)) &&
                            (p.theloai != null && soThich != null && p.theloai.Contains(soThich)))
                .ToList()
                .Select(p => new MovieData {
                    Id = p.id_phim,
                    Name = p.tenphim,
                    Genre = p.theloai ?? "Unknown",
                    Mac = p.mac,
                    ReleaseDate = p.ngayramat ?? DateTime.MinValue,
                    NumberOfViewers = p.sove ?? 0,
                    ReleaseDateDays = (float)((p.ngayramat ?? DateTime.Now) - new DateTime(2000, 1, 1)).TotalDays,
                    IsNew = ((p.ngayramat ?? DateTime.Now) > DateTime.Now.AddMonths(-2)) ? 1 : 0,
                    IsHot = (p.sove ?? 0) > 100 ? 1 : 0,
                    Rating = p.sove ?? 0,
                    CustomerAge = tuoi,
                    CustomerGender = gioiTinh == "Nam" ? 1 : 0
                })
                .Select(md => new {
                    Movie = md,
                    Prediction = predictionEngine.Predict(md)
                })
                .OrderByDescending(result => result.Prediction.Score)
                .Select(result => result.Movie)
                .ToList();

            if (ketQua.Count < 3) {
                var firstMovieMac = ketQua.FirstOrDefault()?.Mac;
                var additionalMovies = _context.Phims
                    .Where(p => !ketQua.Select(k => k.Id).Contains(p.id_phim) &&
                                ((theLoai == "Phim mới" && p.ngayramat > DateTime.Now.AddMonths(-2)) ||
                                 (theLoai == "Phim hot" && p.sove > 20)) && (p.mac != null && p.mac == firstMovieMac))
                    .OrderByDescending(p => p.ngayramat)
                    .ThenByDescending(p => p.sove)
                    .Take(5 - ketQua.Count)
                    .Select(p => new MovieData {
                        Id = p.id_phim,
                        Name = p.tenphim,
                        Genre = p.theloai ?? "Unknown",
                        Mac = p.mac,
                        ReleaseDate = p.ngayramat ?? DateTime.MinValue,
                        NumberOfViewers = p.sove ?? 0,
                        ReleaseDateDays = (float)((p.ngayramat ?? DateTime.Now) - new DateTime(2000, 1, 1)).TotalDays,
                        IsNew = ((p.ngayramat ?? DateTime.Now) > DateTime.Now.AddMonths(-2)) ? 1 : 0,
                        IsHot = (p.sove ?? 0) > 100 ? 1 : 0,
                        Rating = p.sove ?? 0,
                        CustomerAge = tuoi,
                        CustomerGender = gioiTinh == "Nam" ? 1 : 0
                    })
                    .ToList();

                ketQua.AddRange(additionalMovies);
            }

            if (ketQua.Count < 3) {
                // Nếu vẫn không đủ, thêm phim theo tiêu chí thể loại
                var additionalMovies = _context.Phims
                    //.Where(p => !ketQua.Select(k => k.Id).Contains(p.id_phim) && p.theloai.Contains(soThich))
                    .Where(p => !ketQua.Select(k => k.Id).Contains(p.id_phim) && p.mac.Contains(""))
                    .OrderByDescending(p => p.ngayramat)
                    .ThenByDescending(p => p.sove)
                    .Take(5 - ketQua.Count)
                    .Select(p => new MovieData {
                        Id = p.id_phim,
                        Name = p.tenphim,
                        Genre = p.theloai ?? "Unknown",
                        Mac = p.mac,
                        ReleaseDate = p.ngayramat ?? DateTime.MinValue,
                        NumberOfViewers = p.sove ?? 0,
                        ReleaseDateDays = (float)((p.ngayramat ?? DateTime.Now) - new DateTime(2000, 1, 1)).TotalDays,
                        IsNew = ((p.ngayramat ?? DateTime.Now) > DateTime.Now.AddMonths(-2)) ? 1 : 0,
                        IsHot = (p.sove ?? 0) > 100 ? 1 : 0,
                        Rating = p.sove ?? 0,
                        CustomerAge = tuoi,
                        CustomerGender = gioiTinh == "Nam" ? 1 : 0
                    })
                    .ToList();

                ketQua.AddRange(additionalMovies);
            }

            ketQua = ketQua.Take(5).ToList();

            DataGridKetQua.ItemsSource = ketQua;
        }

        private void ButtonTimPhim_Click(object sender, RoutedEventArgs e) {
            TrainResult();
        }
    }
}
