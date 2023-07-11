using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Meal_Planner.Essential
{
    public static class Essential
    {
        /// <summary>
        /// Set parent min size to work
        /// </summary>
        /// <param name="element"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void SetRelativeSize(this FrameworkElement element, double width, double height)
        {
            FrameworkElement parent = element.Parent as FrameworkElement;
            double p_width = parent.MinWidth;
            double p_height = parent.MinHeight;
            double width_percentage = parent.RenderSize.Width / p_width;
            double height_percentage = parent.RenderSize.Height / p_height;
            if(element.GetType() == typeof(Label)) 
            {
                Label label = (Label)element;
               // label.Content = width_percentage + " " + height_percentage;
            }
            element.Width = width * width_percentage;
            element.Height = height * height_percentage;
            //element.Measure(new Size(width * width_percentage, height * height_percentage));
        }

        public static void SetRelativePos(this FrameworkElement element, FrameworkElement relativeTo, double x, double y = 0, RelativeTo relativePosTo = RelativeTo.DEFAULT)
        {
            switch(relativePosTo) 
            {
                case RelativeTo.LEFT:
                    element.Margin = new Thickness(relativeTo.Margin.Left + x, y, 0, 0);
                    break;
                case RelativeTo.RIGHT:
                    element.Margin = new Thickness(relativeTo.Margin.Left + relativeTo.RenderSize.Width + x, y, 0, 0);
                    break;
                case RelativeTo.TOP:
                    element.Margin = new Thickness(x, relativeTo.Margin.Top + y, 0, 0);
                    break;
                case RelativeTo.BOTTOM:
                    element.Margin = new Thickness(x, relativeTo.Margin.Top + relativeTo.RenderSize.Height + y, 0, 0);
                    break;
                case RelativeTo.VERTICALCENTER:
                    element.Margin = new Thickness(x, relativeTo.Margin.Top + relativeTo.RenderSize.Height / 2 + y, 0, 0);
                    break;
                case RelativeTo.HORIZONTALCENTER:
                    element.Margin = new Thickness(relativeTo.Margin.Left + relativeTo.RenderSize.Width / 2 + x, y, 0, 0);
                    break;
                case RelativeTo.CENTER:
                    element.Margin = new Thickness(relativeTo.Margin.Left + relativeTo.RenderSize.Width / 2 + x, relativeTo.Margin.Top + relativeTo.RenderSize.Height / 2 + y, 0, 0);
                    break;
                default:
                    element.Margin = new Thickness(relativeTo.Margin.Left + relativeTo.RenderSize.Width + x, relativeTo.Margin.Top + y, 0, 0);
                    break;
            }
            if (relativePosTo == (RelativeTo.BOTTOM | RelativeTo.LEFT))
            {
                element.Margin = new Thickness(relativeTo.Margin.Left + x, relativeTo.Margin.Top + relativeTo.RenderSize.Height + y, 0, 0);
            }
            else if (relativePosTo == (RelativeTo.TOP | RelativeTo.RIGHT))
            {
                element.Margin = new Thickness(relativeTo.Margin.Left + x, relativeTo.Margin.Top + y, 0, 0);
            }
        }


    }

    public enum RelativeTo
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
        CENTER,
        HORIZONTALCENTER,
        VERTICALCENTER,
        DEFAULT
    }
}
