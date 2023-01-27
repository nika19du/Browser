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
    public partial class UC_Bookmark : UserControl
    {
      //  BrowserContext ctx = new BrowserContext();
        public UC_Bookmark()
        {

            InitializeComponent(); this.lbURL.Visible = false;
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; lbTitle.Text = title.Length > 15 ? title.Substring(0, 10) : title; }
        }

        private string url;

        public string URL
        {
            get { return url; }
            set { url = value; lbURL.Text = url.Length > 35 ? url.Substring(0, 30) : url; }
        }
        public bool isDeleteted = false;

        public ContextMenuStrip ContextMenuStrip1 { get; internal set; }

        private void UC_Bookmark_Load(object sender, EventArgs e)
        {


        }
        private void UC_Bookmark_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Silver;
            lbURL.Visible = true;
        }

        private void UC_Bookmark_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.Transparent;
            lbURL.Visible = false;
        }

        private void UC_Bookmark_Click_1(object sender, EventArgs e)
        {
            lbURL.Visible = true;
            Form1.instance.NewBookmarkTab(URL, Title);
            //Form1.instance.URL = URL;
            //Form1.instance.Search(); 

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = BookmarksList.instance.CurrentUser;
                var ctx = Form1.instance.context;
                var delBookmark = ctx.Bookmarks.FirstOrDefault(x => x.UserId == currentUser.Id && x.Title == this.title && x.URL == this.url);
                currentUser.Bookmarks.Remove( delBookmark);
                ctx.Bookmarks.Remove(delBookmark); 
                ctx.SaveChanges();
                this.isDeleteted = true;
                this.title = null;
                this.url = null;
                currentUser = ctx.Users.FirstOrDefault(x => x.Id == currentUser.Id);
                BookmarksList.instance.ShowBookmarks(currentUser);
            }
            catch(Exception ex)
            {
                MessageBox.Show("UC_Bookmark " + ex.Message); 
            }
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            this.btnDelete.ForeColor = Color.Maroon;
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            this.btnDelete.ForeColor = Color.Black; 
        }
    }
}
