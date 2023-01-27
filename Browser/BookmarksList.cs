using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
    public partial class BookmarksList : UserControl
    {
        public static BookmarksList instance; 
        public BookmarksList()
        {
            InitializeComponent();
            instance = this;
        }
        public User CurrentUser { get; set; }
        public BookmarksList(User currentUser) : this()
        {
            CurrentUser = currentUser;
        }
        private void BookmarksList_Load(object sender, EventArgs e)
        {
            ShowBookmarks();
        }

        public void ShowBookmarks(User a=null)
        {
            //var a = contextMenuStrip1.Name.Where(x => x.ToString() == "Delete");
            try
            {
                //  CurrentUser = ctx.Users.FirstOrDefault(x => x.Id == CurrentUser.Id);
                if (a != null) CurrentUser = a;
                flowLayoutPanel1.Controls.Clear();
                foreach (var item in CurrentUser.Bookmarks)
                {
                    UC_Bookmark uC_Bookmark = new UC_Bookmark();
                    uC_Bookmark.Title = item.Title;
                    uC_Bookmark.URL = item.URL; 
                    if (uC_Bookmark.isDeleteted && uC_Bookmark.Title==null && uC_Bookmark.URL==null)
                        this.flowLayoutPanel1.Controls.Remove(uC_Bookmark);
                    else
                    {
                       
                        this.flowLayoutPanel1.Controls.Add(uC_Bookmark);
                    }

                    //Form1.instance.DeleteStip.Checked.+=  DeletetHandler(item.URL, item.Title);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("BookmarksList "+ex.Message);
            }
        }
    }
}
