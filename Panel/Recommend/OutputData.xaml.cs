﻿using System;
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
using FilmRecommender.Panel.Recommend;

namespace FilmRecommender.Panel.Recommend
{
    /// <summary>
    /// Interaction logic for OutputData.xaml
    /// </summary>
    public partial class OutputData : UserControl
    {
        public OutputData()
        {
            InitializeComponent();
        }

        private void ListBoxImages_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (listBoxImages.SelectedItem != null) {
                string selectedImage = listBoxImages.SelectedItem.ToString();
                //imageControl.SetImageSource(selectedImage);
            }
        }
    }
}
