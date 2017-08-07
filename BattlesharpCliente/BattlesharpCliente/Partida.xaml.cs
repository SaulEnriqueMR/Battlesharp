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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Battlesharp
{
    /// <summary>
    /// Interaction logic for Partida.xaml
    /// </summary>
    public partial class Partida : Window
    {
        public Partida()
        {
            InitializeComponent();
            IniciarImg();
            IniciarImg1();
        }

        #region img
        private TranslateTransform Trasladar;
        private TransformGroup transformGroup;
        private Point Centro = new Point(0, 0);
        private Point Inicio;
        private double Angulo = 0;

        

        private void IniciarImg()
        {
            this.Trasladar = new TranslateTransform();
            this.transformGroup = new TransformGroup();
            this.transformGroup.Children.Add(this.Trasladar);
            imgBarco.RenderTransform = this.transformGroup;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Confirmación si desea abandonar partida
        }

        private void imgBarco_MouseUp(object sender, MouseButtonEventArgs e)
        {
            imgBarco.ReleaseMouseCapture();
        }

        private void imgBarco_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imgBarco.CaptureMouse();
            this.Centro = e.GetPosition(this);
            this.Inicio = new Point(this.Trasladar.X, this.Trasladar.Y);
        }

        private void imgBarco_MouseMove(object sender, MouseEventArgs e)
        {
            if (imgBarco.IsMouseCaptured)
            {
                Point clic = e.GetPosition(this);
                this.Trasladar.X = clic.X - this.Centro.X + this.Inicio.X;
                this.Trasladar.Y = clic.Y - this.Centro.Y + this.Inicio.Y;
            }
        }

        private void btnDerecha_Click(object sender, RoutedEventArgs e)
        {
            hacerdisparo(R1);
        }

        private void hacerdisparo(Rectangle e)
        {
            e.Fill = new SolidColorBrush(Colors.Red);
        }

        private void btnIzquierda_Click(object sender, RoutedEventArgs e)
        {
            Angulo += 90;
            imgBarco.LayoutTransform = new RotateTransform(Angulo);
        }

        private void imgBarco_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Angulo += 90;
            imgBarco.LayoutTransform = new RotateTransform(Angulo);
        }
        #endregion

        #region img1
        private TranslateTransform Trasladar1;
        private TransformGroup transformGroup1;
        private Point Centro1 = new Point(0, 0);
        private Point Inicio1;
        private double Angulo1 = 0;

        private void IniciarImg1()
        {
            this.Trasladar1 = new TranslateTransform();
            this.transformGroup1 = new TransformGroup();
            this.transformGroup1.Children.Add(this.Trasladar1);
            imgBarco1.RenderTransform = this.transformGroup1;
        }

        private void imgBarco1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            imgBarco1.ReleaseMouseCapture();
        }

        private void imgBarco1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imgBarco1.CaptureMouse();
            this.Centro1 = e.GetPosition(this);
            this.Inicio1 = new Point(this.Trasladar1.X, this.Trasladar1.Y);
        }

        private void imgBarco1_MouseMove(object sender, MouseEventArgs e)
        {
            if (imgBarco1.IsMouseCaptured)
            {
                Point clic = e.GetPosition(this);
                this.Trasladar1.X = clic.X - this.Centro1.X + this.Inicio1.X;
                this.Trasladar1.Y = clic.Y - this.Centro1.Y + this.Inicio1.Y;
            }
        }

        private void imgBarco1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Angulo1 += 90;
            imgBarco1.LayoutTransform = new RotateTransform(Angulo1);
        }

        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.Background = Brushes.Red;
            /*
             *Color light blue cuando no tiene nada, color rojo cuando falla y color dark gray cuando acierta
             */
        }

        private void R1_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

/*
•	Portaaviones: ocupa 4 casillas
•	Submarinos/Acorazados: ocupan 3 casillas.
•	Destructores: ocupan 2 casillas
•	Fragatas: ocupan 1 casilla

 */
