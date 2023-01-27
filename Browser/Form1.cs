using System;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Browser;
using System.Configuration;
using System.Data.SqlClient;
using EasyTabs;
using System.Drawing;
using System.Security.Policy;
using System.Collections.Generic;

namespace Browser
{

    public partial class Form1 : Form
    {
        public BrowserContext context = new BrowserContext();
        public User currentUser = new User();
        ChromiumWebBrowser chrome = new ChromiumWebBrowser();
        internal string URL;
        internal string Title;
        protected bool isClicked = false;//is bookmark btn clicked 
        public static Form1 instance;
        public Form1(User user, string URL, string Title, bool isClicked)
        {
            currentUser = user;
            this.URL = URL;
            this.Title = Title;
            this.isClicked = isClicked;
            if (isClicked != true)
                this.AddToFav();
            else
            {
                UserSession.Login(currentUser);
                currentUser = GetLoggedUser();
            }
        }
        public Form1()
        {
            InitializeComponent();
            instance = this;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CefSettings settings = new CefSettings();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(settings);
            InitChrome();
            NewTab();
            DownloadHandler downloadHandler = new DownloadHandler();
            chrome.DownloadHandler = downloadHandler;
        }
        public void InitChrome()
        {
            txtUrl.Text = "https://www.google.com";
            chrome = new ChromiumWebBrowser(txtUrl.Text);
            chrome.Parent = tabControl.SelectedTab;
            chrome.Dock = DockStyle.Fill;
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.TitleChanged += Chrome_TitleChanged;
        }
        private void Chrome_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                txtUrl.Text = e.Address;
                this.URL = e.Address;
            }));
        }
        private void Chrome_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                tabControl.SelectedTab.Text = e.Title;
                //  if (tabControl.SelectedTab.Text.Length > 15)
                this.Title = e.Title;
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
            Application.Exit();
        }

        private void btnNewTab_Click(object sender, EventArgs e)
        {
            NewTab();
        }
        public void NewBookmarkTab(string url, string title)
        {
            this.txtUrl.Enabled = true;
            this.URL = url;
            this.Title = title;
            TabPage tab = new TabPage();
            tab.Text = title.Length > 15 ? title.Substring(0, 10) : title;
            tabControl.Controls.Add(tab);
            tabControl.SelectedTab = tab;
            tabControl.SelectTab(tabControl.TabCount - 1);
            this.chrome = new ChromiumWebBrowser(url);
            chrome.Parent = tab; //bez tva ne bachka 

            chrome.LoadUrl(url);

            chrome.Dock = DockStyle.Fill;
        }
        public void NewTab()
        {
            TabPage tab = new TabPage();
            tabControl.Controls.Add(tab);
            tabControl.SelectTab(tabControl.TabCount - 1);
            this.chrome = new ChromiumWebBrowser("https://www.google.com");
            //  chrome.Parent = this.tabControl.SelectedTab;
            chrome.Parent = tab;
            chrome.Dock = DockStyle.Fill;
            txtUrl.Text = "https://www.google.com";
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.TitleChanged += Chrome_TitleChanged;
            if (tab != currentBookmark)
            {
                this.txtUrl.Enabled = true;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrome = tabControl.SelectedTab.Controls[0] as ChromiumWebBrowser;
            if (chrome != null)
            {
                if (chrome.CanGoBack)
                    chrome.Back();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrome = tabControl.SelectedTab.Controls[0] as ChromiumWebBrowser;
            if (chrome != null)
            {
                if (chrome.CanGoForward)
                    chrome.Forward();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            chrome.Reload();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //  ChromiumWebBrowser chrome = tabControl.SelectedTab.Controls[0] as ChromiumWebBrowser;
            //if (chrome != null)
            //    chrome.Load(txtUrl.Text);
            this.Search();
        }
        public void AddToFav()
        {
            if (currentUser == null)
            {//create new user or login; redirect to new form 
                Register register = new Register();
                register.Title = this.Title;
                register.URL = this.URL;
                register.isClicked = true;
                register.ShowDialog();
            }
            else
            {
                currentUser = context.Users.FirstOrDefault(x => x.Id == currentUser.Id && x.IsLogIn == true);
                Bookmark bookmark = new Bookmark();
                bookmark.URL = this.URL;

                //if (currentBookmark != null)
                //{
                //    currentUser = BookmarksList.instance.CurrentUser;
                //    UserSession.Login(currentUser);
                //      context.SaveChanges();
                //} 
                if (this.tabControl.SelectedTab.Text == "Bookmarks")//this.bookmarksToolStripMenuItem.Enabled == false)
                {
                    MessageBox.Show("Can not added this tab!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (currentUser.Bookmarks.FirstOrDefault(x => x.URL == bookmark.URL) != null)
                {
                    MessageBox.Show("This site is already added to bookmarks!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    bookmark.Title = this.Title;
                    bookmark.User = this.currentUser;
                    bookmark.UserId = this.currentUser.Id;
                    context.Bookmarks.Add(bookmark);
                    context.Users.Attach(currentUser);
                    currentUser.Bookmarks.Add(bookmark);
                    context.SaveChanges();
                    MessageBox.Show("Successfully added bookmark!");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            MessageBox.Show("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
                UserSession.Login(currentUser);
            }
        }

        private void btnBookmark_Click(object sender, EventArgs e)
        {
            currentUser = this.GetLoggedUser();// UserSession.GetUser();
            this.AddToFav();

        }
        public User GetLoggedUser()
        {
            var current = UserSession.GetUser();
            return current;
        }
        private const string baseURL = "https://www.google.com/search?q=";
        public void Search()
        {
            if (txtUrl.Text.Length <= 0)
            {
                //this will clear all search result
                chrome.StopFinding(true);
            }
            else
            {
                this.URL = txtUrl.Text;
                string input = $"{baseURL}{txtUrl.Text}";
                if (currentBookmark != null) this.NewSearchTab(input);
                else chrome.LoadUrl(input);

                //chrome = new ChromiumWebBrowser(input); 
                //     tabControl.SelectedTab.Text = tabControl.SelectedTab.Text.Substring(0, 5);
                // this.tabControl.SelectedTab.Text = this.tabControl.SelectedTab.Text.Substring(0, tabControl.SelectedTab.Text.Length / 2);
            }
        }
        public void NewSearchTab(string input)
        {
            TabPage tab = new TabPage();
            tabControl.Controls.Add(tab);
            tabControl.SelectTab(tabControl.TabCount - 1);
            this.chrome = new ChromiumWebBrowser(input);
            //  chrome.Parent = this.tabControl.SelectedTab;
            chrome.Parent = tab;
            chrome.Dock = DockStyle.Fill;
            // txtUrl.Text = "https://www.google.com";
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.TitleChanged += Chrome_TitleChanged;
            if (tab != currentBookmark)
            {
                this.txtUrl.Enabled = true;
            }
        }
        private void txtUrl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.Search();
            }
        }
        public TabPage currentBookmark;
        private void bookmarksToolStripMenuItem_MouseClick(object sender, EventArgs e)
        {
            try
            {
                currentUser = GetLoggedUser();
                // currentUser = context.Users.FirstOrDefault(x => x.Id == currentUser.Id);// GetLoggedUser(); 
                if (currentUser == null)
                {
                    MessageBox.Show("You should log in first!", "Log in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    BookmarksList bookmarksList = new BookmarksList(this.currentUser);
                    TabPage tabPage = new TabPage();
                    tabPage.Controls.Add(bookmarksList);
                    tabPage.Width = bookmarksList.Width;
                    tabPage.Text = "Bookmarks";
                    this.tabControl.TabPages.Add(tabPage);
                    tabControl.SelectedTab = tabPage;
                    if (currentBookmark == null)
                        currentBookmark = tabPage;
                    this.bookmarksToolStripMenuItem.Enabled = false;
                    this.txtUrl.Enabled = false;
                    this.currentUser = BookmarksList.instance.CurrentUser;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.loginToolStripMenuItem.Text == "Log out")
            {
                if (currentBookmark != null)
                    tabControl.TabPages.Remove(currentBookmark);
                this.bookmarksToolStripMenuItem.Enabled = true;
                this.currentBookmark = null;
                this.currentUser = null;
                UserSession.Logout();
                this.loginToolStripMenuItem.Text = "Log in";
            }
            else
            {
                Login login = new Login();
                login.ShowDialog();
                currentUser = GetLoggedUser();
                if (currentUser != null)
                    this.loginToolStripMenuItem.Text = "Log out";
            }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == currentBookmark)
            {
                currentBookmark = null;
                this.bookmarksToolStripMenuItem.Enabled = true;
            }
            if (tabControl.SelectedTab != null)
                this.tabControl.TabPages.Remove(this.tabControl.SelectedTab);
        }

        private void updateInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentUser = GetLoggedUser();
            if (currentUser == null) MessageBox.Show("You're not logged in!", "Try again!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Register register = new Register(currentUser);

                register.isUpdate = true;
                register.ShowDialog();
            }
        }

        private void tabControl_MouseUp(object sender, MouseEventArgs e)
        {
            // check if the right mouse button was pressed
            if (e.Button == MouseButtons.Left)
            {
                // iterate through all the tab pages
                if (this.tabControl.SelectedTab == currentBookmark)
                {
                    //   bookmarksToolStripMenuItem_MouseClick(sender, e);

                    BookmarksList.instance.ShowBookmarks(this.currentUser);

                    this.txtUrl.Enabled = false;
                }
                else
                {
                    this.txtUrl.Enabled = true;
                }
            }
        }
    }
}
