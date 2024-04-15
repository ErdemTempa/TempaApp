using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempaApp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TempaApp.AppShell), typeof(TempaApp.Droid.MyShellRenderer))]

namespace TempaApp.Droid
{
    
    public class MyShellRenderer : ShellRenderer
    {
        public MyShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new MyBottomNavViewAppearanceTracker(this, shellItem);
        }

        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new MyToolBarAppearanceTracker(this);
        }
    }

    class MyBottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        public MyBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
        {
        }
        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            

            bottomView.LayoutParameters.Height = 60;
            bottomView.LabelVisibilityMode = NavigationBarView.LabelVisibilityUnlabeled;
            base.SetAppearance(bottomView, appearance);
        }
    }

    class MyToolBarAppearanceTracker: ShellToolbarAppearanceTracker
    {
        public MyToolBarAppearanceTracker(IShellContext shellContext): base(shellContext)
        {

        }

        public override void SetAppearance(AndroidX.AppCompat.Widget.Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            var i = toolbar.LayoutParameters.Height;
            i = i * 70;
            i = i / 100;
            toolbar.LayoutParameters.Height = i;
            base.SetAppearance(toolbar, toolbarTracker, appearance);
        }
    }
}