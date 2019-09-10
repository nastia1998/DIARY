using DIARY_V4.Model;
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

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для UpdateNoteWindow.xaml
    /// </summary>
    public partial class UpdateNoteWindow : Window
    {
        public UpdateNoteWindow()
        {
            InitializeComponent();
        }

        public string Login { get; set; }
        public string VRow { get; set; }

        private void UpdateNoteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                DateTime Date = Convert.ToDateTime(VRow);
                var note = unitOfWork.NoteRepository.Entities
                        .FirstOrDefault(p => (p.User.Login == Login) && (p.Date == Date));

                string oldText = note.Text;
                updDate.Text = note.Date.ToShortDateString();
                updNoteRtb.AppendText(oldText);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OKBtnUpdNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);
                DateTime date = Convert.ToDateTime(VRow);

                var note = unitOfWork.NoteRepository.Entities
                            .FirstOrDefault(p => (p.User.Login == Login) && (p.Date == date));

                string rtbText = new TextRange(updNoteRtb.Document.ContentStart, updNoteRtb.Document.ContentEnd).Text;

                note.Text = rtbText;
                unitOfWork.Commit();
                MessageBox.Show("Успешно обновлено");
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CancelBtnUpdNote_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void viewAtts_Click(object sender, RoutedEventArgs e)
        {
            ShowAtts showAtts = new ShowAtts();
            showAtts.Login = Login;
            showAtts.Owner = this;
            showAtts.VRow = VRow; // Дата
            showAtts.ShowDialog();
        }
    }
}
