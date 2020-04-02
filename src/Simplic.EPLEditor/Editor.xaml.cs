using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop.Editor;
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

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Editor implementation that contains the logic for the epl editor
    /// </summary>
    public partial class Editor : UserControl, IErrorListener
    {
        #region Private Member
        private ObservableCollection<EPLError> errors;
        private ITextMarkerService textMarkerService;
        private EPLFormular eplFormular;
        private Point? lastCenterPositionOnTarget;
        private Point? lastMousePositionOnTarget;
        private Point? lastDragPoint;
        private bool textChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Crete epl editor control
        /// </summary>
        public Editor()
        {
            InitializeComponent();

            InitializeTextMarkerService();

            UseAutoRendering = true;
            textChanged = false;
            editor.TextChanged += OnEditorTextChanged;

            SearchPanel.Install(editor.TextArea);

            // Init
            errors = new ObservableCollection<EPLError>();
            errorListBox.ItemsSource = errors;

            // Initialize viewer
            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;

            slider.ValueChanged += OnSliderValueChanged;

            this.GotFocus += OnEditorGotFocus;
            this.GotKeyboardFocus += OnEditorGotKeyboardFocus;
            // Set default width
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);

            this.SetLabelPrinterSizeCm(10.0, 5.7, 1.9);

            // Create timer
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }
        #endregion

        #region Private Methods

        #region [Focus]
        /// <summary>
        /// Got keyboard focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditorGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Keyboard.Focus(editor);
        }

        /// <summary>
        /// Got editor focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditorGotFocus(object sender, RoutedEventArgs e)
        {
            //editor.Focus();
        }
        #endregion

        #region [OnEditorTextChanged]
        /// <summary>
        /// Input changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditorTextChanged(object sender, EventArgs e)
        {
            ContentChanged = true;
            textChanged = true;
        }

        /// <summary>
        /// Editor tock for asnyc rendering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (UseAutoRendering && textChanged)
            {
                Render();

                textChanged = false;
            }
        }
        #endregion

        #region [Viewer Input/Handling]
        /// <summary>
        /// Mouse move over canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (lastDragPoint.HasValue)
                {
                    Point posNow = e.GetPosition(scrollViewer);

                    double dX = posNow.X - lastDragPoint.Value.X;
                    double dY = posNow.Y - lastDragPoint.Value.Y;

                    lastDragPoint = posNow;

                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
                }
            }
        }

        /// <summary>
        /// Mouse left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var mousePos = e.GetPosition(scrollViewer);
                if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
                {
                    scrollViewer.Cursor = Cursors.SizeAll;
                    lastDragPoint = mousePos;
                    Mouse.Capture(scrollViewer);
                }
            }
        }

        /// <summary>
        /// Scrolling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                lastMousePositionOnTarget = Mouse.GetPosition(grid);

                if (e.Delta > 0)
                {
                    slider.Value += 1;
                }
                if (e.Delta < 0)
                {
                    slider.Value -= 1;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// End dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        /// <summary>
        /// Zoom value changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            scaleTransform.ScaleX = e.NewValue;
            scaleTransform.ScaleY = e.NewValue;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
        }

        /// <summary>
        /// Scrolling changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        #endregion

        #region [InitializeTextMarkerService]
        /// <summary>
        /// Init service
        /// </summary>
        private void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(editor.Document);
            editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            editor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)editor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
            {
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            }
            this.textMarkerService = textMarkerService;

            // Syntax highlight
            using (Stream s = this.GetType().Assembly.GetManifestResourceStream("Simplic.EPLEditor.AvalonEditor.EPL.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
        #endregion

        #endregion

        #region Public Methods

        #region [Undo]
        /// <summary>
        /// Undo changes
        /// </summary>
        public void Undo()
        {
            editor.Undo();
        }

        /// <summary>
        /// Restore undo -> redo
        /// </summary>
        public void Redo()
        {
            editor.Redo();
        }
        #endregion

        #region [Clipboard]
        /// <summary>
        /// Copy into clipboard
        /// </summary>
        public void Copy()
        {
            editor.Copy();
        }

        /// <summary>
        /// Pate into editor
        /// </summary>
        public void Paste()
        {
            editor.Paste();
        }

        /// <summary>
        /// Cut frome ditor text
        /// </summary>
        public void Cut()
        {
            editor.Cut();
        }
        #endregion

        #region [SetLabelPrinterSize]
        /// <summary>
        /// Set printer size in cm
        /// </summary>
        /// <param name="printerWidth">Width of the printer head</param>
        /// <param name="labelWidth">Size of the label (width)</param>
        /// <param name="labelHeight">Size of the label (height)</param>
        /// <param name="dpi">DPI of the screen</param>
        public void SetLabelPrinterSizeCm(double printerWidth, double labelWidth, double labelHeight)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiX = (int)dpiXProperty.GetValue(null, null);

            var dpiYProperty = typeof(SystemParameters).GetProperty("DpiY", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiY = (int)dpiXProperty.GetValue(null, null);

            SetLabelPrinterSizePixel(UnitHelper.CmToPx(printerWidth, dpiX), UnitHelper.CmToPx(labelWidth, dpiX), UnitHelper.CmToPx(labelHeight, dpiY));
        }

        /// <summary>
        /// Set printer size in pixel
        /// </summary>
        /// <param name="printerWidth">Width of the printer head</param>
        /// <param name="labelWidth">Size of the label (width)</param>
        /// <param name="labelHeight">Size of the label (height)</param>
        /// <param name="dpi">DPI of the screen</param>
        public void SetLabelPrinterSizePixel(double printerWidth, double labelWidth, double labelHeight)
        {
            grid.Width = Math.Max(printerWidth, labelWidth);
            grid.Height = labelHeight + 10;

            labelCanvas.Height = labelHeight;
            labelCanvas.Width = labelWidth;

            Canvas.SetTop(labelCanvas, 5);
            Canvas.SetLeft(labelCanvas, (printerWidth / 2) - (labelWidth / 2));
        }
        #endregion

        #region [Error]
        /// <summary>
        /// Print error
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="line">Line number of the error</param>
        /// <param name="index">Index</param>
        /// <param name="length">Length</param>
        public void Error(string message, int line, int start, int end)
        {
            errors.Add(new EPLError()
            {
                Message = message,
                Line = line,
                Index = start,
                Length = end
            });

            // Show in editor
            ITextMarker marker = this.textMarkerService.Create(start, end);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = System.Windows.Media.Colors.Red;
        }
        #endregion

        #region [OnErrorListBoxDoubleClick]
        /// <summary>
        /// Jump to error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnErrorListBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (errorListBox.SelectedItem != null)
            {
                editor.Select((errorListBox.SelectedItem as EPLError).Index, (errorListBox.SelectedItem as EPLError).Length);
            }
        }
        #endregion

        #region [Render]
        /// <summary>
        /// Render the current EPL formular
        /// </summary>
        public void Render()
        {
            if (eplFormular == null)
            {
                eplFormular = new EPLFormular(this, editor.Text);
                eplFormular.Processor = Processor;
            }

            errors.Clear();
            eplFormular.Load(editor.Text);
            textMarkerService.RemoveAll(m => true);

            labelCanvas.Children.Clear();
            renderGrid.Children.Clear();
            renderGrid.Children.Add(labelCanvas);

            generatedEPLEditor.Text = eplFormular.GetEPLCode();

            // Render
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiX = (int)dpiXProperty.GetValue(null, null);

            var dpiYProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiY = (int)dpiXProperty.GetValue(null, null);

            Tuple<double, double> relativePosition = new Tuple<double, double>(0, 0);
            Tuple<double, double> absoltePosition = new Tuple<double, double>(0, 0);

            // Label size
            double labelHeightPx = UnitHelper.CmToPx(1.9, dpiX);
            double labelWidthPx = UnitHelper.CmToPx(5.7, dpiY);
            double printerWidthPx = UnitHelper.CmToPx(10, dpiX);

            foreach (var command in eplFormular.Commands)
            {
                if (command is RCommand)
                {
                    double _x = 0;
                    double _y = 0;

                    if (double.TryParse((command as RCommand).P1, out _x) && double.TryParse((command as RCommand).P2, out _y))
                    {
                        relativePosition = new Tuple<double, double>(UnitHelper.DotToPixel(8, _x, dpiX), UnitHelper.DotToPixel(8, _y, dpiY));

                        absoltePosition = new Tuple<double, double>
                            (
                                relativePosition.Item1,
                                relativePosition.Item2 + 5
                            );
                    }
                }
                else if (command is LabelHeightCommand)
                {
                    if (double.TryParse(((LabelHeightCommand)command).P1, out labelHeightPx))
                    {
                        labelHeightPx = UnitHelper.DotToPixel(8, (int)labelHeightPx, dpiY);
                        SetLabelPrinterSizePixel(printerWidthPx, labelHeightPx, labelWidthPx);

                        absoltePosition = new Tuple<double, double>
                            (
                                ((printerWidthPx / 2) - (labelWidthPx / 2)),
                                relativePosition.Item2 + 5
                            );
                    }
                }
                else if (command is LabelWidthCommand)
                {
                    if (double.TryParse(((LabelWidthCommand)command).P1, out labelWidthPx))
                    {
                        labelWidthPx = UnitHelper.DotToPixel(8, (int)labelWidthPx, dpiX);
                        SetLabelPrinterSizePixel(printerWidthPx, labelWidthPx, labelHeightPx);

                        absoltePosition = new Tuple<double, double>
                            (
                                ((printerWidthPx / 2) - (labelWidthPx / 2)),
                                relativePosition.Item2 + 5
                            );
                    }
                }
                else
                {
                    var uiFactory = UIControlFactory.Get(command.GetType());

                    if (uiFactory != null)
                    {
                        // Render on UI
                        uiFactory.Render(this.renderGrid, eplFormular, command, absoltePosition, dpiX, dpiY);
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Public Member
        /// <summary>
        /// List of occured errors
        /// </summary>
        public ObservableCollection<EPLError> Errors
        {
            get
            {
                return errors;
            }
        }

        /// <summary>
        /// EPL formular, setting will update the editor, getting
        /// will create new EPLFormular from editor content
        /// </summary>
        public EPLFormular EPLFormular
        {
            get
            {
                eplFormular = new EPLFormular(this, editor.Text);
                eplFormular.Processor = Processor;

                return eplFormular;
            }

            set
            {
                eplFormular = value;

                if (value != null)
                {
                    editor.Text = value.GetEPLCode(false);
                }
            }
        }

        /// <summary>
        /// Value of the text editor
        /// </summary>
        public string Text
        {
            get
            {
                return editor.Text;
            }
            set
            {
                editor.Text = value;
            }
        }

        /// <summary>
        /// Will render changed automatically after a few seconds
        /// </summary>
        public bool UseAutoRendering
        {
            get;
            set;
        }

        /// <summary>
        /// Content changed attribute, for private control use
        /// </summary>
        public bool ContentChanged
        {
            get;
            set;
        }

        /// <summary>
        /// EPL processor
        /// </summary>
        public IPreprocessor Processor
        {
            get;
            set;
        }
        #endregion
    }
}
