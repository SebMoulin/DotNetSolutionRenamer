using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SolutionTemplateRenamer.Business;
using SolutionTemplateRenamer.ViewModels;

namespace SolutionTemplateRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowsViewModel(new SolutionRenamerService());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var vm = DataContext as MainWindowsViewModel;
            if (vm != null)
            {
                vm.TemplateApplied();
            }
        }
    }
}
