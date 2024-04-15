using System;
using System.Collections.Generic;
using System.ComponentModel;
using TempaApp.Models;
using TempaApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TempaApp.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}