using Microsoft.Win32;
using pick14;
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
using System.Windows.Shapes;

namespace pick14
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<GalleryImage> galleryImages = new List<GalleryImage>();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSampleImages();
            UpdateLayoutInfo();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayoutInfo();
        }

        private void LoadSampleImages()
        {
            // Додаємо зразкові фотографії
            galleryImages = new List<GalleryImage>
            {
                new GalleryImage { Title = "Природа", ImagePath = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400" },
                new GalleryImage { Title = "Гори", ImagePath = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400" },
                new GalleryImage { Title = "Місто", ImagePath = "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=400" },
                new GalleryImage { Title = "Пляж", ImagePath = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400" },
                new GalleryImage { Title = "Архітектура", ImagePath = "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=400" },
                new GalleryImage { Title = "Нічне небо", ImagePath = "https://images.unsplash.com/photo-1518837695005-2083093ee35b?w=400" },
                new GalleryImage { Title = "Канали", ImagePath = "https://images.unsplash.com/photo-1514890547357-a9ee288728e0?w=400" },
                new GalleryImage { Title = "Каньйон", ImagePath = "https://images.unsplash.com/photo-1509316785289-025f5b846b35?w=400" },
                new GalleryImage { Title = "Ліс", ImagePath = "https://images.unsplash.com/photo-1448375240586-882707db888b?w=400" },
                new GalleryImage { Title = "Озеро", ImagePath = "https://images.unsplash.com/photo-1470071459604-3b5ec3a7fe05?w=400" }
            };

            RefreshGallery();
        }

        private void RefreshGallery()
        {
            GalleryWrapPanel.Children.Clear();

            foreach (var image in galleryImages)
            {
                AddImageToGallery(image);
            }

            ImageCountText.Text = $"{galleryImages.Count} зображень";
        }

        private void AddImageToGallery(GalleryImage galleryImage)
        {
            // Створюємо карточку зображення
            var card = new Border
            {
                Style = (Style)FindResource("ImageCardStyle"),
                ToolTip = galleryImage.Title,
                Cursor = Cursors.Hand,
                Margin = new Thickness(8),
                Width = 200,
                Height = 180
            };

            var stackPanel = new StackPanel();

            // Зображення
            var image = new Image
            {
                Stretch = Stretch.UniformToFill,
                Width = 180,
                Height = 140,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Завантажуємо зображення з файлу або URL
            try
            {
                if (galleryImage.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    image.Source = LoadImageFromUrl(galleryImage.ImagePath);
                }
                else
                {
                    image.Source = LoadImageFromFile(galleryImage.ImagePath);
                }
            }
            catch
            {
                image.Source = CreatePlaceholderImage();
            }

            image.MouseDown += (s, e) => ShowImageModal(galleryImage);

            // Назва зображення
            var textBlock = new TextBlock
            {
                Text = galleryImage.Title,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12,
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Medium
            };

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);
            card.Child = stackPanel;

            GalleryWrapPanel.Children.Add(card);
        }

        private BitmapImage LoadImageFromUrl(string url)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch
            {
                return CreatePlaceholderImage();
            }
        }

        private BitmapImage LoadImageFromFile(string filePath)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження зображення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return CreatePlaceholderImage();
            }
        }

        private BitmapImage CreatePlaceholderImage()
        {
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    Brushes.LightGray,
                    null,
                    new Rect(0, 0, 180, 140));

                drawingContext.DrawText(
                    new FormattedText("🖼️",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        24,
                        Brushes.Gray,
                        96),
                    new Point(70, 50));
            }

            var renderTargetBitmap = new RenderTargetBitmap(180, 140, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void ShowImageModal(GalleryImage image)
        {
            ModalTitle.Text = image.Title;

            try
            {
                if (image.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    ModalImage.Source = LoadImageFromUrl(image.ImagePath);
                }
                else
                {
                    ModalImage.Source = LoadImageFromFile(image.ImagePath);
                }
            }
            catch
            {
                ModalImage.Source = CreatePlaceholderImage();
            }

            ModalOverlay.Visibility = Visibility.Visible;
            StatusText.Text = $"Перегляд: {image.Title}";
        }

        private void CloseModalButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            StatusText.Text = "Готово до перегляду";
        }

        private void UpdateLayoutInfo()
        {
            double windowWidth = ActualWidth;

            if (windowWidth < 600)
            {
                // Мобільний вигляд - менші карточки
                foreach (var child in GalleryWrapPanel.Children)
                {
                    if (child is Border card)
                    {
                        card.Width = 140;
                        card.Height = 130;
                        if (card.Child is StackPanel stackPanel && stackPanel.Children.Count > 0)
                        {
                            if (stackPanel.Children[0] is Image img)
                            {
                                img.Width = 120;
                                img.Height = 100;
                            }
                        }
                    }
                }
                StatusText.Text = "📱 Мобільний вигляд";
            }
            else if (windowWidth < 900)
            {
                // Планшетний вигляд
                foreach (var child in GalleryWrapPanel.Children)
                {
                    if (child is Border card)
                    {
                        card.Width = 170;
                        card.Height = 150;
                        if (card.Child is StackPanel stackPanel && stackPanel.Children.Count > 0)
                        {
                            if (stackPanel.Children[0] is Image img)
                            {
                                img.Width = 150;
                                img.Height = 120;
                            }
                        }
                    }
                }
                StatusText.Text = "📋 Планшетний вигляд";
            }
            else
            {
                // Десктопний вигляд
                foreach (var child in GalleryWrapPanel.Children)
                {
                    if (child is Border card)
                    {
                        card.Width = 200;
                        card.Height = 180;
                        if (card.Child is StackPanel stackPanel && stackPanel.Children.Count > 0)
                        {
                            if (stackPanel.Children[0] is Image img)
                            {
                                img.Width = 180;
                                img.Height = 140;
                            }
                        }
                    }
                }
                StatusText.Text = "💻 Десктопний вигляд";
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Всі файли (*.*)|*.*",
                Title = "Виберіть зображення",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                int addedCount = 0;
                foreach (string filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        // Перевіряємо, чи файл існує
                        if (!System.IO.File.Exists(filePath)) // Використовуємо повне ім'я
                        {
                            MessageBox.Show($"Файл не знайдено: {filePath}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            continue;
                        }

                        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath); // Використовуємо повне ім'я

                        var newImage = new GalleryImage
                        {
                            Title = fileName,
                            ImagePath = filePath
                        };

                        galleryImages.Add(newImage);
                        AddImageToGallery(newImage);
                        addedCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при завантаженні файлу {filePath}: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                ImageCountText.Text = $"{galleryImages.Count} зображень";
                StatusText.Text = addedCount > 0 ? $"Додано {addedCount} зображень" : "Не вдалося додати зображення";
            }
        }

        private void ClearGalleryButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Очистити всю галерею?", "Підтвердження",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                galleryImages.Clear();
                RefreshGallery();
                StatusText.Text = "Галерея очищена";
            }
        }
    }

    public class GalleryImage
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }
    }
}


