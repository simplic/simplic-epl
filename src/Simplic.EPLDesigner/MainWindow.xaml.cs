using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.Win32;
using Newtonsoft.Json;
using Simplic.EPL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using Telerik.Windows.Controls;

namespace Simplic.EPLDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadRibbonWindow
    {
        #region Private Member
        private Settings settings;
        private string currentPath;
        #endregion

        /// <summary>
        /// Create designer
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += OnMainWindowClosing;

            Telerik.Windows.Controls.StyleManager.SetTheme(radRibbonView, new Telerik.Windows.Controls.Windows8Theme());

            if (System.IO.File.Exists("last.epl"))
            {
                editor.Text = System.IO.File.ReadAllText("last.epl");
            }

            editor.Processor = new IronPythonProcessor();

            LoadSettings();
        }

        #region Private Methods

        #region [Settings]
        /// <summary>
        /// Load application settings
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Configuration\\Settings.json"))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\Configuration\\Settings.json"));
                }
                catch
                {
                    settings = new Settings();
                    settings.DefaultLabelHeight = 1.9;
                    settings.DefaultLabelWidth = 5.7;
                    settings.PrinterHeaderWidth = 10.0;
                }
            }
            else
            {
                settings = new Settings();
                settings.DefaultLabelHeight = 1.9;
                settings.DefaultLabelWidth = 5.7;
                settings.PrinterHeaderWidth = 10.0;
            }

            editor.SetLabelPrinterSizeCm(settings.PrinterHeaderWidth, settings.DefaultLabelWidth, settings.DefaultLabelHeight);
        }

        /// <summary>
        /// Restore application settings
        /// </summary>
        private void SaveSettings()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Configuration"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Configuration");
            }

            File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\Configuration\\Settings.json", JsonConvert.SerializeObject(settings));
        }
        #endregion

        #region [Window Closing]
        /// <summary>
        /// Window closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (editor.ContentChanged == true)
            {
                var res = MessageBox.Show("Would you like to save the changes before closing the application?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (res == MessageBoxResult.Yes)
                {
                    e.Cancel = !Save(false);
                }
            }

            SaveSettings();
            System.IO.File.WriteAllText("last.epl", editor.Text);
        }
        #endregion

        #region [Events]
        /// <summary>
        /// Click save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            Save(false);
        }

        /// <summary>
        /// Click save under button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveUnderButtonClick(object sender, RoutedEventArgs e)
        {
            Save(true);
        }

        /// <summary>
        /// Click undo button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUndoButtonClick(object sender, RoutedEventArgs e)
        {
            editor.Undo();
        }

        /// <summary>
        /// Click print button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrintButtonClick(object sender, RoutedEventArgs e)
        {
            if (editor.EPLFormular == null)
            {
                MessageBox.Show("No EPL-Report created yet.", "No report", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                editor.EPLFormular.Print(new Printing.PrinterDevice(dialog.PrintQueue.Name));
            }
        }

        /// <summary>
        /// Openfile button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            Open();
        }

        /// <summary>
        /// Exit app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Paste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            editor.Paste();
        }

        /// <summary>
        /// Cut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCutClick(object sender, RoutedEventArgs e)
        {
            editor.Cut();
        }

        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            editor.Copy();
        }
        #endregion

        #region [Save]
        /// <summary>
        /// Save / save under
        /// </summary>
        /// <param name="saveUnder">True if new path should be taken</param>
        private bool Save(bool saveUnder)
        {
            if (string.IsNullOrWhiteSpace(currentPath) || saveUnder == true)
            {
                saveUnder = true;
            }

            if (!saveUnder)
            {
                try
                {
                    File.WriteAllText(currentPath, editor.Text);
                    editor.ContentChanged = false;
                    return true;
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Could not save report: " + ex.Message, "Not saved", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "All Files|*.*|EPL-Report|*.epl";
                dialog.FilterIndex = 1;

                if (dialog.ShowDialog() == true)
                {
                    currentPath = dialog.FileName;                    

                    try
                    {
                        File.WriteAllText(currentPath, editor.Text);
                        editor.ContentChanged = false;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not save report: " + ex.Message, "Not saved", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }

            return false;
        }
        #endregion

        #region [Open]
        /// <summary>
        /// Open file
        /// </summary>
        private void Open()
        {
            if (editor.ContentChanged)
            {
                var res = MessageBox.Show("Would you like to save the changes before opening a new report?", "Save changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (res == MessageBoxResult.Yes)
                {
                    var saveRes = Save(false);

                    if (!saveRes)
                    {
                        return;
                    }
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files|*.*|EPL-Report|*.epl";
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() == true)
            {
                currentPath = dialog.FileName;
                editor.Text = File.ReadAllText(currentPath);
                editor.ContentChanged = false;
            }
        }
        #endregion

        #endregion

        #region Public Member
        /// <summary>
        /// Application settings
        /// </summary>
        public Settings Settings
        {
            get
            {
                return settings;
            }

            set
            {
                settings = value;
            }
        }
        #endregion
    }
}
