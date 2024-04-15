using System.ComponentModel;
using TempaApp.ViewModels;
using Xamarin.Forms;

namespace TempaApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}