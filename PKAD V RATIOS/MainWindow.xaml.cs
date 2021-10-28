using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;

namespace PKAD_V_RATIOS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private V_Ratio_Renderer renderer = null;
        string exportFolderPath = "";
        private string pageIDnum ="";
        public MainWindow()
        {
            renderer = null;
            InitializeComponent();

            pbStatus.Visibility = Visibility.Hidden;
            pbStatus.Value = 0;
            
        }
        private void btnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            List<V_Ratio_Model> data = new List<V_Ratio_Model>();

            if (openFileDialog.ShowDialog() == true)
            {
                //try
                //{
                    IWorkbook workbook = null;
                    string fileName = openFileDialog.FileName;
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (fileName.IndexOf(".xlsx") > 0)
                            workbook = new XSSFWorkbook(fs);
                        else if (fileName.IndexOf(".xls") > 0)
                            workbook = new HSSFWorkbook(fs);
                    }
                    ISheet sheet = workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;

                            for (int i = 1; i< rowCount; i++)
                            {
                                IRow curRow = sheet.GetRow(i);
                                if (curRow == null)
                                {
                                    rowCount = i - 1;
                                    break;
                                }
                                if (curRow.Cells.Count == 10)
                                {
                                    string batch = "";
                                    if (curRow.GetCell(2).CellType == CellType.String) batch = curRow.GetCell(2).StringCellValue.Trim();
                                    else batch = curRow.GetCell(2).NumericCellValue.ToString().Trim();
                                    var tmp = new V_Ratio_Model()
                                    {
                                        pallet = Convert.ToInt32( curRow.GetCell(0).NumericCellValue ),
                                        box = curRow.GetCell(1).StringCellValue.Trim(),
                                        batch = batch,
                                        total_ballots = Convert.ToInt32( curRow.GetCell(3).NumericCellValue),
                                        trump = Convert.ToInt32(curRow.GetCell(4).NumericCellValue),
                                        biden = Convert.ToInt32(curRow.GetCell(5).NumericCellValue),
                                        jorgenson = Convert.ToInt32(curRow.GetCell(6).NumericCellValue),
                                        writeInOverUnder = Convert.ToInt32(curRow.GetCell(7).NumericCellValue),
                                        ballottype = curRow.GetCell(8).StringCellValue.Trim(),
                                        percent = curRow.GetCell(9).NumericCellValue
                                    };
                                    data.Add(tmp);

                                }
                            
                            }


                    }
                //}
                //catch (Exception)
                //{

                //}
                if (data.Count > 0)
                {
                    renderer.setData(data);
                    Render();
                    int pageNum = 0;
                    pageNum = renderer.getDataCount() / 12;
                    if (renderer.getDataCount() % 12 != 0) pageNum++;

                    Task.Run(() =>
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            AllpagesLabel.Content = "Of " + pageNum.ToString() +  " Pages";
                        }), DispatcherPriority.Render);
                        Thread.Sleep(100);
                    });
                }
            }
        }
       private void btnExportAllChart_Click(object sender, RoutedEventArgs e)
        {
            if (renderer == null)
            {
                renderer = new V_Ratio_Renderer((int)myCanvas.Width, (int)myCanvas.Height);
            }

            if (renderer.getDataCount() > 0 )
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {
                        exportFolderPath = dialog.SelectedPath;
                        pbStatus.Visibility = Visibility.Visible;
                        pbStatus.Minimum = 0;

                        int pageNum = 0;
                        pageNum = renderer.getDataCount() / 12;
                        if (renderer.getDataCount() % 12 != 0) pageNum++;

                        pbStatus.Maximum = pageNum;
                        pbStatus.Value = 0;

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoExport;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerAsync();
                        worker.RunWorkerCompleted += worker_CompletedWork;
                    }
                }
            }
        }
        private void pageChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            try
            {
                pageIDnum = pageID.Text;
                Render(Convert.ToInt32(pageIDnum));
            }
            catch (Exception)
            {

            }
        }
        private void btnExportCurrentChart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.png)|*.png";
            //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveControlImage(PrecinctChart, saveFileDialog.FileName);
            }
        }
        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Render(1);
        }
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public BitmapSource CreateBitmapFromControl(FrameworkElement element)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(element);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }
        private void SaveControlImage(FrameworkElement control, string filename)
        {
            RenderTargetBitmap rtb = (RenderTargetBitmap)CreateBitmapFromControl(control);
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }
        void Render(int pageId = 1)
        {
            if (renderer == null)
                renderer = new V_Ratio_Renderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            renderer.setRenderSize((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            renderer.draw(pageId);
            myImage.Source = BmpImageFromBmp(renderer.getBmp());

        }
        void worker_DoExport(object sender, DoWorkEventArgs e)
        {

            if (renderer == null) return;
            List<V_Ratio_Model> data = renderer.getData();

            int pageNum = 0;
            pageNum = data.Count / 12;
            if (data.Count % 12 != 0) pageNum++;

            for (int page = 1; page <= pageNum; page++)
            {
                renderer.draw(page);
                string filename = exportFolderPath + "/" + (page).ToString() + ".png";
                SaveBitmapImagetoFile(BmpImageFromBmp(renderer.getBmp()), filename);
                (sender as BackgroundWorker).ReportProgress(page);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }
        void worker_CompletedWork(object sender, RunWorkerCompletedEventArgs e)
        {
            pbStatus.Visibility = Visibility.Hidden;
            string msg = "Exporting has been done\n";
            MessageBox.Show(msg);
        }
        private void SaveBitmapImagetoFile(BitmapImage image, string filePath)
        {
            //PngBitmapEncoder encoder1 = new PngBitmapEncoder();
            //encoder1.Frames.Add(BitmapFrame.Create(image));

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            try
            {
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {

            }


        }
    }
}
