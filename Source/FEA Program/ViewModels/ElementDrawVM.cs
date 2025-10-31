using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace FEA_Program.ViewModels
{
    internal class ElementDrawVM: ObservableObject
    {
        public static Color SelectedColor = Colors.Yellow;
        public static Color DefaultElementColor = Colors.LightGray;
        
        // ---------------------- Properties ----------------------
        public NodeDrawVM[] Nodes { get; private set; } = [];
        public ElementVM Element { get; private set; } = new();

        public Color ? ColorOverride { get; set; } = null;
        public Color ElementColor => Element.Selected ? SelectedColor : (ColorOverride ?? DefaultElementColor);
        public string ElementText { get; set; } = "";

        // ---------------------- Commands ----------------------

        public ElementDrawVM() { }

        public ElementDrawVM(ElementVM element, NodeDrawVM[] nodes)
        {
            Nodes = nodes;
            Element = element;
            Element.PropertyChanged += OnElementPropertyChanged;
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ElementVM element)
            {
                if (e.PropertyName == (nameof(ElementVM.Selected)))
                {
                    OnPropertyChanged(nameof(ElementColor));
                    SetTextForSelection();
                }
            }
        }


        /// <summary>
        /// Sets the display text based on it being selected
        /// </summary>
        private void SetTextForSelection()
        {
            if (Element.Selected)
            {
                ElementText = $"Element {Element.Model?.ID}";
            }
            else
            {
                ElementText = "";
            }
        }



        // ---------------------- Static Methods ----------------------

        /// <summary>
        /// Calculates stress ratios and applies a green (low stress) to red (high stress)
        /// gradient color across a flat collection of elements.
        /// </summary>
        /// <param name="elements">The flat list of elements to process.</param>
        public static void ApplyStressColors(IEnumerable<ElementDrawVM> elements)
        {
            if (elements == null || !elements.Any()) return;

            double globalMaxStress = elements.Max(e => Math.Abs(e.Element.Model?.MaxStress ?? 0));

            // Apply colors to all elements iteratively
            foreach (var element in elements)
            {
                // Calculate stress fraction (0.0 to 1.0)
                double stressRatio = Math.Abs((element.Element.Model?.MaxStress ?? 0)) / globalMaxStress;

                // Safety check to ensure ratio doesn't exceed 1.0 due to floating point inaccuracies
                stressRatio = Math.Min(1.0, Math.Max(0.0, stressRatio));

                // Apply the gradient color (only is the solution is valid)
                if (element.Element.Model?.SolutionValid ?? false)
                {
                    element.ColorOverride = GetGradientColor(stressRatio);
                }
            }
        }

        /// <summary>
        /// Calculates safety factors and applies a green (high sf) to red (low sf)
        /// gradient color across a flat collection of elements.
        /// </summary>
        /// <param name="elements">The list of elements to process.</param>
        /// <param name="greenLimit">The safety factor limit to be pure green</param>
        /// <param name="yield">True to calulate based on yield safety factor</param>
        public static void ApplySafetyFactorColors(IEnumerable<ElementDrawVM> elements, double greenLimit, bool yield = true)
        {
            if (elements == null || !elements.Any()) return;

            // Apply colors to all elements iteratively
            foreach (var element in elements)
            {
                double safetyFactor = yield ? element.Element.Model?.SafetyFactorYield ?? 0 : element.Element.Model?.SafetyFactorUltimate ?? 0;

                // Normalize between the max and min
                double minSf = 1.0;
                double maxSf = greenLimit;
                safetyFactor = Math.Min(maxSf, Math.Max(minSf, safetyFactor));

                // normalize on a ratio of 0 (green) - 1 (red)
                var colorRatio = (maxSf - safetyFactor) / (maxSf - minSf);

                // Apply the gradient color (only is the solution is valid)
                if (element.Element.Model?.SolutionValid ?? false)
                {
                    element.ColorOverride = GetGradientColor(colorRatio);
                }
            }
        }


        /// <summary>
        /// Converts a ratio (0.0 to 1.0) into a Color using a green-to-red gradient.
        /// 0.0 is green, 1.0 is red.
        /// </summary>
        /// <param name="ratio">The fraction of maximum stress (0.0 to 1.0).</param>
        /// <returns>A Windows.Media.Color instance.</returns>
        private static Color GetGradientColor(double ratio)
        {
            // R component: increases from 0 (green) to 255 (red) as ratio increases.
            byte red = (byte)Math.Round(255 * ratio);

            // G component: decreases from 255 (green) to 0 (red) as ratio increases.
            byte green = (byte)Math.Round(255 * (1.0 - ratio));

            // B component: always 0 for a pure green-to-red gradient
            byte blue = 0;

            return Color.FromRgb(red, green, blue);
        }
    }
}
