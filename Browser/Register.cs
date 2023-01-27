using Browser;
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
    public partial class Register : Form
    {
        public BrowserContext context = new BrowserContext();
        public string URL;
        public string Title;
        public bool isClicked;
        public bool isUpdate = false;

        public User CurrentUser { get; }
        public User c { get; set; } = new User();
        public Register()
        {
            InitializeComponent();

        }

        public Register(User currentUser) : this()
        {
            CurrentUser = currentUser;
            c = context.Users.FirstOrDefault(x => x.Id == CurrentUser.Id);
            this.txtUserName.Text = c.UserName;
            this.txtEmail.Text = c.Email;

            this.label1.Text = "Update Info";
            this.btnLogIn.Hide();
            this.btnCreateAccount.Text = "Update";
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUserName.Text;
                string email = txtEmail.Text;
                string pass = txtPass.Text;
                string confPass = txtConfPass.Text;


                if (isUpdate)
                { 
                    UpdateUser(username, pass, confPass, email);
                    return;
                }
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(confPass))
                {
                    MessageBox.Show("Fill in the empty form!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (context.Users.FirstOrDefault(x => x.UserName == username) != null)
                {
                    MessageBox.Show("Username already exist!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (pass == confPass)
                {
                    User user = new User();
                    user.UserName = username;
                    user.Email = email;
                    user.Password = pass;
                    context.Users.Add(user);
                    context.SaveChanges();
                    MessageBox.Show("Successfully created account!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    Login login = new Login();
                    login.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please make sure your passwords match.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
         

        private void UpdateUser(string username, string pass, string confPass, string email)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(pass) && string.IsNullOrWhiteSpace(confPass))
            {
                MessageBox.Show("Fill in the empty form!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (pass != confPass)
            {
                MessageBox.Show("Please make sure your passwords match.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(username) )// !string.IsNullOrWhiteSpace(pass))
            {
                c.UserName = username;
                c.Password = !string.IsNullOrWhiteSpace(pass) ? pass :  CurrentUser.Password;
            }
            if (!string.IsNullOrWhiteSpace(email)) c.Email = email;
            else c.Email = CurrentUser.Email;
            context.SaveChanges();
            MessageBox.Show("Successfully updated account!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnLogIn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.URL = URL;
            login.Title = Title;
            login.isClicked = this.isClicked;
            login.ShowDialog();

        }

        private void txtConfPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.btnCreateAccount_Click(sender, e);
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
