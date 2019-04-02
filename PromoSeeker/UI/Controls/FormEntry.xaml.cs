using System.Windows;
using System.Windows.Controls;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for FormEntry.xaml
    /// </summary>
    public partial class FormEntry : UserControl
    {
        public FormEntry()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(FormEntry), new PropertyMetadata("Text Label"));

        #endregion
    }
}
