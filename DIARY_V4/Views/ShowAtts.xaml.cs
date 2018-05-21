using DIARY_V4.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для ShowAtts.xaml
    /// </summary>
    public partial class ShowAtts : Window
    {
        public string Login { get; set; }
        public string VRow { get; set; }
        public static int cphotos = 0;
        public ShowAtts()
        {
            InitializeComponent();

            MyImageControl0.RenderTransform = transform;
            MyImageControl1.RenderTransform = transform;
            MyImageControl2.RenderTransform = transform;
        }

        private void ShowAtts_Loaded(object sender, RoutedEventArgs e)
        {
            var dbContext = new BaseDbContext();

            var unitOfWork = new UnitOfWork(dbContext);
            DateTime Date = Convert.ToDateTime(VRow);

            var note = unitOfWork.NoteRepository.Entities
                       .FirstOrDefault(b => (b.User.Login == Login) && (b.Date == Date));

            var photos = unitOfWork.PhotosRepository.Entities
                        .Where(n => n.Id_Note == note.Id_Note && n.Note.Id_User == note.User.Id).ToList();

            string allPaths = "";
            foreach (var p in photos)
            {
                allPaths = allPaths + p.Path + ';';
            }

            string[] parts = allPaths.Split(';');

            if (parts[0] != "")
            {
                bitmap1 = new BitmapImage(new Uri(parts[0]));
                cphotos++;
                MyImageControl0.Source = bitmap1;
                Next.IsEnabled = false;
                //Next.ToolTip = "У вас есть только одна фотография";
            }
            if (cphotos == 1)
            {
                if (parts[1] != "")
                {
                    bitmap2 = new BitmapImage(new Uri(parts[1]));
                    cphotos++;
                    MyImageControl1.Source = bitmap2;
                    Next.IsEnabled = true;
                }
            }
            if (cphotos == 2)
            {
                if (parts[2] != "")
                {
                    bitmap3 = new BitmapImage(new Uri(parts[2]));
                    cphotos++;
                    MyImageControl2.Source = bitmap3;
                    Next.IsEnabled = true;
                }
            }

            else cphotos++;
           
        }

        bool a = true;
        bool b = true;
        bool c = false;
        DoubleAnimation anim = new DoubleAnimation();
        TranslateTransform transform = new TranslateTransform();

        BitmapImage bitmap1;
        BitmapImage bitmap2;
        BitmapImage bitmap3;
        private void Next_Click(object sender, RoutedEventArgs e)
        {          
            if(cphotos == 2)
            {
                if(a)
                {
                    anim.To = 500;
                    a = false;
                }
                else
                {
                    anim.To = 0;
                    a = true;
                }
                transform.BeginAnimation(TranslateTransform.XProperty, anim);
            }
            
            if(cphotos == 3)
            {
                if (b && !c)
                {
                    anim.To = 500;
                    b = false;
                    c = true;
                    transform.BeginAnimation(TranslateTransform.XProperty, anim);
                    
                }
                else if (!b && c)
                {
                    anim.To = 1000;
                    c = false;
                    transform.BeginAnimation(TranslateTransform.XProperty, anim);
                }
                else if(!b && !c)
                {
                    anim.To = 500;
                    b = true;
                    c = true;
                    transform.BeginAnimation(TranslateTransform.XProperty, anim);
                }
                else if(b && c)
                {
                    anim.To = 0;
                    b = true;
                    c = false;
                    transform.BeginAnimation(TranslateTransform.XProperty, anim);
                }
            }

           
        }

       
    }
}
