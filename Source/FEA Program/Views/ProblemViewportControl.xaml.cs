using FEA_Program.Views.Helix;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for ProblemViewportControl.xaml
    /// </summary>
    public partial class ProblemViewportControl : UserControl
    {
        public ProblemViewportControl()
        {
            InitializeComponent();

            // Handle the viewport rightclick for context menu
            //problemViewport.viewPort.MouseRightButtonDown += OnViewPortRightClick;
            //problemViewport.MouseRightButtonDown += OnViewPortRightClick;
            //this.MouseRightButtonDown += OnViewPortRightClick;
            problemViewport.viewPort.MouseDown += OnViewPortRightClick;
        }

        private void OnViewPortRightClick(object? sender, MouseButtonEventArgs e)
        {
            //sender = ((ViewPortControl)sender).viewPort;
            
            if (sender is Viewport3DX viewport)
            {
                // Get the mouse position relative to the Viewport3DX
                var position = Mouse.GetPosition(viewport);

                // Perform a hit test at the mouse position
                var hitResult = viewport.FindHits(position).FirstOrDefault();
                if (hitResult == null)
                    return;

                //hitResult.ModelHit
                if(hitResult.ModelHit is Element3D visual)
                {
                    while (visual?.Parent != null)
                    {
                       // var wrapper = vis
                        
                        if (visual.Parent is NodeVisual3D node)
                        {
                            // Found the parent GroupModel3D
                            node.ContextMenu.DataContext = node.DataContext;
                            node.ContextMenu.PlacementTarget = viewport;
                            node.ContextMenu.Placement = PlacementMode.MousePoint;
                            node.ContextMenu.IsOpen = true;
                            break;
                        }

                        if (visual.Parent is Element3D parent)
                            visual = parent;
                        else
                            break;
                    }


                    //var parent = visual.Parent;
                }
                /*
                // Check if the hit item is your custom visual or its child
                if (hitResult.ModelHit is NodeVisual3D visual)
                {
                    // Retrieve the context menu defined in XAML
                    var menu = visual.ContextMenu;
                    if (menu != null)
                    {
                        menu.PlacementTarget = viewport; // important: must have a UI placement target
                        menu.IsOpen = true;
                    }

                    e.Handled = true;
                }
                */
            }
        }
    }
}
