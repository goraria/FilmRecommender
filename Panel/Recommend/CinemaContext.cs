using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.Data;

namespace FilmRecommender.Panel.Recommend
{
    public class CinemaContext : DbContext
    {
        public DbSet<Phim> Phims { get; set; }
        public DbSet<Khachhang> Khachhangs { get; set; }
        public DbSet<Ve> Ves { get; set; }
        public DbSet<Ghe> Ghes { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<Doan> Doans { get; set; }
        public DbSet<Hoadon> Hoadons { get; set; }
        public DbSet<Thanhvien> Thanhviens { get; set; }
        public DbSet<Cachieu> Cachieus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-V6D8BH0B;Initial Catalog=Recommender;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cachieu>().HasKey(c => c.id_cachieu);
            modelBuilder.Entity<Doan>().HasKey(d => d.id_doan);
            modelBuilder.Entity<Ghe>().HasKey(g => g.id_ghe);
            modelBuilder.Entity<Hoadon>().HasKey(h => h.id_hoadon);
            modelBuilder.Entity<Khachhang>().HasKey(k => k.id_khachhang);
            modelBuilder.Entity<Phim>().HasKey(p => p.id_phim);
            modelBuilder.Entity<Phong>().HasKey(p => p.id_phong);
            modelBuilder.Entity<Thanhvien>().HasKey(t => t.id_thanhvien);
            modelBuilder.Entity<Ve>().HasKey(v => v.id_ve);
        }
    }

    [Table("Phim")]
    public class Phim
    {
        public string id_phim { get; set; }
        public string tenphim { get; set; }
        public string quocgia { get; set; }
        public string theloai { get; set; }
        public int? sove { get; set; }
        public int? thoiluong { get; set; }
        public string hanglamphim { get; set; }
        public DateTime? ngayramat { get; set; }
        public string mac { get; set; }
    }

    [Table("Khachhang")]
    public class Khachhang
    {
        public string id_khachhang { get; set; }
        public string hoten { get; set; }
        public DateTime? ngaysinh { get; set; }
        public string diachi { get; set; }
        public string gioitinh { get; set; }
        public string sodienthoai { get; set; }
    }

    [Table("Ve")]
    public class Ve
    {
        public string id_ve { get; set; }
        public string id_phim { get; set; }
        public string id_phong { get; set; }
        public string id_ghe { get; set; }
        public string id_khachhang { get; set; }
        public string id_nhanvien { get; set; }
        public string id_cachieu { get; set; }
        public DateTime? ngayban { get; set; }
        public int? giave { get; set; }
    }

    [Table("Ghe")]
    public class Ghe
    {
        public string id_ghe { get; set; }
        public string id_phong { get; set; }
        public string tenghe { get; set; }
        public int? giaghe { get; set; }
        public bool? trangthai { get; set; }
    }

    [Table("Phong")]
    public class Phong
    {
        public string id_phong { get; set; }
        public string tenphong { get; set; }
        public int? soghe { get; set; }
        public bool? trangthai { get; set; }
    }

    [Table("Doan")]
    public class Doan
    {
        public string id_doan { get; set; }
        public string ten { get; set; }
        public string loai { get; set; }
        public int? soluong { get; set; }
        public int? gia { get; set; }
    }

    [Table("Hoadon")]
    public class Hoadon
    {
        public string id_hoadon { get; set; }
        public string id_doan { get; set; }
        public string id_khachhang { get; set; }
        public string id_thanhvien { get; set; }
        public int? soluong { get; set; }
        public int? tonggia { get; set; }
        public DateTime? ngayban { get; set; }
    }

    [Table("Thanhvien")]
    public class Thanhvien
    {
        public string id_thanhvien { get; set; }
        public string hoten { get; set; }
        public DateTime? ngaysinh { get; set; }
        public string diachi { get; set; }
        public string gioitinh { get; set; }
        public string sodienthoai { get; set; }
        public bool? loai_taikhoan { get; set; }
        public string taikhoan { get; set; }
        public string matkhau { get; set; }
    }

    [Table("Cachieu")]
    public class Cachieu
    {
        public string id_cachieu { get; set; }
        public string tencachieu { get; set; }
        public TimeSpan? thoigian_batdau { get; set; }
        public TimeSpan? thoigian_ketthuc { get; set; }
    }

    public class MovieData {
        public string Id { get; set; }
        public string Name { get; set; }
        public float NumberOfViewers { get; set; }
        public string Genre { get; set; }
        public string Mac { get; set; }
        public DateTime ReleaseDate { get; set; }
        public float ReleaseDateDays { get; set; }
        public float IsNew { get; set; }
        public float IsHot { get; set; }
        public float Rating { get; set; }
        public float Label { get; set; }
    }

    public class MoviePrediction {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}

